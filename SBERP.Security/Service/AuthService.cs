using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SBERP.EmailService.Models;
using SBERP.EmailService.Service;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Persistence;
using SBERP.Shared.Authorization;
using SBERP.Shared.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SBERP.Security.Service
{
    /// <summary>
    /// It implements <see cref="IAuthService"/> where all the methods inclduing login or user authentication, refresh token, and revoke user from loggedin.
    /// </summary>
    public class AuthService : IAuthService
    {
        #region Variable declaration & constructor initialization

        private readonly IConfiguration _configuration;
        private readonly SecurityDBContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly ITokenService _tokenService;
        private readonly IRoleMenuService _roleMenuService;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisSettings _redisSettings;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        /// <param name="tokenService"></param>
        /// <param name="roleMenuService"></param>
        public AuthService(IConfiguration config, SecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, ITokenService tokenService, IRoleMenuService roleMenuService,
        IDistributedCache distributedCache, IOptions<RedisSettings> redisSettings, ITokenBlacklistService tokenBlacklistService,
        IRefreshTokenService refreshTokenService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = config;
            _context = context;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _tokenService = tokenService;
            _roleMenuService = roleMenuService;
            _distributedCache = distributedCache;
            _redisSettings = redisSettings.Value;
            _tokenBlacklistService = tokenBlacklistService;
            _refreshTokenService = refreshTokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        /// <summary>
        /// <para>EF Codeblock: AuthenticateUserAsync</para> 
        /// This method authenticate user credential. It checks user name and then password. In between the checking, if client attempts consecutive 
        /// 3 failed request then this method will block the any further request for authentication of the user. Where, It update the datetime
        /// of the failed attempts and count of failed attempts. So threshold(appsettings.json) says after 3 failed attempts, user get blocked for the 
        /// next 1 min. This method ensures the unique username for all the user records.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        public async Task<DataResponse> AuthenticateUserAsync(LoginRequest? request)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, LogSafe.Serialize(new { request?.UserName }, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                AppUser? oExistAppUser = await _context.AppUsers!.Include(u => u.AppUserProfile).FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);

                if (Utilities.IsNull(oExistAppUser))
                {
                    // same BCrypt cost as a real check, so the response time does not reveal unknown user names
                    BCryptNet.Verify(request?.Password ?? string.Empty, TimingDummyHash);
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.AUTH_INVALID_CREDENTIAL_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                // locked: refuse even the right password until the lockout ends
                DateTime now = DateTime.UtcNow;
                if (oExistAppUser!.LockoutEnd > now)
                    return LockedResponse(oExistAppUser.LockoutEnd.Value, now);

                if (!BCryptNet.Verify(request!.Password, oExistAppUser.Password))
                {
                    DateTime? lockedUntil = await RegisterFailedLoginAsync(oExistAppUser, now);
                    if (lockedUntil.HasValue)
                    {
                        _securityLogService.LogWarning($"Login of user {oExistAppUser.Id} locked until {lockedUntil:u} after {MaxFailedAttempts} failed attempts.");
                        await SendLockNoticeAsync(request, oExistAppUser);
                        return LockedResponse(lockedUntil.Value, now);
                    }
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.AUTH_INVALID_CREDENTIAL_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                if (oExistAppUser.FailedLoginAttempts != 0 || oExistAppUser.LockoutEnd != null)
                    await _context.AppUsers!.Where(u => u.Id == oExistAppUser.Id)
                        .ExecuteUpdateAsync(s => s.SetProperty(u => u.FailedLoginAttempts, 0).SetProperty(u => u.LockoutEnd, (DateTime?)null));

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(x => x.Id == oExistAppUser.AppUserProfileId && x.IsActive == true);

                if (Utilities.IsNull(oAppUserProfile))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                ClaimRequest oClaimRequest = await BuildClaimsAsync(oExistAppUser.Id, oExistAppUser.UserName, oAppUserProfile!);
                string accessToken = _tokenService.GenerateJwtToken(oClaimRequest);
                if (Utilities.IsNullOrEmptyOrWhiteSpace(accessToken))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED_GENRATE_JWT_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }
                string refreshToken = await _refreshTokenService.IssueAsync(oExistAppUser.Id, ClientIp(), ClientUserAgent());

                TokenResponse oTokenResponse = new() { access_token = accessToken, refresh_token = refreshToken };


                return new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResponse };

            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in AuthenticateUserAsync: {Ex.Message}");
                throw new Exception(ConstantSupplier.AUTH_EXCEPTION_MSG);
            }
        }

        public async Task<DataResponse> GetAppUserProfileMenuAsync(string userId)
        {
            _securityLogService.LogInfo(String.Format(
                ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_REQ_MSG,
                LogSafe.Serialize(userId, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                if (Utilities.IsNullOrEmptyOrWhiteSpace(userId))
                    return Utilities.FailedResponse(nullValue, _securityLogService,
                        (int)HttpStatusCode.Unauthorized, null,
                        ConstantSupplier.APP_USER_PROFILE_MENU_INVALID_TOKEN_MSG,
                        ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                // ── Step 1: Try Redis cache (IDistributedCache.GetStringAsync) ────────
                string cacheKey = await MenuCache.UserMenuKeyAsync(_distributedCache, userId);
                try
                {
                    var cached = await _distributedCache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cached))
                    {
                        _securityLogService.LogInfo(
                            $"Menu cache HIT for userId: {userId}");
                        return JsonConvert.DeserializeObject<DataResponse>(cached)!;
                    }
                }
                catch (Exception cacheEx)
                {
                    // Redis unavailable — fall back to DB
                    _securityLogService.LogWarning(
                        $"Redis read failed for {userId}: {cacheEx.Message}. Fallback to DB.");
                }

                // ── Step 2: Cache MISS — fetch from database ───────────────────────────
                _securityLogService.LogInfo(
                    $"Menu cache MISS for userId: {userId}. Fetching from DB.");

                AppUser? oExistAppUser = await _context.AppUsers!
                    .Include(u => u.AppUserProfile)
                    .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId)
                                           && u.IsActive == true);

                if (Utilities.IsNull(oExistAppUser))
                    return Utilities.FailedResponse(nullValue, _securityLogService,
                        (int)HttpStatusCode.NotFound, null,
                        ConstantSupplier.INVALID_USER_MSG,
                        ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!
                    .FirstOrDefaultAsync(x => x.Id == oExistAppUser!.AppUserProfileId
                                           && x.IsActive == true);

                if (Utilities.IsNull(oAppUserProfile))
                    return Utilities.FailedResponse(nullValue, _securityLogService,
                        (int)HttpStatusCode.BadRequest, null,
                        ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG,
                        ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                DataResponse oUserMenuResponse = await _roleMenuService
                    .GetAllAppUserMenuByUserIdAsync(oAppUserProfile!.Id.ToString());

                if (!oUserMenuResponse.Success)
                    return Utilities.FailedResponse(nullValue, _securityLogService,
                        (int)HttpStatusCode.BadRequest, null,
                        ConstantSupplier.AUTH_FAILED_RETRIEVE_MENU_MSG,
                        ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                ProfileMenuResponse oProfileMenuResponse = new()
                {
                    user = new User
                    {
                        Id = Convert.ToString(oAppUserProfile.Id),
                        FullName = oAppUserProfile.FullName,
                        UserName = oExistAppUser!.UserName,
                        Email = oAppUserProfile.Email,
                        UserRole = oAppUserProfile.AppUserRoleId.ToString(),
                        RoleName = await _context.AppUserRoles!.Where(r => r.Id == oAppUserProfile.AppUserRoleId)
                                                               .Select(r => r.RoleName).FirstOrDefaultAsync(),
                        JobTitle = oAppUserProfile.JobTitle,
                        EmployeeId = oAppUserProfile.EmployeeId?.ToString(),
                        CreatedDate = oAppUserProfile.CreatedDate
                    },
                    userMenus = Convert.ToString(oUserMenuResponse.Result)
                };

                var successResponse = new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.APP_USER_PROFILE_MENU_SUCCESS_MSG,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.OK,
                    Result = oProfileMenuResponse
                };

                // ── Step 3: Store in Redis (IDistributedCache.SetStringAsync) ─────────
                try
                {
                    await _distributedCache.SetStringAsync(
                        cacheKey,
                        JsonConvert.SerializeObject(successResponse),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow =
                                TimeSpan.FromMinutes(_redisSettings.MenuCacheExpiryMinutes),
                            SlidingExpiration =
                                TimeSpan.FromMinutes(_redisSettings.MenuCacheSlidingExpiryMinutes)
                        });
                    _securityLogService.LogInfo(
                        $"Menu cached. Key:{cacheKey}. TTL:{_redisSettings.MenuCacheExpiryMinutes}min");
                }
                catch (Exception cacheEx)
                {
                    _securityLogService.LogWarning(
                        $"Redis write failed for {userId}: {cacheEx.Message}");
                }

                return successResponse;
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(
                    $"Exception in GetAppUserProfileMenuAsync: {Ex.Message}");
                throw new Exception(
                    ConstantSupplier.APP_USER_PROFILE_MENU_SERVICE_EXCEPTION_MSG);
            }
        }

        public async Task InvalidateMenuCacheAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            try
            {
                // IDistributedCache.RemoveAsync — Microsoft official pattern
                await _distributedCache.RemoveAsync(await MenuCache.UserMenuKeyAsync(_distributedCache, userId));
                _securityLogService.LogInfo(
                    $"Menu cache invalidated for userId: {userId}");
            }
            catch (Exception ex)
            {
                _securityLogService.LogWarning(
                    $"Cache invalidation failed for userId {userId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Renews the access token. The refresh token is rotated: the presented one is revoked and a
        /// new one is returned, in the same session. A rotated token that comes back again was copied,
        /// so that whole session is signed out (reuse detection). The expired access token must be one
        /// this service signed for the same user, and the user and profile must still be active.
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        /// <exception cref="Exception"></exception>
        public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest? tokenRequest)
        {
            // tokens are credentials: never log them
            _securityLogService.LogInfo(ConstantSupplier.REFRESHTOKEN_STARTED_INFO_MSG);
            object? nullValue = null;
            try
            {
                if (string.IsNullOrWhiteSpace(tokenRequest?.Refresh_Token))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.REFRESHTOKEN_REQUIRED_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                ClaimsPrincipal? principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.Access_Token);
                if (principal == null || !Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.REFRESHTOKEN_ACCESS_TOKEN_MISMATCH_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                RefreshTokenRotation rotation = await _refreshTokenService.RotateAsync(tokenRequest.Refresh_Token, userId, ClientIp(), ClientUserAgent());
                switch (rotation.Status)
                {
                    case RefreshTokenStatus.Reused:
                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.REFRESHTOKEN_REUSE_DETECTED_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                    case RefreshTokenStatus.Expired:
                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.REFRESHTOKEN_SESSION_ENDED_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                    case RefreshTokenStatus.Invalid:
                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.REFRESHTOKEN_INVALID_EXPIRED_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                AppUser user = rotation.User!;
                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.AppUserProfileId && x.IsActive == true);
                if (Utilities.IsNull(oAppUserProfile))
                {
                    await _refreshTokenService.RevokeFamilyAsync(rotation.FamilyId!.Value, ClientIp(), ConstantSupplier.TOKEN_REVOKE_USER_DEACTIVATED);
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                ClaimRequest oClaimRequest = await BuildClaimsAsync(user.Id, user.UserName, oAppUserProfile!);
                string accessToken = _tokenService.GenerateJwtToken(oClaimRequest);
                if (Utilities.IsNullOrEmptyOrWhiteSpace(accessToken))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED_GENRATE_JWT_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                TokenResponse oTokenResponse = new() { access_token = accessToken, refresh_token = rotation.NewToken };

                return new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResponse };
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in RefreshTokenAsync: {Ex.Message}");
                throw new Exception(ConstantSupplier.REFRESHTOKEN_EXCEPTION_MSG);
            }
        }


        /// <summary>
        /// Logout: signs out the session of the given refresh token and blacklists the access token from
        /// the Authorization header. Only an access token this service signed is blacklisted (an
        /// unvalidated token could name any JTI). Always succeeds, so it cannot be used to probe tokens.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessToken">Raw bearer token from the Authorization header, if any.</param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        public async Task<DataResponse> RevokeAsync(RevokeRequest? request, string? accessToken)
        {
            _securityLogService.LogInfo(ConstantSupplier.REVOKE_STARTED_INFO_MSG);

            try
            {
                Guid? userId = null;
                if (!string.IsNullOrWhiteSpace(request?.Refresh_Token))
                    userId = await _refreshTokenService.RevokeSessionAsync(request.Refresh_Token, ClientIp(), ConstantSupplier.TOKEN_REVOKE_LOGOUT);

                ClaimsPrincipal? principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                if (principal != null)
                {
                    JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                    TimeSpan remaining = jwt.ValidTo - DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(jwt.Id) && remaining > TimeSpan.Zero)
                        await _tokenBlacklistService.BlacklistAsync(jwt.Id, remaining);

                    if (!userId.HasValue && Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out Guid tokenUserId))
                        userId = tokenUserId;
                }

                if (userId.HasValue)
                    await InvalidateMenuCacheAsync(userId.Value.ToString());

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.REVOKE_SUCCESSFUL_MSG,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in RevokeAsync: {Ex.Message}");
                return new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.REVOKE_EXCEPTION_MSG,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// The token's claims, with the role's current rights from AppUserRoleMenus. Rebuilt on every refresh,
        /// so a change to a role reaches its users within one access token lifetime.
        /// </summary>
        private async Task<ClaimRequest> BuildClaimsAsync(Guid userId, string? userName, AppUserProfile profile)
        {
            var rows = await _context.AppUserRoleMenus!.AsNoTracking()
                .Where(rm => rm.AppUserRoleId == profile.AppUserRoleId && rm.IsActive == true
                             && rm.AppUserMenu!.IsActive == true && rm.AppUserMenu.RouteLink != null && rm.AppUserMenu.RouteLink != "")
                .Select(rm => new { rm.AppUserMenu!.RouteLink, rm.IsView, rm.IsCreate, rm.IsUpdate, rm.IsDelete })
                .ToListAsync();

            return new ClaimRequest
            {
                UserId = userId.ToString(),
                UserName = userName,
                Email = profile.Email,
                Role = profile.AppUserRoleId.ToString(),
                ProfileId = profile.Id.ToString(),
                EmployeeId = profile.EmployeeId?.ToString(),
                Permissions = ScreenPermissions.Encode(rows.Select(r => (r.RouteLink!,
                    (r.IsView == true ? ScreenAction.View : 0) | (r.IsCreate == true ? ScreenAction.Create : 0)
                    | (r.IsUpdate == true ? ScreenAction.Update : 0) | (r.IsDelete == true ? ScreenAction.Delete : 0))))
            };
        }

        /// <summary>A valid BCrypt hash of a random value, verified against when the user name is unknown.</summary>
        private static readonly string TimingDummyHash = BCryptNet.HashPassword(Guid.NewGuid().ToString(), BCryptNet.GenerateSalt(10));

        private int MaxFailedAttempts => _appSettings?.MaxNumberOfFailedAttempts > 0 ? _appSettings.MaxNumberOfFailedAttempts : 3;
        private int BlockMinutes => _appSettings?.BlockMinutes > 0 ? _appSettings.BlockMinutes : 15;

        /// <summary>
        /// Counts a failed login. The increment is one atomic UPDATE, so parallel attempts are all counted.
        /// When the count reaches AppSettings:MaxNumberOfFailedAttempts the login is locked for
        /// AppSettings:BlockMinutes and the count starts again. An expired lockout starts a new count.
        /// </summary>
        /// <returns>The lockout end when this failure locked the account, otherwise <c>null</c>.</returns>
        private async Task<DateTime?> RegisterFailedLoginAsync(AppUser user, DateTime now)
        {
            if (user.LockoutEnd != null && user.LockoutEnd <= now)
                await _context.AppUsers!.Where(u => u.Id == user.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.FailedLoginAttempts, 0).SetProperty(u => u.LockoutEnd, (DateTime?)null));

            await _context.AppUsers!.Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.FailedLoginAttempts, u => u.FailedLoginAttempts + 1));
            int attempts = await _context.AppUsers!.AsNoTracking().Where(u => u.Id == user.Id).Select(u => u.FailedLoginAttempts).FirstAsync();
            if (attempts < MaxFailedAttempts)
                return null;

            DateTime lockoutEnd = now.AddMinutes(BlockMinutes);
            await _context.AppUsers!.Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.FailedLoginAttempts, 0).SetProperty(u => u.LockoutEnd, lockoutEnd));
            return lockoutEnd;
        }

        private DataResponse LockedResponse(DateTime lockoutEnd, DateTime now)
        {
            int minutes = Math.Max(1, (int)Math.Ceiling((lockoutEnd - now).TotalMinutes));
            object? nullValue = null;
            return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Locked, null,
                string.Format(ConstantSupplier.AUTH_ACCOUNT_LOCKED_MSG, minutes), ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
        }

        /// <summary>Best effort: a failing mail server must not change the login response.</summary>
        private async Task SendLockNoticeAsync(LoginRequest? request, AppUser user)
        {
            try
            {
                await SendEmail(request, user);
            }
            catch (Exception ex)
            {
                _securityLogService.LogWarning($"Lockout notice e-mail for user {user.Id} failed: {ex.Message}");
            }
        }

        /// <summary>Client IP for the token audit columns: first X-Forwarded-For entry (set by the gateway), else the connection.</summary>
        private string? ClientIp()
        {
            HttpContext? http = _httpContextAccessor.HttpContext;
            if (http == null) return null;
            string? forwarded = http.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
            string? ip = string.IsNullOrWhiteSpace(forwarded) ? http.Connection.RemoteIpAddress?.ToString() : forwarded;
            return ip == null || ip.Length <= 64 ? ip : ip[..64];
        }

        private string? ClientUserAgent() => _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

        /// <summary>
        /// Send email regarding the user blocked after 3 times incorrect login attempt.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns><see cref="Task{SendResponse}"/></returns>
        private async Task<SendResponse> SendEmail(LoginRequest? request, AppUser? user)
        {
            string body = _emailService.PopulateBody("EmailTemplates/emailblocknotice.html", user?.UserName,
                                    "User management", _configuration["Onboarding:LoginUrl"] ?? "http://localhost:4200/",
                                    "Please check your username and password. Please wait for mentioned time to re-login");
            string? email = _context.AppUserProfiles?.Where(u => u.Id == user!.AppUserProfileId && u.IsActive == true).SingleOrDefault()?.Email;
            if (Utilities.IsNullOrEmpty(email))
                return new() { ErrorMessages = new List<string> { "No valid email address found" }, MessageId = "" };

            Message message = new() { To = email, Name = request?.UserName, Subject = "Email Blocked", Body = body };
            return await _emailService.SendEmailAsync(_appSettings!.EmailConfiguration, message);
        }

    }
}
