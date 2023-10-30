using Azure;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SB.DataAccessLayer;
using SB.EmailService.Models;
using SB.EmailService.Service;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SB.Security.Service
{
    public class AuthService : IAuthService
    {
        #region Variable declaration & constructor initialization

        public IConfiguration _configuration;
        //private readonly SBSecurityDBContext _context;
        private readonly SecurityDBContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly IDatabaseManager _dbmanager;
        private readonly ITokenService _tokenService;
        private readonly IRoleMenuService _roleMenuService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
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
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> AuthenticateUserAsync(LoginRequest request)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));

            try
            {
                if (request != null)
                {

                    UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);
                    if (user != null)
                    {
                        if (user.LoginFailedAttemptsCount > Convert.ToInt32(_configuration["AppSettings:MaxNumberOfFailedAttempts"])
                            && user.LastLoginAttemptAt.HasValue
                            && DateTime.Now < user.LastLoginAttemptAt.Value.AddMinutes(Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])))
                        {

                            SendResponse emailResponse = await SendEmail(request, user);
                            if (!emailResponse.Successful)
                            {
                                _securityLogService.LogError(String.Format("{0}", JsonConvert.SerializeObject(emailResponse, Formatting.Indented)));
                            }

                            dataResponse = new DataResponse
                            {
                                Success = false,
                                Message = String.Format(ConstantSupplier.AUTH_FAILED_ATTEMPT, Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])),
                                MessageType = Enum.EnumResponseType.Error,
                                ResponseCode = (int)HttpStatusCode.BadRequest,
                                Result = null
                            };
                            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                            return dataResponse;
                        }

                        bool verified = BCryptNet.Verify(request.Password, user.Password);
                        if (verified)
                        {
                            user.LoginFailedAttemptsCount = 0;
                            user.LastLoginAttemptAt = DateTime.Now;
                            await TrackAndUpdateLoginAttempts(user);
                            JwtSecurityToken token;
                            DateTime expires;
                            Token? tokenResult = _tokenService?.GenerateAccessToken(user);
                            if (tokenResult != null)
                            {
                                tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();

                            }

                            DataResponse menuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(user.Id.ToString());
                            if (menuResponse != null && menuResponse.ResponseCode == 200)
                            {
                                tokenResult.userMenus = Convert.ToString(menuResponse.Result);
                            }

                            UserLogin? userlogin = _context.UserLogin.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == user.Password));
                            if (userlogin is null)
                            {
                                UserLogin oUserLogin = new()
                                {
                                    Id = Guid.NewGuid(),
                                    UserName = request.UserName,
                                    Password = user.Password,
                                    RefreshToken = tokenResult?.refresh_token,
                                    RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
                                };
                                await _context.UserLogin.AddAsync(oUserLogin);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                userlogin.RefreshToken = tokenResult?.refresh_token;
                                userlogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                                await _context.SaveChangesAsync();
                            }

                            //return new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = TokenResult };

                            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };

                            return dataResponse;
                        }
                        else
                        {
                            user.LastLoginAttemptAt = DateTime.Now;
                            user.LoginFailedAttemptsCount++;
                            await TrackAndUpdateLoginAttempts(user);

                            //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };

                            dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                            return dataResponse;
                        }


                    }

                    //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                    return dataResponse;

                }
                //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }


        public async Task<DataResponse> AltAuthenticateUserAsync(LoginRequest request)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));

            try
            {
                if (request != null)
                {

                    AppUserProfile? oAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);
                    
                    if (oAppUserProfile != null)
                    {
                        bool isVarified = BCryptNet.Verify(request.Password, oAppUserProfile.Password);
                        if (isVarified)
                        {
                            AppLoggedInUser? oExistAppLoggedInUser = await _context.AppLoggedInUsers.FirstOrDefaultAsync(x => x.AppUserProfileId == oAppUserProfile.Id && x.IsActive == true);
                            if (oExistAppLoggedInUser != null)
                            {
                                
                                Token? oTokenResult = _tokenService?.GenerateAccessToken(oAppUserProfile);
                                if (oTokenResult != null)
                                {
                                    oTokenResult.refresh_token = _tokenService?.GenerateRefreshToken();
                                }

                                DataResponse? oMenuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(oAppUserProfile.Id.ToString());
                                if (oMenuResponse != null && oMenuResponse.ResponseCode == 200)
                                {
                                    oTokenResult.userMenus = Convert.ToString(oMenuResponse.Result);
                                }
                                
                                
                                oExistAppLoggedInUser.RefreshToken = oTokenResult?.refresh_token;
                                oExistAppLoggedInUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                                oExistAppLoggedInUser.LastLoginAttemptAt = DateTime.UtcNow;
                                oExistAppLoggedInUser.LoginFailedAttemptsCount = 0;
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            AppLoggedInUser? oExistAppLoggedInUser = await _context.AppLoggedInUsers.FirstOrDefaultAsync(x => x.AppUserProfileId == oAppUserProfile.Id && x.IsActive == true);
                            if(oExistAppLoggedInUser != null)
                            {
                                oAppLoggedInUser.LastLoginAttemptAt = DateTime.UtcNow;
                                oAppLoggedInUser.LoginFailedAttemptsCount++;
                            }
                            
                        }
                        
                        if (loggedInUser != null)
                        {
                            if (loggedInUser.LoginFailedAttemptsCount > Convert.ToInt32(_configuration["AppSettings:MaxNumberOfFailedAttempts"])
                            && loggedInUser.LastLoginAttemptAt.HasValue
                            && DateTime.UtcNow < loggedInUser.LastLoginAttemptAt.Value.AddMinutes(Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])))
                            {

                                SendResponse emailResponse = await SendEmail(request, user);
                                if (!emailResponse.Successful)
                                {
                                    _securityLogService.LogError(String.Format("{0}", JsonConvert.SerializeObject(emailResponse, Formatting.Indented)));
                                }

                                dataResponse = new DataResponse
                                {
                                    Success = false,
                                    Message = String.Format(ConstantSupplier.AUTH_FAILED_ATTEMPT, Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])),
                                    MessageType = Enum.EnumResponseType.Error,
                                    ResponseCode = (int)HttpStatusCode.BadRequest,
                                    Result = null
                                };
                                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                                return dataResponse;
                            }
                            bool verified = BCryptNet.Verify(request.Password, user.Password);
                            if (verified)
                            {
                                user.LoginFailedAttemptsCount = 0;
                                user.LastLoginAttemptAt = DateTime.Now;
                                await TrackAndUpdateLoginAttempts(user);
                                JwtSecurityToken token;
                                DateTime expires;
                                Token? tokenResult = _tokenService?.GenerateAccessToken(user);
                                if (tokenResult != null)
                                {
                                    tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();

                                }

                                DataResponse menuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(user.Id.ToString());
                                if (menuResponse != null && menuResponse.ResponseCode == 200)
                                {
                                    tokenResult.userMenus = Convert.ToString(menuResponse.Result);
                                }

                                UserLogin? userlogin = _context.UserLogin.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == user.Password));
                                if (userlogin is null)
                                {
                                    UserLogin oUserLogin = new()
                                    {
                                        Id = Guid.NewGuid(),
                                        UserName = request.UserName,
                                        Password = user.Password,
                                        RefreshToken = tokenResult?.refresh_token,
                                        RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
                                    };
                                    await _context.UserLogin.AddAsync(oUserLogin);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    userlogin.RefreshToken = tokenResult?.refresh_token;
                                    userlogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                                    await _context.SaveChangesAsync();
                                }

                                //return new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = TokenResult };

                                dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };

                                return dataResponse;
                            }
                            else
                            {
                                user.LastLoginAttemptAt = DateTime.Now;
                                user.LoginFailedAttemptsCount++;
                                await TrackAndUpdateLoginAttempts(user);

                                //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };

                                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                                return dataResponse;
                            }
                        }
                        else
                        {

                        }


                    }

                    //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

                    return dataResponse;

                }
                else
                {

                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));
                }

            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }

        /// <summary>
        /// This method used to validate refresh token, renew a jwt token.  
        /// </summary>
        /// <param name="refreshTokenReq"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenReq)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
            try
            {
                if (refreshTokenReq != null)
                {
                    string? accessToken = refreshTokenReq?.Access_Token;
                    string? refreshToken = refreshTokenReq?.Refresh_Token;

                    ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                    //string? username = principal?.Identity?.Name; //this is mapped to the Name claim by default
                    //principal.Claims.ToList()[5].Value
                    string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value;

                    var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == username && u.IsActive == true);

                    Token? tokenResult = _tokenService?.GenerateAccessToken(user);
                    if (tokenResult != null)
                    {
                        tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();
                    }

                    DataResponse menuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(user.Id.ToString());
                    if (menuResponse != null && menuResponse.ResponseCode == 200)
                    {
                        tokenResult.userMenus = Convert.ToString(menuResponse.Result);
                    }

                    var userLogin = _context.UserLogin.SingleOrDefault(u => u.UserName == username);

                    if (userLogin is null || userLogin.RefreshToken != refreshToken || userLogin.RefreshTokenExpiryTime <= DateTime.Now)
                    {
                        _securityLogService.LogError(String.Format(ConstantSupplier.INVALID_CLIENT_REQUEST, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
                        dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.INVALID_CLIENT_REQUEST, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        return dataResponse;
                    }

                    userLogin.RefreshToken = tokenResult?.refresh_token;
                    userLogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                    await _context.SaveChangesAsync();

                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };

                    return dataResponse;
                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }

        /// <summary>
        /// If renewing refresh token failed then it revokes the current user from loggedin  any longer.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> RevokeAsync(string userToken)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_REQ_MSG, JsonConvert.SerializeObject(userToken, Formatting.Indented)));
            try
            {
                if (userToken != null)
                {
                    ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(userToken);
                    string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value; //this is mapped to the Name claim by default
                    UserLogin? oUserLogin = _context?.UserLogin.SingleOrDefault(u => u.UserName == username);
                    if (oUserLogin == null)
                    {
                        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                        dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        return dataResponse;
                    }
                    oUserLogin.RefreshToken = null;
                    await _context.SaveChangesAsync();
                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SESSION_EXPIRATION_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.REVOKE_USER_SUCCESS };
                    return dataResponse;
                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }

        /// <summary>
        /// It update the "LastLoginAttemptAt" and "LoginFailedAttemptsCount" database table columns.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task TrackAndUpdateLoginAttempts(UserInfo? user)
        {
            try
            {
                var dbUserInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == user.Id);
                dbUserInfo.LastLoginAttemptAt = user.LastLoginAttemptAt;
                dbUserInfo.LoginFailedAttemptsCount = user.LoginFailedAttemptsCount;
                var isLastLoginAttemptAtModified = _context.Entry(dbUserInfo).Property("LastLoginAttemptAt").IsModified;
                var isLoginFailedAttemptsCountModified = _context.Entry(dbUserInfo).Property("LoginFailedAttemptsCount").IsModified;
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Send email regarding the user blocked after 3 times incorrect login attempt.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<SendResponse> SendEmail(LoginRequest request, UserInfo? user)
        {
            SendResponse response;
            try
            {
                string body = _emailService.PopulateBody("EmailTemplates/emailblocknotice.htm", user.UserName,
                                        "User management", "https://localhost:4200/",
                                        "Please check your username and password. Please wait for mentioned time to re-login");
                Message message = new() { To = user.Email, Name = request.UserName, Subject = "Email Blocked", Body = body };
                response = await _emailService.SendEmailAsync(_appSettings.EmailConfiguration, message);

            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        private async Task<DataResponse> UpdateAppUserLoginAttempt(AppLoggedInUser oAppLoggedInUser)
        {
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                AppLoggedInUser? oExistAppLoggedInUser = await _context.AppLoggedInUsers.FirstOrDefaultAsync(x => x.Id == oAppLoggedInUser.Id && x.AppUserProfileId == oAppLoggedInUser.AppUserProfileId && x.IsActive == true);
                if (oExistAppLoggedInUser != null)
                {
                    oExistAppLoggedInUser.AppUserProfileId = oAppLoggedInUser.AppUserProfileId;
                    oExistAppLoggedInUser.RefreshToken = oAppLoggedInUser.RefreshToken;
                    oExistAppLoggedInUser.RefreshTokenExpiryTime = oAppLoggedInUser.RefreshTokenExpiryTime;
                    oExistAppLoggedInUser.LastLoginAttemptAt = oAppLoggedInUser.LastLoginAttemptAt;
                    oExistAppLoggedInUser.LoginFailedAttemptsCount = oAppLoggedInUser.LoginFailedAttemptsCount;
                    oExistAppLoggedInUser.IsActive = oAppLoggedInUser.IsActive;
                    _context.Entry(oExistAppLoggedInUser).Property("AppUserProfileId").IsModified = true;
                    _context.Entry(oExistAppLoggedInUser).Property("RefreshToken").IsModified = true;
                    _context.Entry(oExistAppLoggedInUser).Property("RefreshTokenExpiryTime").IsModified = true;
                    _context.Entry(oExistAppLoggedInUser).Property("LastLoginAttemptAt").IsModified = true;
                    _context.Entry(oExistAppLoggedInUser).Property("LoginFailedAttemptsCount").IsModified = true;
                    _context.Entry(oExistAppLoggedInUser).Property("IsActive").IsModified = true;
                    await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();

                    return new DataResponse { Success = true, Message = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_SUCCESS_MSG };

                }
                else
                {
                    AppLoggedInUser oNewAppLoggedInUser = new()
                    {
                        Id = Guid.NewGuid(),
                        AppUserProfileId = oAppLoggedInUser.AppUserProfileId,
                        RefreshToken = oAppLoggedInUser.RefreshToken,
                        RefreshTokenExpiryTime = oAppLoggedInUser.RefreshTokenExpiryTime,
                        LastLoginAttemptAt = oAppLoggedInUser.LastLoginAttemptAt,
                        LoginFailedAttemptsCount = oAppLoggedInUser.LoginFailedAttemptsCount,
                        IsActive = oAppLoggedInUser.IsActive
                    };
                    await _context.AppLoggedInUsers.AddAsync(oNewAppLoggedInUser);
                    await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();
                }
                return new DataResponse { Success = false, Message = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_FAILED_MSG };
            }
            catch (Exception)
            {
                oTrasaction.Rollback();
                return new DataResponse { Success = false, Message = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_FAILED_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.InternalServerError, Result = ConstantSupplier.UPDATE_LOGGEDINUSER_LOGIN_ATTEMPT_FAILED_MSG };
            }
        }
    }
}
