using Azure;
using Azure.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using SBERP.DataAccessLayer;
using SBERP.EmailService.Models;
using SBERP.EmailService.Service;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Persistence;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
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

        public IConfiguration _configuration;
        private readonly SecurityDBContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly IDatabaseManager _dbmanager;
        private readonly ITokenService _tokenService;
        private readonly IRoleMenuService _roleMenuService;


        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        /// <param name="dbManager"></param>
        /// <param name="tokenService"></param>
        /// <param name="roleMenuService"></param>
        public AuthService(IConfiguration config, SecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager, ITokenService tokenService, IRoleMenuService roleMenuService)
        {
            _configuration = config;
            _context = context;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
            _tokenService = tokenService;
            _roleMenuService = roleMenuService;
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
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                AppUser? oExistAppUser = await _context.AppUsers!.Include(u => u.AppUserProfile).FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);

                if (Utilities.IsNull(oExistAppUser) || !BCryptNet.Verify(request!.Password, oExistAppUser!.Password))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.AUTH_INVALID_CREDENTIAL_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(x => x.Id == oExistAppUser.AppUserProfileId && x.IsActive == true);

                if (Utilities.IsNull(oAppUserProfile))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                ClaimRequest oClaimRequest = new() { UserId = oExistAppUser.Id.ToString(), UserName = request.UserName, Email = oAppUserProfile.Email, Role = oAppUserProfile.AppUserRoleId.ToString() };
                string accessToken = _tokenService.GenerateJwtToken(oClaimRequest);
                if (Utilities.IsNullOrEmptyOrWhiteSpace(accessToken))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED_GENRATE_JWT_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }
                string refreshToken = _tokenService.GenerateRefreshToken();



                oExistAppUser.RefreshToken = refreshToken;
                oExistAppUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_appSettings.RefreshTokenExpirationDays);
                oExistAppUser.UpdatedBy = Convert.ToString(oExistAppUser.AppUserProfileId);
                oExistAppUser.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TokenResponse oTokenResponse = new() { access_token = accessToken, refresh_token = refreshToken };


                return new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResponse };

            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in AuthenticateUserAsync: {Ex.Message}");
                throw new Exception(ConstantSupplier.AUTH_EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This method is used to pull the application user profile and user menu with permissin details after successful authentication.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        /// <exception cref="Exception"></exception>
        public async Task<DataResponse> GetAppUserProfileMenuAsync(string userId)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_REQ_MSG, JsonConvert.SerializeObject(userId, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                if (Utilities.IsNullOrEmptyOrWhiteSpace(userId))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.APP_USER_PROFILE_MENU_INVALID_TOKEN_MSG, ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                AppUser? oExistAppUser = await _context.AppUsers!.Include(u => u.AppUserProfile).FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId) && u.IsActive == true);

                if (Utilities.IsNull(oExistAppUser))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.NotFound, null, ConstantSupplier.INVALID_USER_MSG, ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(x => x.Id == oExistAppUser!.AppUserProfileId && x.IsActive == true);

                if (Utilities.IsNull(oAppUserProfile))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG, ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);

                DataResponse oUserMenuResponse = await _roleMenuService.GetAllAppUserMenuByUserIdAsync(oAppUserProfile!.Id.ToString());
                if (!oUserMenuResponse.Success)
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED_RETRIEVE_MENU_MSG, ConstantSupplier.SERVICE_APP_USER_PROFILE_MENU_FAILED_MSG);


                ProfileMenuResponse oProfileMenuResponse = new()
                {
                    user = new User
                    {
                        Id = Convert.ToString(oAppUserProfile.Id),
                        FullName = oAppUserProfile.FullName,
                        UserName = oExistAppUser!.UserName,
                        Email = oAppUserProfile.Email,
                        UserRole = oAppUserProfile.AppUserRoleId.ToString(),
                        CreatedDate = oAppUserProfile.CreatedDate
                    },
                    userMenus = Convert.ToString(oUserMenuResponse.Result)
                };


                return new DataResponse { Success = true, Message = ConstantSupplier.APP_USER_PROFILE_MENU_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oProfileMenuResponse };

            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in GetAppUserProfileMenuAsync: {Ex.Message}");
                throw new Exception(ConstantSupplier.APP_USER_PROFILE_MENU_SERVICE_EXCEPTION_MSG);
            }
        }

        /// <summary>
        /// This method used to validate refresh token, renew a jwt token.
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        /// <exception cref="Exception"></exception>
        public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest? tokenRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(tokenRequest, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                AppUser? user = await _context.AppUsers!.FirstOrDefaultAsync(u => u.RefreshToken == tokenRequest!.Refresh_Token);
                if (Utilities.IsNull(user) || user!.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.Unauthorized, null, ConstantSupplier.REFRESHTOKEN_INVALID_EXPIRED_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(x => x.Id == user.AppUserProfileId && x.IsActive == true);

                if (Utilities.IsNull(oAppUserProfile))
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_USER_PROFILE_NOT_FOUND_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

                ClaimRequest oClaimRequest = new() { UserId = user.Id.ToString(), UserName = user.UserName, Email = oAppUserProfile!.Email, Role = oAppUserProfile.AppUserRoleId.ToString() };
                string accessToken = _tokenService.GenerateJwtToken(oClaimRequest);
                if (Utilities.IsNullOrEmptyOrWhiteSpace(accessToken))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED_GENRATE_JWT_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }
                string refreshToken = _tokenService.GenerateRefreshToken();



                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_appSettings!.RefreshTokenExpirationDays);
                user.UpdatedBy = Convert.ToString(user.AppUserProfileId);
                user.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TokenResponse oTokenResponse = new() { access_token = accessToken, refresh_token = refreshToken };

                return new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResponse };
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Service: Exception in RefreshTokenAsync: {Ex.Message}");
                throw new Exception(ConstantSupplier.REFRESHTOKEN_EXCEPTION_MSG);
            }
        }


        /// <summary>
        /// If renewing refresh token failed then it revokes the current user from loggedin  any longer.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        public async Task<DataResponse> RevokeAsync(string? userName)
        {
            DataResponse? oDataResponse;
            object? nullValue = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_REQ_MSG, JsonConvert.SerializeObject(userName, Formatting.Indented)));


            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.INVALID_USERNAME_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                var user = await _context.AppUsers!.FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive == true);

                if (Utilities.IsNull(user))
                {
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.NotFound, null,
                        ConstantSupplier.REVOKE_USER_NOT_FOUND_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }

                // Clear the refresh token and update
                user!.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                user.UpdatedBy = user.AppUserProfileId.ToString();
                user.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

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
        /// Send email regarding the user blocked after 3 times incorrect login attempt.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns><see cref="Task{SendResponse}"/></returns>
        private async Task<SendResponse> SendEmail(LoginRequest? request, AppUser? user)
        {
            SendResponse response;
            try
            {
                //string? userEmail = _context?.AppUserProfiles?.SingleOrDefault(u => u.Id == user.AppUserProfileId && u.IsActive == true)?.Email;

                string body = _emailService.PopulateBody("EmailTemplates/emailblocknotice.htm", user?.UserName,
                                        "User management", "https://localhost:4200/",
                                        "Please check your username and password. Please wait for mentioned time to re-login");
                string? email = _context?.AppUserProfiles?.Where(u => u.Id == user.AppUserProfileId && u.IsActive == true)?.SingleOrDefault()?.Email;
                if (!Utilities.IsNullOrEmpty(email))
                {
                    Message message = new() { To = email, Name = request?.UserName, Subject = "Email Blocked", Body = body };
                    response = await _emailService.SendEmailAsync(_appSettings.EmailConfiguration, message);
                }
                else
                {
                    return new() { ErrorMessages = new List<string> { "No valid email address found" }, MessageId = "" };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

    }
}
