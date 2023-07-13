using Azure;
using Azure.Core;
using FluentEmail.Core.Models;
using SB.EmailService;
using SB.EmailService.Models;
using SB.EmailService.Service;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Drawing.Printing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;
using SB.DataAccessLayer;
using System.Data;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SB.Security.Service
{
    /// <summary>
    /// includes all the methods for user operation incuding the user login. It implements  <see cref="IUserService"/>.
    /// </summary>
    public class UserService : IUserService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        private readonly SBSecurityDBContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly IDatabaseManager _dbmanager;
        private readonly ITokenService _tokenService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        public UserService(IConfiguration config, SBSecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager, ITokenService tokenService)
        {
            _configuration = config;
            _context = context;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
            _tokenService = tokenService;
        }
        #endregion

        #region All service methods

        /// <summary>
        /// <para>EF Codeblock: GetUserByIdAsync</para> 
        /// This service method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAsync(string id)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            UserInfo? user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == new Guid(id) && u.IsActive == true);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(user, Formatting.Indented)));
            return user != null
                ? new DataResponse { Success = true, Message = ConstantSupplier.GET_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = user }
                : new DataResponse { Success = false, Message = ConstantSupplier.GET_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetUserByIdAdoAsync</para> 
        /// <para>This service method used to get a specific user details by supplying user id.</para> 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAdoAsync(string id)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            UserInfo? user;
            List<IDbDataParameter> parameters = new()
            {
                _dbmanager.CreateParameter("@Id", new Guid(id), DbType.Guid)
            };
            DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_USER_BY_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(oDT, Formatting.Indented)));
            if (oDT != null && oDT.Rows.Count > 0)
            {
                user = JArray.FromObject(oDT)[0].ToObject<UserInfo>();
                return new DataResponse { Success = true, Message = ConstantSupplier.GET_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = user };
            }
            return new DataResponse { Success = false, Message = ConstantSupplier.GET_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        }

        /// <summary>
        /// <para>EF Codeblock: GetAllUserAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PageResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PageResult<UserInfo>> GetAllUserAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            int count = await _context.UserInfo.CountAsync();
            List<UserInfo> Items = await _context.UserInfo.OrderByDescending(x => x.CreatedDate).Skip((paramRequest.PageNumber - 1) * paramRequest.PageSize).Take(paramRequest.PageSize).ToListAsync();
            PageResult<UserInfo> result = new PageResult<UserInfo>
            {
                Count = count,
                PageIndex = paramRequest.PageNumber > 0 ? paramRequest.PageNumber : 1,
                PageSize = 10,
                Items = Items
            };
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
            return result;


        }

        /// <summary>
        /// <para>EF Codeblock: GetAllUserExtnAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PagingResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<UserInfo>> GetAllUserExtnAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            //var source = _context.UserInfos.OrderBy(a=>a.CreatedDate).AsQueryable();
            IQueryable<UserInfo> source = (from user in _context?.UserInfo?.OrderBy(a => a.CreatedDate) select user).AsQueryable();
            PagingResult<UserInfo> result = await Utilities.GetPagingResult(source, paramRequest.PageNumber, paramRequest.PageSize);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
            return result;
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetAllUserAdoAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<UserInfo>?> GetAllUserAdoAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            List<UserInfo> oUserList;
            List<IDbDataParameter> parameters = new()
            {
                _dbmanager.CreateParameter("@PageIndex", paramRequest.PageNumber, DbType.Int32),
                _dbmanager.CreateParameter("@PageSize", paramRequest.PageSize, DbType.Int32)
            };
            DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_ALL_USER_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

            if (oDT != null && oDT.Rows.Count > 0)
            {
                oUserList = Utilities.ConvertDataTable<UserInfo>(oDT);

                //return Utilities.GetPagingResult(oUserList, paramRequest.PageNumber, paramRequest.PageSize);
                PagingResult<UserInfo> result = Utilities.GetPagingResult(oUserList, paramRequest.PageNumber, paramRequest.PageSize);
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                return result;
            }
            return null;

        }

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
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            if (request != null)
            {

                var user = await this._context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive == true);
                if (user != null)
                {
                    if (user.LoginFailedAttemptsCount > Convert.ToInt32(this._configuration["AppSettings:MaxNumberOfFailedAttempts"])
                        && user.LastLoginAttemptAt.HasValue
                        && DateTime.Now < user.LastLoginAttemptAt.Value.AddMinutes(Convert.ToInt32(this._configuration["AppSettings:BlockMinutes"])))
                    {

                        SendResponse emailResponse = await SendEmail(request, user);
                        if (!emailResponse.Successful)
                        {
                            this._securityLogService.LogError(String.Format("{0}", JsonConvert.SerializeObject(emailResponse, Formatting.Indented)));
                        }

                        var oDataResponse = new DataResponse
                        {
                            Success = false,
                            Message = String.Format(ConstantSupplier.AUTH_FAILED_ATTEMPT, Convert.ToInt32(this._configuration["AppSettings:BlockMinutes"])),
                            MessageType = Enum.EnumResponseType.Error,
                            ResponseCode = (int)HttpStatusCode.BadRequest,
                            Result = null
                        };
                        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));

                        return oDataResponse;
                    }

                    bool verified = BCryptNet.Verify(request.Password, user.Password);
                    if (verified)
                    {
                        user.LoginFailedAttemptsCount = 0;
                        user.LastLoginAttemptAt = DateTime.Now;
                        await TrackAndUpdateLoginAttempts(user);
                        JwtSecurityToken token;
                        DateTime expires;
                        //var TokenResult = GetToken(user);
                        Token? tokenResult = _tokenService?.GenerateAccessToken(user);
                        if (tokenResult != null)
                        {
                            tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();
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

                        var oDataResponse1 = new DataResponse { Success = true, Message = ConstantSupplier.AUTH_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };
                        _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_LOGIN_RES_MSG, JsonConvert.SerializeObject(oDataResponse1, Formatting.Indented)));

                        return oDataResponse1;
                    }
                    else
                    {
                        user.LastLoginAttemptAt = DateTime.Now;
                        user.LoginFailedAttemptsCount++;
                        await TrackAndUpdateLoginAttempts(user);

                        //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };

                        var oDataResponse2 = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(oDataResponse2, Formatting.Indented)));

                        return oDataResponse2;
                    }


                }

                //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                var oDataResponse3 = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(oDataResponse3, Formatting.Indented)));

                return oDataResponse3;

            }
            //return new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            var oDataResponse4 = new DataResponse { Success = false, Message = ConstantSupplier.AUTH_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_LOGIN_FAILED_MSG, JsonConvert.SerializeObject(oDataResponse4, Formatting.Indented)));

            return oDataResponse4;
        }

        public async Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenReq)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
            if (refreshTokenReq != null)
            {
                string? accessToken = refreshTokenReq?.Access_Token;
                string? refreshToken = refreshTokenReq?.Refresh_Token;

                ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                //string? username = principal?.Identity?.Name; //this is mapped to the Name claim by default
                //principal.Claims.ToList()[5].Value
                string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value;

                var user = await this._context.UserInfo.FirstOrDefaultAsync(u => u.UserName == username && u.IsActive == true);

                Token? tokenResult = _tokenService?.GenerateAccessToken(user);
                if (tokenResult != null)
                {
                    tokenResult.refresh_token = _tokenService?.GenerateRefreshToken();
                }

                var userLogin = _context.UserLogin.SingleOrDefault(u => u.UserName == username);

                if (userLogin is null || userLogin.RefreshToken != refreshToken || userLogin.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.INVALID_CLIENT_REQUEST, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
                    return new DataResponse { Success = false, Message = ConstantSupplier.INVALID_CLIENT_REQUEST, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                }

                userLogin.RefreshToken = tokenResult?.refresh_token;
                userLogin.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _context.SaveChangesAsync();

                DataResponse oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REFRESHTOKEN_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = tokenResult };
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));

                return oDataResponse;
            }
            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
            return new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REFRESHTOKEN_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        }

        public async Task<DataResponse> RevokeAsync(string userToken)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_REQ_MSG, JsonConvert.SerializeObject(userToken, Formatting.Indented)));
            if (userToken != null)
            {
                ClaimsPrincipal principal = _tokenService.GetPrincipalFromExpiredToken(userToken);
                string? username = principal?.Claims?.Where(x => x.Type == "UserName")?.FirstOrDefault()?.Value; //this is mapped to the Name claim by default
                UserLogin? oUserLogin = _context?.UserLogin.SingleOrDefault(u => u.UserName == username);
                if (oUserLogin == null)
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                    return new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                }
                oUserLogin.RefreshToken = null;
                await _context.SaveChangesAsync();
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_REVOKE_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REVOKE_USER_SUCCESS, Formatting.Indented)));
                return new DataResponse { Success = true, Message = ConstantSupplier.SESSION_EXPIRATION_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = ConstantSupplier.REVOKE_USER_SUCCESS };
            }
            _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
            return new DataResponse { Success = false, Message = ConstantSupplier.SERVICE_REVOKE_FAILED_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        }

        /// <summary>
        /// <para>EF & ADO.NET Codeblocks: RegisterUserAsync</para>
        /// This method saves and update the user details. It tracks the action name (save or update). Based on this it send the request for saving or
        /// updating the user credential. In Update method, no user password can be updated by Admin due to data protection policy in general. Password 
        /// is being encrypted using the Bcrypt during the registration.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> RegisterUserAsync(UserRegisterRequest request)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            if (request != null)
            {


                switch (request.ActionName)
                {
                    case ConstantSupplier.SAVE_KEY:
                        string saltKey = BCryptNet.GenerateSalt(13);
                        UserInfo oSaveUserInfo = new()
                        {
                            Id = Guid.NewGuid(),
                            FullName = request.FullName,
                            UserName = request.UserName,
                            Password = BCryptNet.HashPassword(request.Password, saltKey),
                            SaltKey = saltKey,
                            Email = request.Email,
                            UserRole = request.UserRole,
                            CreatedBy = Convert.ToString(this._context.UserInfo.FirstOrDefault(s => s.UserRole.Equals(ConstantSupplier.ADMIN)).Id),
                            CreatedDate = DateTime.UtcNow,
                            IsActive = request.IsActive
                        };

                        var user = await this._context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName);
                        if (user != null && !String.IsNullOrEmpty(Convert.ToString(user.Id)))
                        {
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(user, Formatting.Indented)));
                            return new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                        }

                        #region EF Codeblock of saving data
                        await this._context.UserInfo.AddAsync(oSaveUserInfo);
                        await this._context.SaveChangesAsync();

                        request.Id = Convert.ToString(oSaveUserInfo.Id);
                        _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
                        return new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                        #endregion

                        #region ADO.NET Codeblock of saving data
                    //List<IDbDataParameter> parameters = new()
                    //{
                    //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.SAVE_KEY, DbType.String),
                    //    _dbmanager.CreateParameter("@Id", oSaveUserInfo.Id, DbType.Guid),
                    //    _dbmanager.CreateParameter("@FullName", oSaveUserInfo.FullName, DbType.String),
                    //    _dbmanager.CreateParameter("@UserName", oSaveUserInfo.UserName, DbType.String),
                    //    _dbmanager.CreateParameter("@Password", oSaveUserInfo.Password, DbType.String),
                    //    _dbmanager.CreateParameter("@SaltKey", oSaveUserInfo.SaltKey, DbType.String),
                    //    _dbmanager.CreateParameter("@Email", oSaveUserInfo.Email, DbType.String),
                    //    _dbmanager.CreateParameter("@UserRole", oSaveUserInfo.UserRole, DbType.String),
                    //    _dbmanager.CreateParameter("@CreatedBy", oSaveUserInfo.CreatedBy, DbType.String),
                    //    _dbmanager.CreateParameter("@CreatedDate", oSaveUserInfo.CreatedDate, DbType.DateTime),
                    //    _dbmanager.CreateParameter("@UpdatedBy", DBNull.Value, DbType.String),
                    //    _dbmanager.CreateParameter("@UpdatedDate", DBNull.Value, DbType.DateTime),
                    //    _dbmanager.CreateParameter("@IsActive", oSaveUserInfo.IsActive, DbType.Boolean)
                    //};

                    //int isSave = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                    //request.Id = Convert.ToString(oSaveUserInfo.Id);
                    //_securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
                    //return isSave > 0
                    //? new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request }
                    //: new DataResponse { Success = false, Message = ConstantSupplier.REG_USER_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    #endregion

                    case ConstantSupplier.UPDATE_KEY:

                        var oldUser = await this._context.UserInfo.FirstOrDefaultAsync(u => u.UserName == (request.UserName));

                        if ((oldUser != null) && (oldUser.Id != new Guid(request.Id)))
                        {
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(oldUser, Formatting.Indented)));
                            return new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                        }


                        var dbUserInfo = this._context.UserInfo.FirstOrDefault(s => s.Id.Equals(new Guid(request.Id)));
                        dbUserInfo.FullName = request.FullName;
                        dbUserInfo.UserName = request.UserName;
                        dbUserInfo.Email = request.Email;
                        dbUserInfo.UserRole = request.UserRole;
                        dbUserInfo.UpdatedBy = Convert.ToString(this._context.UserInfo.FirstOrDefault(s => s.UserRole.Equals(ConstantSupplier.ADMIN)).Id);
                        dbUserInfo.UpdatedDate = DateTime.UtcNow;
                        dbUserInfo.IsActive = request.IsActive;

                        #region EF Codeblock of updating data
                        var isFullNameModified = this._context.Entry(dbUserInfo).Property("FullName").IsModified;
                        var isUserNameModified = this._context.Entry(dbUserInfo).Property("UserName").IsModified;
                        var isEmailModified = this._context.Entry(dbUserInfo).Property("Email").IsModified;
                        var isUserRoleModified = this._context.Entry(dbUserInfo).Property("UserRole").IsModified;
                        var isUpdatedByModified = this._context.Entry(dbUserInfo).Property("UpdatedBy").IsModified;
                        var isUpdatedDateModified = this._context.Entry(dbUserInfo).Property("UpdatedDate").IsModified;
                        var isIsActive = this._context.Entry(dbUserInfo).Property("IsActive").IsModified;
                        this._context.SaveChanges();

                        _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
                        return new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                        #endregion

                        #region ADO.NET Codeblock of updating data
                        //List<IDbDataParameter> upParameters = new()
                        //{
                        //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.UPDATE_KEY, DbType.String),
                        //    _dbmanager.CreateParameter("@Id", dbUserInfo.Id, DbType.Guid),
                        //    _dbmanager.CreateParameter("@FullName", dbUserInfo.FullName, DbType.String),
                        //    _dbmanager.CreateParameter("@UserName", dbUserInfo.UserName, DbType.String),
                        //    _dbmanager.CreateParameter("@Password", DBNull.Value, DbType.String),
                        //    _dbmanager.CreateParameter("@SaltKey", DBNull.Value, DbType.String),
                        //    _dbmanager.CreateParameter("@Email", dbUserInfo.Email, DbType.String),
                        //    _dbmanager.CreateParameter("@UserRole", dbUserInfo.UserRole, DbType.String),
                        //    _dbmanager.CreateParameter("@CreatedBy", DBNull.Value, DbType.String),
                        //    _dbmanager.CreateParameter("@CreatedDate", DBNull.Value, DbType.DateTime),
                        //    _dbmanager.CreateParameter("@UpdatedBy", dbUserInfo.UpdatedBy, DbType.String),
                        //    _dbmanager.CreateParameter("@UpdatedDate", dbUserInfo.UpdatedDate, DbType.DateTime)
                        //    _dbmanager.CreateParameter("@IsActive", oSaveUserInfo.IsActive, DbType.Boolean)
                        //};

                        //int isUpdate = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, upParameters.ToArray());

                        ////_securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));

                        //return isUpdate > 0
                        //? new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request }
                        //: new DataResponse { Success = false, Message = ConstantSupplier.REG_USER_UPDATE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        #endregion
                }

            }
            _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
            return new DataResponse { Success = false, Message = ConstantSupplier.REG_USER_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
        }

        /// <summary>
        /// <para>EF & ADO.NET Codeblocks: DeleteUserAsync</para>
        /// This method simply delete the user details from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> DeleteUserAsync(string id)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DELUSER_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            UserInfo? oUserInfo = await this._context.UserInfo.FindAsync(new Guid(id));
 
            if (oUserInfo != null)
            {

                #region EF Codeblock of deleting data
                if (_appSettings.IsUserDelate)
                {
                    this._context.UserInfo.Remove(oUserInfo);
                }
                else
                {
                    UserInfo oUserInfoUp = await _context.UserInfo.FindAsync(id);
                    oUserInfoUp.IsActive = false;
                    _context.Entry(oUserInfoUp).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                
                await this._context.SaveChangesAsync();
                return new DataResponse { Success = true, Message = ConstantSupplier.DELETE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oUserInfo };
                #endregion

                #region ADO.NET Codeblock of deleting data
                //List<IDbDataParameter> parameters = new()
                //        {
                //            _dbmanager.CreateParameter("@Id", oUserInfo.Id, DbType.Guid),
                //            _dbmanager.CreateParameter("@IsDelete", _appSettings.IsUserDelate? true: false, DbType.Boolean)
                //        };

                //object isDelete = await _dbmanager.DeleteAsync(ConstantSupplier.DELETE_USER_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());


                //return Convert.ToInt32(isDelete) > 0
                //? new DataResponse { Success = true, Message = ConstantSupplier.DELETE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oUserInfo }
                //: new DataResponse { Success = false, Message = ConstantSupplier.DELETE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                #endregion
            }
            _securityLogService.LogError(String.Format(ConstantSupplier.DELUSER_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
            return new DataResponse { Success = false, Message = ConstantSupplier.DELETE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = id };
        }

        /// <summary>
        /// This private method is being used for generating token after user credential found ok.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Token</returns>
        //private Token? GetToken(UserInfo user)
        //{

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(this._configuration["AppSettings:JWT:Key"]);

        //    DateTime expiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(this._configuration["AppSettings:AccessTokenExpireTime"]));
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //                new Claim(JwtRegisteredClaimNames.Sub, this._configuration["AppSettings:JWT:Subject"]),
        //                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //                new Claim("UserId", user.Id.ToString()),
        //                new Claim("FullName", user.FullName),
        //                new Claim("UserName", user.UserName),
        //                new Claim("Email", user.Email)
        //        }),
        //        Expires = expiryTime,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);

        //    if (tokenString != null)
        //    {
        //        return new Token()
        //        {
        //            access_token = tokenString,
        //            expires_in = Convert.ToInt32(Convert.ToDouble(this._configuration["AppSettings:AccessTokenExpireTime"])),
        //            token_type = ConstantSupplier.AUTHORIZATION_TOKEN_TYPE,
        //            error = string.Empty,
        //            error_description = string.Empty,
        //            user = new User() { Id = Convert.ToString(user.Id), FullName = user.UserName, UserName = user.UserName, Email = user.Email, UserRole = user.UserRole, CreatedDate = user.CreatedDate }

        //        };
        //    }
        //    return null;


        //}


        /// <summary>
        /// It update the "LastLoginAttemptAt" and "LoginFailedAttemptsCount" database table columns.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task TrackAndUpdateLoginAttempts(UserInfo? user)
        {
            var dbUserInfo = await this._context.UserInfo.FirstOrDefaultAsync(u => u.Id == user.Id);
            dbUserInfo.LastLoginAttemptAt = user.LastLoginAttemptAt;
            dbUserInfo.LoginFailedAttemptsCount = user.LoginFailedAttemptsCount;
            var isLastLoginAttemptAtModified = this._context.Entry(dbUserInfo).Property("LastLoginAttemptAt").IsModified;
            var isLoginFailedAttemptsCountModified = this._context.Entry(dbUserInfo).Property("LoginFailedAttemptsCount").IsModified;
            this._context.SaveChanges();
        }

        /// <summary>
        /// Send email regarding the user blocked after 3 times incorrect login attempt.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<SendResponse> SendEmail(LoginRequest request, UserInfo? user)
        {
            string body = _emailService.PopulateBody("EmailTemplates/emailblocknotice.htm", user.UserName,
                                        "User management", "https://localhost:4200/",
                                        "Please check your username and password. Please wait for mentioned time to re-login");
            var message = new Message() { To = user.Email, Name = request.UserName, Subject = "Email Blocked", Body = body };
            var emailResponse = await this._emailService.SendEmailAsync(_appSettings.EmailConfiguration, message);
            return emailResponse;
        }
        #endregion
    }
}
