﻿using Azure;
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
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SB.Security.Service
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

        #region AuthenticateUserAsync old
        /// <summary>
        /// <para>EF Codeblock: AuthenticateUserAsync</para> 
        /// This method authenticate user credential. It checks user name and then password. In between the checking, if client attempts consecutive 
        /// 3 failed request then this method will block the any further request for authentication of the user. Where, It update the datetime
        /// of the failed attempts and count of failed attempts. So threshold(appsettings.json) says after 3 failed attempts, user get blocked for the 
        /// next 1 min. This method ensures the unique username for all the user records.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataResponse</returns>
        //public async Task<DataResponse> AuthenticateUserAsync(LoginRequest request)
        //{
        //    DataResponse dataResponse;
        //    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));

        //    try
        //    {
        //        if (request != null)
        //        {

        //            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);
        //            if (user != null)
        //            {
        //                if (user.LoginFailedAttemptsCount > Convert.ToInt32(_configuration["AppSettings:MaxNumberOfFailedAttempts"])
        //                    && user.LastLoginAttemptAt.HasValue
        //                    && DateTime.Now < user.LastLoginAttemptAt.Value.AddMinutes(Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])))
        //                {

        //                    SendResponse emailResponse = await SendEmail(request, user);
        //                    if (!emailResponse.Successful)
        //                    {
        //                        _securityLogService.LogError(String.Format("{0}", JsonConvert.SerializeObject(emailResponse, Formatting.Indented)));
        //                    }

        //                    dataResponse = new DataResponse
        //                    {
        //                        Success = false,
        //                        Message = String.Format(ConstantSupplier.AUTH_FAILED_ATTEMPT, Convert.ToInt32(_configuration["AppSettings:BlockMinutes"])),
        //                        MessageType = Enum.EnumResponseType.Error,
        //                        ResponseCode = (int)HttpStatusCode.BadRequest,
        //                        Result = null
        //                    };
        //                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

        //                    return dataResponse;
        //                }

        //                bool verified = BCryptNet.Verify(request.Password, user.Password);
        //                if (verified)
        //                {
        //                    user.LoginFailedAttemptsCount = 0;
        //                    user.LastLoginAttemptAt = DateTime.Now;
        //                    await TrackAndUpdateLoginAttempts(user);
        //                    JwtSecurityToken token;
        //                    DateTime expires;
        //                    Token? tokenResult = _tokenService?.GenerateAccessToken(user);
        //                    if (tokenResult != null)
        //                    {
        //                        tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();

        //                    }

        //                    DataResponse menuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(user.Id.ToString());
        //                    if (menuResponse != null && menuResponse.ResponseCode == 200)
        //                    {
        //                        tokenResult.userMenus = Convert.ToString(menuResponse.Result);
        //                    }

        //                    UserLogin? userlogin = _context.UserLogin.FirstOrDefault(u => (u.UserName == user.UserName) && (u.Password == user.Password));
        //                    if (userlogin is null)
        //                    {
        //                        UserLogin oUserLogin = new()
        //                        {
        //                            Id = Guid.NewGuid(),
        //                            UserName = request.UserName,
        //                            Password = user.Password,
        //                            RefreshToken = tokenResult?.refresh_token,
        //                            RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
        //                        };
        //                        await _context.UserLogin.AddAsync(oUserLogin);
        //                        await _context.SaveChangesAsync();
        //                    }
        //                    else
        //                    {
        //                        userlogin.RefreshToken = tokenResult?.refresh_token;
        //                        userlogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        //                        await _context.SaveChangesAsync();
        //                    }

        //                    //return new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = TokenResult };

        //                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };

        //                    return dataResponse;
        //                }
        //                else
        //                {
        //                    user.LastLoginAttemptAt = DateTime.Now;
        //                    user.LoginFailedAttemptsCount++;
        //                    await TrackAndUpdateLoginAttempts(user);

        //                    //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };

        //                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

        //                    return dataResponse;
        //                }


        //            }

        //            //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //            dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));

        //            return dataResponse;

        //        }
        //        //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //        dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return dataResponse;
        //}
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
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            object? nullValue = null;
            try
            {
                if (Utilities.IsNotNull(request))
                {

                    AppUser? oExistAppUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == request.UserName && x.IsActive == true);
                    if (Utilities.IsNotNull(oExistAppUser))
                    {
                        AppUserProfile? oAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(x => x.Id == oExistAppUser.AppUserProfileId && x.IsActive == true);
                        if (Utilities.IsNotNull(oAppUserProfile))
                        {
                            bool isVarified = BCryptNet.Verify(request.Password, oExistAppUser.Password);
                            if (isVarified)
                            {
                                User oUser = new() { Id = Convert.ToString(oAppUserProfile.Id), FullName = oAppUserProfile.FullName, UserName = oExistAppUser.UserName, Email = oAppUserProfile.Email, UserRole = oAppUserProfile.AppUserRoleId.ToString(), CreatedDate = oAppUserProfile.CreatedDate };
                                Token? oTokenResult = _tokenService?.GenerateAccessToken(oUser);
                                if (Utilities.IsNotNull(oTokenResult))
                                {
                                    oExistAppUser.RefreshToken = oTokenResult?.refresh_token;
                                    oExistAppUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                                    dataResponse = await UpdateRefreshToken(oExistAppUser);
                                    if (Utilities.IsNotNull(dataResponse))
                                    {
                                        dataResponse = await _roleMenuService.GetAllAppUserMenuByUserIdAsync(oAppUserProfile.Id.ToString());
                                        if (Utilities.IsNotNull(dataResponse) && (dataResponse.ResponseCode == 200))
                                        {
                                            oTokenResult.userMenus = Convert.ToString(dataResponse.Result);
                                            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResult };
                                            return dataResponse;
                                        }
                                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, dataResponse, ConstantSupplier.AUTH_FAILED, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                                    }
                                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, dataResponse, ConstantSupplier.AUTH_FAILED, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                                }
                                return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                            }
                            return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_FAILED, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                        }
                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.INVALID_USER_MSG, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                    }

                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.AUTH_INVALID_CREDENTIAL, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);
                }
                return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.UnprocessableEntity, null, ConstantSupplier.AUTH_FAILED, ConstantSupplier.SERVICE_LOGIN_FAILED_MSG);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #region RefreshTokenAsync old
        /// <summary>
        /// This method used to validate refresh token, renew a jwt token.  
        /// </summary>
        /// <param name="refreshTokenReq"></param>
        /// <returns>DataResponse</returns>
        //public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenReq)
        //{
        //    DataResponse dataResponse;
        //    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
        //    try
        //    {
        //        if (refreshTokenReq != null)
        //        {
        //            string? accessToken = refreshTokenReq?.Access_Token;
        //            string? refreshToken = refreshTokenReq?.Refresh_Token;

        //            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        //            string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value;

        //            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == username && u.IsActive == true);

        //            Token? tokenResult = _tokenService?.GenerateAccessToken(user);
        //            if (tokenResult != null)
        //            {
        //                tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();
        //            }

        //            DataResponse menuResponse = await _roleMenuService.GetAllMenuByUserIdAsync(user.Id.ToString());
        //            if (menuResponse != null && menuResponse.ResponseCode == 200)
        //            {
        //                tokenResult.userMenus = Convert.ToString(menuResponse.Result);
        //            }

        //            var userLogin = _context.UserLogin.SingleOrDefault(u => u.UserName == username);

        //            if (userLogin is null || userLogin.RefreshToken != refreshToken || userLogin.RefreshTokenExpiryTime <= DateTime.Now)
        //            {
        //                _securityLogService.LogError(String.Format(ConstantSupplier.INVALID_CLIENT_REQUEST, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
        //                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.INVALID_CLIENT_REQUEST, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //                return dataResponse;
        //            }

        //            userLogin.RefreshToken = tokenResult?.refresh_token;
        //            userLogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        //            await _context.SaveChangesAsync();

        //            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };

        //            return dataResponse;
        //        }
        //        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
        //        dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return dataResponse;
        //}
        #endregion

        /// <summary>
        /// This method used to validate refresh token, renew a jwt token.  
        /// </summary>
        /// <param name="refreshTokenReq"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest? refreshTokenReq)
        {
            DataResponse oDataResponse;
            object? nullValue = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
            try
            {
                if (Utilities.IsNotNull(refreshTokenReq))
                {
                    string? accessToken = refreshTokenReq?.Access_Token;
                    string? refreshToken = refreshTokenReq?.Refresh_Token;

                    ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                    string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value;

                    AppUser? oLoginUser = _context.AppUsers.SingleOrDefault(u => u.UserName == username && u.IsActive == true);

                    if (Utilities.IsNull(oLoginUser) || oLoginUser.RefreshToken != refreshToken || oLoginUser.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    {
                        _securityLogService.LogError(String.Format(ConstantSupplier.INVALID_CLIENT_REQUEST, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
                        oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.INVALID_CLIENT_REQUEST, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        return oDataResponse;
                    }


                    User? oUser = await _context.AppUsers
                                            .Join(_context.AppUserProfiles,
                                                loginUser => loginUser.AppUserProfileId,
                                                userProfile => userProfile.Id,
                                                (loginUser, userProfile) => new { loginUser, userProfile }
                                            )
                                            .Where(x => x.loginUser.UserName == username && x.loginUser.IsActive == true && x.userProfile.IsActive == true)
                                            .Select(x => new User
                                            {
                                                Id = Convert.ToString(x.userProfile.Id),
                                                FullName = x.userProfile.FullName,
                                                UserName = x.loginUser.UserName,
                                                Email = x.userProfile.Email,
                                                UserRole = x.userProfile.AppUserRoleId.ToString(),
                                                CreatedDate = x.userProfile.CreatedDate
                                            }).FirstOrDefaultAsync();

                    Token? oTokenResult = _tokenService?.GenerateAccessToken(oUser);
                    if (Utilities.IsNotNull(oTokenResult))
                    {
                        oLoginUser.RefreshToken = oTokenResult?.refresh_token;
                        oLoginUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                        oDataResponse = await UpdateRefreshToken(oLoginUser);
                        if (Utilities.IsNotNull(oDataResponse))
                        {
                            oDataResponse = await _roleMenuService.GetAllAppUserMenuByUserIdAsync(oLoginUser.AppUserProfileId.ToString());
                            if (Utilities.IsNotNull(oDataResponse) && (oDataResponse.ResponseCode == 200))
                            {
                                oTokenResult.userMenus = Convert.ToString(oDataResponse.Result);
                                oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oTokenResult };
                                return oDataResponse;
                            }
                            return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, oDataResponse, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG);
                        }
                        return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, oDataResponse, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG);
                    }
                    return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.BadRequest, null, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG);
                }
                return Utilities.FailedResponse(nullValue, _securityLogService, (int)HttpStatusCode.UnprocessableEntity, null, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region RevokeAsync old
        ///// <summary>
        ///// If renewing refresh token failed then it revokes the current user from loggedin  any longer.
        ///// </summary>
        ///// <param name="userToken"></param>
        ///// <returns>DataResponse</returns>
        //public async Task<DataResponse> RevokeAsync(string userToken)
        //{
        //    DataResponse dataResponse;
        //    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_REQ_MSG, JsonConvert.SerializeObject(userToken, Formatting.Indented)));
        //    try
        //    {
        //        if (userToken != null)
        //        {
        //            ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(userToken);
        //            string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value; //this is mapped to the Name claim by default
        //            UserLogin? oUserLogin = _context?.UserLogin.SingleOrDefault(u => u.UserName == username);
        //            if (oUserLogin == null)
        //            {
        //                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
        //                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //                return dataResponse;
        //            }
        //            oUserLogin.RefreshToken = null;
        //            await _context.SaveChangesAsync();
        //            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SESSION_EXPIRATION_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.REVOKE_USER_SUCCESS };
        //            return dataResponse;
        //        }
        //        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
        //        dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return dataResponse;
        //}
        #endregion

        /// <summary>
        /// If renewing refresh token failed then it revokes the current user from loggedin  any longer.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns><see cref="Task{DataResponse}"/></returns>
        public async Task<DataResponse> RevokeAsync(string? userToken)
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_REQ_MSG, JsonConvert.SerializeObject(userToken, Formatting.Indented)));
            try
            {
                if (Utilities.IsNotNull(userToken))
                {
                    ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(userToken);
                    string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value; 
                    AppUser? oLoginUser = _context?.AppUsers.SingleOrDefault(u => u.UserName == username && u.IsActive == true);
                    if (Utilities.IsNull(oLoginUser))
                    {
                        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                        oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        return oDataResponse;
                    }

                    oLoginUser.RefreshToken = null;
                    oDataResponse = await UpdateRefreshToken(oLoginUser);
                    if (oDataResponse.Success)
                    {
                        oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SESSION_EXPIRATION_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.REVOKE_USER_SUCCESS };
                        return oDataResponse;
                    }
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    return oDataResponse;
                    
                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }

        #region TrackAndUpdateLoginAttempts old
        ///// <summary>
        ///// It update the "LastLoginAttemptAt" and "LoginFailedAttemptsCount" database table columns.
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //private async Task TrackAndUpdateLoginAttempts(UserInfo? user)
        //{
        //    try
        //    {
        //        var dbUserInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == user.Id);
        //        dbUserInfo.LastLoginAttemptAt = user.LastLoginAttemptAt;
        //        dbUserInfo.LoginFailedAttemptsCount = user.LoginFailedAttemptsCount;
        //        var isLastLoginAttemptAtModified = _context.Entry(dbUserInfo).Property("LastLoginAttemptAt").IsModified;
        //        var isLoginFailedAttemptsCountModified = _context.Entry(dbUserInfo).Property("LoginFailedAttemptsCount").IsModified;
        //        _context.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

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
                    return new() { ErrorMessages = new List<string> { "No valid email address found" }, MessageId="" };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        /// <summary>
        /// This method is used to update the refresh token and expiry token date field in application user table.
        /// </summary>
        /// <param name="oAppUser"></param>
        /// <returns><see cref="Task{SendResponse}"/></returns>
        private async Task<DataResponse> UpdateRefreshToken(AppUser? oAppUser)
        {
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                AppUser? oExistAppUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.AppUserProfileId == oAppUser.AppUserProfileId && x.IsActive == true);
                if (Utilities.IsNotNull(oExistAppUser))
                {
                    oExistAppUser.RefreshToken = oAppUser.RefreshToken;
                    oExistAppUser.RefreshTokenExpiryTime = oAppUser.RefreshTokenExpiryTime;
                    oExistAppUser.UpdatedBy = Convert.ToString(oAppUser.AppUserProfileId);
                    oExistAppUser.UpdatedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();

                    return new DataResponse { Success = true, Message = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_SUCCESS_MSG };

                }
                return new DataResponse { Success = false, Message = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_FAILED_MSG };

            }
            catch (Exception)
            {
                oTrasaction.Rollback();
                return new DataResponse { Success = false, Message = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.InternalServerError, Result = ConstantSupplier.APP_USER_REFRESH_TOKEN_DURING_LOGIN_FAILED_MSG };
            }

        }

    }
}
