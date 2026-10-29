using Azure;
using Azure.Core;
using FluentEmail.Core.Models;
using SBERP.EmailService;
using SBERP.EmailService.Models;
using SBERP.EmailService.Service;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Persistence;
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
using SBERP.DataAccessLayer;
using System.Data;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using SBERP.Shared.Storage;

namespace SBERP.Security.Service
{
    /// <summary>
    /// includes all the methods for user operation incuding the getAllUser, getByUserId, registerUser, and DeleteUser . It implements  <see cref="IUserService"/>.
    /// </summary>
    public class UserService : IUserService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        private readonly SecurityDBContext _context;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly IDatabaseManager _dbmanager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IDistributedCache _distributedCache;
        private readonly IPhotoStorage _photoStorage;
        private readonly PhotoStorageSettings _photoSettings;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        public UserService(IConfiguration config, SecurityDBContext context, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager, IRefreshTokenService refreshTokenService,
        IDistributedCache distributedCache, IPhotoStorage photoStorage, IOptions<PhotoStorageSettings> photoSettings)
        {
            _configuration = config;
            _context = context;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _refreshTokenService = refreshTokenService;
            _distributedCache = distributedCache;
            _photoStorage = photoStorage;
            _photoSettings = photoSettings.Value;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.SCSqlConnectionString, _appSettings?.ConnectionProvider);
        }
        #endregion

        #region AppUser related methods
        /// <summary>
        /// <para>EF And ADO.NET Codeblocks: CreateUpdateAppUserAsync</para>
        /// <br>This method is used to create or update application user for getting user credential to use the application. Also it creates salt key to save</br> 
        /// <br>the password with proper hasing during the creating and updating the application user.</br>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DataResponse> CreateUpdateAppUserAsync(AppUserRequest request)
        {
            DataResponse? oDataResponse = null;
            AppUser? oExistAppUser = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_REQ_MSG, LogSafe.Serialize(request, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                string saltKey = BCryptNet.GenerateSalt(13);
                if (request != null)
                {
                    switch (request.ActionName)
                    {
                        case ConstantSupplier.SAVE_KEY:
                            oExistAppUser = await _context.AppUsers.FirstOrDefaultAsync(x => (x.UserName.Trim().ToLower()) == request.UserName.Trim().ToLower());
                            if (Utilities.IsNotNull(oExistAppUser))
                            {
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_RES_MSG, LogSafe.Serialize(oExistAppUser, Formatting.Indented)));
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_APP_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                return oDataResponse;
                            }


                            AppUser oNewAppUser = new()
                            {
                                Id = Guid.NewGuid(),
                                AppUserProfileId = new Guid(request.AppUserProfileId),
                                UserName = request.UserName,
                                Password = BCryptNet.HashPassword(request.Password, saltKey),
                                SaltKey = saltKey,
                                CreatedBy = request.CreateUpdatedBy,
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };

                            #region EF Codeblock of saving data
                            await _context.AppUsers.AddAsync(oNewAppUser);
                            int isSave = await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();

                            request.Id = Convert.ToString(oNewAppUser.Id);
                            request.Password = null;
                            if (!isSave.Equals(1))
                            {
                                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, LogSafe.Serialize(oNewAppUser, Formatting.Indented)));
                            }

                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };

                            #endregion

                            #region ADO.NET Codeblock of saving data
                            //object? refreshTokenValue = oNewAppUser.RefreshToken;
                            //object? refreshTokenExpiryTimeValue = oNewAppUser.RefreshTokenExpiryTime;
                            //List<IDbDataParameter> parameters = new()
                            //{
                            //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.SAVE_KEY, DbType.String),
                            //    _dbmanager.CreateParameter("@Id", oNewAppUser.Id, DbType.Guid),
                            //    _dbmanager.CreateParameter("@AppUserProfileId", oNewAppUser.AppUserProfileId, DbType.Guid),
                            //    _dbmanager.CreateParameter("@UserName", oNewAppUser.UserName, DbType.String),
                            //    _dbmanager.CreateParameter("@Password", oNewAppUser.Password, DbType.String),
                            //    _dbmanager.CreateParameter("@SaltKey", oNewAppUser.SaltKey, DbType.String),
                            //    _dbmanager.CreateParameter("@RefreshToken", refreshTokenValue, (DbType)(refreshTokenValue??DBNull.Value)),
                            //    _dbmanager.CreateParameter("@RefreshTokenExpiryTime", refreshTokenExpiryTimeValue, (DbType)(refreshTokenExpiryTimeValue??DBNull.Value)),
                            //    _dbmanager.CreateParameter("@CreatedBy", oNewAppUser.CreatedBy, DbType.String),
                            //    _dbmanager.CreateParameter("@CreatedDate", oNewAppUser.CreatedDate, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@UpdatedBy", DBNull.Value, DbType.String),
                            //    _dbmanager.CreateParameter("@UpdatedDate", DBNull.Value, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@IsActive", oNewAppUser.IsActive, DbType.Boolean)
                            //};

                            //int isSave = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_APP_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                            //request.Id = Convert.ToString(oNewAppUser.Id);
                            //if (!isSave.Equals(1))
                            //{
                            //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, LogSafe.Serialize(oNewAppUser, Formatting.Indented)));
                            //}

                            //oDataResponse =  isSave > 0
                            //? new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oNewAppUser }
                            //: new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                            #endregion

                            break;

                        case ConstantSupplier.UPDATE_KEY:
                            oExistAppUser = await _context.AppUsers.FirstOrDefaultAsync(x => (x.Id == new Guid(request.Id)) && (x.UserName.Trim().ToLower()) == request.UserName.Trim().ToLower());

                            if (Utilities.IsNotNull(oExistAppUser))
                            {
                                oExistAppUser.AppUserProfileId = new Guid(request.AppUserProfileId);
                                oExistAppUser.UserName = request.UserName;
                                oExistAppUser.SaltKey = saltKey;
                                oExistAppUser.Password = BCryptNet.HashPassword(request.Password, saltKey);
                                oExistAppUser.UpdatedBy = request.CreateUpdatedBy;
                                oExistAppUser.UpdatedDate = DateTime.UtcNow;
                                oExistAppUser.IsActive = request.IsActive;
                                oExistAppUser.FailedLoginAttempts = 0;
                                oExistAppUser.LockoutEnd = null;

                                #region EF Codeblock of saving data
                                await _context.SaveChangesAsync();
                                await oTrasaction.CommitAsync();
                                await _refreshTokenService.EndAllSessionsAsync(oExistAppUser.Id, ConstantSupplier.TOKEN_REVOKE_PASSWORD_CHANGED);
                                request.Password = null;
                                oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.UPDATE_APP_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                                #endregion

                                #region ADO.NET Codeblock of updating data
                                //object? refreshTokenValue = request.RefreshToken;
                                //object? refreshTokenExpiryTimeValue = request.RefreshTokenExpiryTime;
                                //List<IDbDataParameter> parameters = new()
                                //{
                                //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.UPDATE_KEY, DbType.String),
                                //    _dbmanager.CreateParameter("@Id", request.Id, DbType.Guid),
                                //    _dbmanager.CreateParameter("@AppUserProfileId", request.AppUserProfileId, DbType.Guid),
                                //    _dbmanager.CreateParameter("@UserName", request.UserName, DbType.String),
                                //    _dbmanager.CreateParameter("@SaltKey", saltKey, DbType.String),
                                //    _dbmanager.CreateParameter("@Password", BCryptNet.HashPassword(request.Password, saltKey), DbType.String),
                                //    _dbmanager.CreateParameter("@RefreshToken", refreshTokenValue, (DbType)(refreshTokenValue??DBNull.Value)),
                                //    _dbmanager.CreateParameter("@RefreshTokenExpiryTime", refreshTokenExpiryTimeValue, (DbType)(refreshTokenExpiryTimeValue??DBNull.Value)),
                                //    _dbmanager.CreateParameter("@CreatedBy", oExistAppUser.CreatedBy, DbType.String),
                                //    _dbmanager.CreateParameter("@CreatedDate", oExistAppUser.CreatedDate, DbType.DateTime),
                                //    _dbmanager.CreateParameter("@UpdatedBy", request.CreateUpdatedBy, DbType.String),
                                //    _dbmanager.CreateParameter("@UpdatedDate", DateTime.UtcNow, DbType.DateTime),
                                //    _dbmanager.CreateParameter("@IsActive", request.IsActive, DbType.Boolean)
                                //};

                                //int isUpdate = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_APP_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                                //if (!isUpdate.Equals(1))
                                //{
                                //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, LogSafe.Serialize(oExistAppUser, Formatting.Indented)));
                                //}

                                //oDataResponse = isUpdate > 0
                                //? new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oExistAppUser }
                                //: new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                                #endregion
                            }
                            else
                            {
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.UPDATE_APP_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                            }
                            break;

                    }
                    return oDataResponse;

                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, LogSafe.Serialize(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.CREATE_UPDATE_APP_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }
        #endregion

        #region AppUserProfile related methods
        /// <summary>
        /// <para>ADO.NET Codeblock: GetAllAppUserProfilePagingWithSearchAsync</para> 
        /// This method is used to get a list of user profile based on the supplied searchterm, sortcolumnname, sortcolumndirection, page number, and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>. 
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns></returns>
        public async Task<PagingResult<AppUserProfileResponse>?> GetAllAppUserProfilePagingWithSearchAsync(PagingSearchFilter paramRequest)
        {
            try
            {
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_REQ_MSG, LogSafe.Serialize(paramRequest, Formatting.Indented)));
                List<IDbDataParameter> parameters = new()
                {
                    _dbmanager.CreateParameter("@PageNumber", paramRequest.PageNumber, DbType.Int32),
                    _dbmanager.CreateParameter("@PageSize", paramRequest.PageSize, DbType.Int32),
                    _dbmanager.CreateParameter("@SearchTerm", paramRequest.SearchTerm, DbType.String),
                    _dbmanager.CreateParameter("@SortColumnName", paramRequest.SortColumnName, DbType.String),
                    _dbmanager.CreateParameter("@SortColumnDirection", paramRequest.SortColumnDirection, DbType.String)
                };

                object dbResult = await _dbmanager.GetScalarValueAsync(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());
                if (Utilities.IsNotNull(dbResult))
                {
                    string resultJStr = Utilities.ConvertJObjectToJsonString(dbResult, "Items");

                    // Deserialize the JSON string into PagingResult<AppUserProfile>
                    PagingResult<AppUserProfileResponse>? oAppUserProfileResult = JsonConvert.DeserializeObject<PagingResult<AppUserProfileResponse>>(resultJStr);
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_RES_MSG, LogSafe.Serialize(oAppUserProfileResult, Formatting.Indented)));
                    return oAppUserProfileResult;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// <para>EF Codeblock: GetAppUserProfileByIdAsync</para> 
        /// This service method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetAppUserProfileByIdAsync(string id)
        {
            DataResponse oDataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_REQ_MSG, LogSafe.Serialize(id, Formatting.Indented)));
            try
            {
                IQueryable<AppUserProfileResponse> query =
                from profile in _context.AppUserProfiles!
                join role in _context.AppUserRoles!
                    on profile.AppUserRoleId equals role.Id
                join user in _context.AppUsers!
                on profile.Id equals user.AppUserProfileId
                where profile.Id == new Guid(id)
                select new AppUserProfileResponse
                {
                    Id = profile.Id,
                    FullName = profile.FullName,
                    Address = profile.Address,
                    Email = profile.Email,
                    AppUserRoleId = profile.AppUserRoleId,
                    RoleName = role.RoleName,
                    JobTitle = profile.JobTitle,
                    UserName = user.UserName,
                    EmployeeId = profile.EmployeeId,
                    ProfilePhotoId = profile.ProfilePhotoId,
                    CreatedBy = profile.CreatedBy,
                    CreatedDate = profile.CreatedDate,
                    UpdatedBy = profile.UpdatedBy,
                    UpdatedDate = profile.UpdatedDate,
                    IsActive = profile.IsActive
                };

                AppUserProfileResponse? result = await query.FirstOrDefaultAsync();

                if (result == null)
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_RES_MSG, LogSafe.Serialize(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    // Serialize to JSON string (as requested)
                    string jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = jsonResult };
                }

                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetAppUserProfileByIdAdoAsync</para> 
        /// <para>This service method used to get a specific user details by supplying user id.</para> 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetAppUserProfileByIdAdoAsync(string id)
        {
            DataResponse oDataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_REQ_MSG, LogSafe.Serialize(id, Formatting.Indented)));
            try
            {
                AppUserProfileResponse? result;
                List<IDbDataParameter> parameters = new() { _dbmanager.CreateParameter("@Id", new Guid(id), DbType.Guid) };
                DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_USER_PROFILE_BY_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (oDT != null && oDT.Rows.Count > 0)
                {
                    result = JArray.FromObject(oDT)[0].ToObject<AppUserProfileResponse>();
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = result };
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_RES_MSG, LogSafe.Serialize(oDataResponse, Formatting.Indented)));
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// <para>EF And ADO.NET Codeblocks: CreateUpdateAppUserProfileAsync</para>
        /// <br>This method is used to registering or creating or updating application user profile.</br>
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> CreateUpdateAppUserProfileAsync(AppUserProfileRegisterRequest? request)
        {
            _securityLogService.LogInfo(String.Format(
                ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_REQ_MSG, 
                LogSafe.Serialize(request, Formatting.Indented)));
            if(request == null)
            {
                _securityLogService.LogError(String.Format(
                    ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, 
                    LogSafe.Serialize(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                return new DataResponse 
                { Success = false, Message = ConstantSupplier.CREATE_UPDATE_APP_USER_PROFILE_FAILED, 
                  MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null 
                };
                
            }

            // Normalize username early
            request.UserName = request.UserName?.Trim().ToLowerInvariant();

            // If password is provided, confirm must match
            if (!string.IsNullOrWhiteSpace(request.Password) &&
                request.Password != request.ConfirmPassword)
            {
                return new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.CREATE_APP_USER_PROFILE_PWD_NOT_MATCHED,
                    MessageType = Enum.EnumResponseType.Warning,
                    ResponseCode = (int)HttpStatusCode.BadRequest,
                    Result = request
                };
            }

            bool isCreate = request.ActionName == ConstantSupplier.SAVE_KEY;
            bool hasUserCredentials = !string.IsNullOrWhiteSpace(request.UserName);
            // set when the change must sign the user out everywhere (password, role, deactivation)
            string? endSessionsReason = null;

            await using IDbContextTransaction oTrasaction = await _context.Database.BeginTransactionAsync();
            try
            {
                AppUserProfile oAppUserProfile;
                if (isCreate)
                {
                    // Read-only check => AsNoTracking
                    AppUserProfile? oExistAppUserProfile = await _context.AppUserProfiles!
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.FullName != null &&
                            x.Email != null &&
                            x.FullName.Trim().ToLower() == request.FullName.Trim().ToLower() &&
                            x.Email.Trim().ToLower() == request.Email.Trim().ToLower() &&
                            x.AppUserRoleId == new Guid(request.AppUserRoleId));

                    if (Utilities.IsNotNull(oExistAppUserProfile))
                    {
                        _securityLogService.LogWarning(string.Format(
                            ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG,
                            LogSafe.Serialize(oExistAppUserProfile, Formatting.Indented)));

                        return new DataResponse
                        {
                            Success = false,
                            Message = ConstantSupplier.EXIST_APP_USER_PROFILE,
                            MessageType = Enum.EnumResponseType.Warning,
                            ResponseCode = (int)HttpStatusCode.BadRequest,
                            Result = request
                        };
                    }

                    // CREATE Profile
                    oAppUserProfile = new AppUserProfile
                    {
                        Id = Guid.NewGuid(),
                        FullName = request.FullName,
                        Address = request.Address,
                        JobTitle = NormalizeJobTitle(request.JobTitle),
                        Email = request.Email,
                        AppUserRoleId = new Guid(request.AppUserRoleId),
                        CreatedBy = request.CreateUpdateBy,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = request.IsActive
                    };
                    _context.AppUserProfiles!.Add(oAppUserProfile);
                }
                else
                {
                    // Must be tracked => NO AsNoTracking
                    Guid profileId = new Guid(request.Id!);
                    oAppUserProfile = await _context.AppUserProfiles!
                        .FirstOrDefaultAsync(p => p.Id == profileId);

                    if (Utilities.IsNull(oAppUserProfile))
                    {
                        _securityLogService.LogWarning(String.Format(
                            ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG,
                            LogSafe.Serialize(request, Formatting.Indented)));

                        return new DataResponse
                        {
                            Success = false,
                            Message = ConstantSupplier.CREATE_APP_USER_PROFILE_NOT_FOUND,
                            MessageType = Enum.EnumResponseType.Warning,
                            ResponseCode = (int)HttpStatusCode.BadRequest,
                            Result = request
                        };
                    }

                    if (oAppUserProfile.AppUserRoleId != new Guid(request.AppUserRoleId))
                        endSessionsReason = ConstantSupplier.TOKEN_REVOKE_ROLE_CHANGED;
                    if (oAppUserProfile.IsActive == true && request.IsActive != true)
                        endSessionsReason = ConstantSupplier.TOKEN_REVOKE_USER_DEACTIVATED;

                    oAppUserProfile.FullName = request.FullName;
                    oAppUserProfile.Address = request.Address;
                    oAppUserProfile.JobTitle = NormalizeJobTitle(request.JobTitle);
                    oAppUserProfile.Email = request.Email;
                    oAppUserProfile.AppUserRoleId = new Guid(request.AppUserRoleId);
                    oAppUserProfile.UpdatedBy = request.CreateUpdateBy;
                    oAppUserProfile.UpdatedDate = DateTime.UtcNow;
                    oAppUserProfile.IsActive = request.IsActive;
                }

                // Save profile first (so we have profile ID for AppUser)
                await _context.SaveChangesAsync();

                // Handle AppUser (one-to-one) - tracked (for update)
                AppUser? oAppUser = await _context.AppUsers!
                    .FirstOrDefaultAsync(u => u.AppUserProfileId == oAppUserProfile.Id);

                if (hasUserCredentials)
                {
                    bool isCreatingUser = (oAppUser == null);

                    request.Password = request.Password?.Trim();
                    bool isUpdatingPassword = !string.IsNullOrWhiteSpace(request.Password);

                    // If creating user OR updating password, password is mandatory
                    if ((isCreatingUser || isUpdatingPassword) && string.IsNullOrWhiteSpace(request.Password))
                    {
                        return new DataResponse
                        {
                            Success = false,
                            Message = ConstantSupplier.CREATE_UPDATE_APP_USER_REQ_PASSWORD,
                            MessageType = Enum.EnumResponseType.Warning,
                            ResponseCode = (int)HttpStatusCode.BadRequest,
                            Result = request
                        };
                    }

                    string? salt = null;
                    string? hashedPassword = null;

                    if (isCreatingUser || isUpdatingPassword)
                    {
                        salt = BCryptNet.GenerateSalt(13);
                        hashedPassword = BCryptNet.HashPassword(request.Password!, salt);
                    }
                    // the request is echoed in the response: do not send the password back
                    request.Password = null;
                    request.ConfirmPassword = null;

                    if (isCreatingUser)
                    {
                        oAppUser = new AppUser
                        {
                            Id = Guid.NewGuid(),
                            AppUserProfileId = oAppUserProfile.Id,
                            UserName = request.UserName!,
                            Password = hashedPassword!,
                            SaltKey = salt!,
                            CreatedBy = request.CreateUpdateBy,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = request.IsActive
                        };
                        _context.AppUsers!.Add(oAppUser);
                    }
                    else
                    {
                        oAppUser!.UserName = request.UserName!;
                        if (isUpdatingPassword)
                        {
                            oAppUser.Password = hashedPassword!;
                            oAppUser.SaltKey = salt!;
                            endSessionsReason ??= ConstantSupplier.TOKEN_REVOKE_PASSWORD_CHANGED;
                        }
                        if (oAppUser.IsActive == true && request.IsActive != true)
                            endSessionsReason = ConstantSupplier.TOKEN_REVOKE_USER_DEACTIVATED;
                        oAppUser.UpdatedBy = request.CreateUpdateBy;
                        oAppUser.UpdatedDate = DateTime.UtcNow;
                        oAppUser.IsActive = request.IsActive;
                        // saving the user is also how an administrator unlocks a locked login
                        oAppUser.FailedLoginAttempts = 0;
                        oAppUser.LockoutEnd = null;
                    }
                }

                await _context.SaveChangesAsync();
                await oTrasaction.CommitAsync();

                if (endSessionsReason != null && oAppUser != null)
                {
                    await _refreshTokenService.EndAllSessionsAsync(oAppUser.Id, endSessionsReason);
                    // a role change changes the menu: drop every cached menu
                    await MenuCache.InvalidateAllAsync(_distributedCache);
                }
                else if (oAppUser != null)
                {
                    // the cached login user carries FullName / Email / JobTitle: drop this user's entry
                    try { await _distributedCache.RemoveAsync(await MenuCache.UserMenuKeyAsync(_distributedCache, oAppUser.Id.ToString())); }
                    catch (Exception ex) { _securityLogService.LogWarning($"Menu cache of user {oAppUser.Id} not cleared: {ex.Message}"); }
                }

                // the client uploads the photo of a new profile with this id, after the save
                request.Id = oAppUserProfile.Id.ToString();
                return new DataResponse
                {
                    Success = true,
                    Message = isCreate
                        ? ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS
                        : ConstantSupplier.UPDATE_APP_USER_PROFILE_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.OK,
                    Result = request
                };
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueUserNameViolation(dbEx))
            {
                await oTrasaction.RollbackAsync();
                return new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.EXIST_APP_USER,
                    MessageType = Enum.EnumResponseType.Warning,
                    ResponseCode = (int)HttpStatusCode.BadRequest,
                    Result = request
                };
            }
            catch
            {
                await oTrasaction.RollbackAsync();
                throw;
            }
        }
        public async Task<DataResponse> CreateUpdateAppUserProfileAsyncTemp(AppUserProfileRegisterRequest? request)
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_REQ_MSG, LogSafe.Serialize(request, Formatting.Indented)));

            // Validate password match if password is provided
            if (!string.IsNullOrEmpty(request!.Password) && request.Password != request.ConfirmPassword)
            {
                return new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_PWD_NOT_MATCHED, MessageType = Enum.EnumResponseType.Warning, 
                    ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
            }

            bool isCreate = request.ActionName == ConstantSupplier.SAVE_KEY;

            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                if (request != null)
                {
                    AppUserProfile? oAppUserProfile;
                    if (isCreate)
                    {
                        AppUserProfile? oExistAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(x => (x.FullName!.Trim().ToLower()) == request.FullName.Trim().ToLower() && (x.Email.Trim().ToLower()) == request.Email.Trim().ToLower() && x.AppUserRoleId == new Guid(request.AppUserRoleId));

                        if (Utilities.IsNotNull(oExistAppUserProfile) && !String.IsNullOrWhiteSpace(Convert.ToString(request.Id)))
                        {
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG, LogSafe.Serialize(oExistAppUserProfile, Formatting.Indented)));
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_APP_USER_PROFILE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                            return oDataResponse;
                        }
                        // CREATE Profile
                        oAppUserProfile = new AppUserProfile
                        {
                            Id = Guid.NewGuid(),
                            FullName = request.FullName,
                            Address = request.Address,
                            JobTitle = NormalizeJobTitle(request.JobTitle),
                            Email = request.Email,
                            AppUserRoleId = new Guid(request.AppUserRoleId),
                            CreatedBy = request.CreateUpdateBy,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = request.IsActive
                        };
                        _context.AppUserProfiles!.Add(oAppUserProfile);

                    }
                    else
                    {
                        // UPDATE Profile
                        oAppUserProfile = await _context.AppUserProfiles!
                            .FirstOrDefaultAsync(p => p.Id == new Guid(request.Id!));

                        if (oAppUserProfile == null)
                        {
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG, LogSafe.Serialize(request, Formatting.Indented)));
                            return new DataResponse
                            {
                                Success = false,
                                Message = ConstantSupplier.CREATE_APP_USER_PROFILE_NOT_FOUND,
                                MessageType = Enum.EnumResponseType.Warning,
                                ResponseCode = (int)HttpStatusCode.BadRequest,
                                Result = request
                            };
                        }
                        else
                        {
                            oAppUserProfile.FullName = request.FullName;
                            oAppUserProfile.Address = request.Address;
                            oAppUserProfile.JobTitle = NormalizeJobTitle(request.JobTitle);
                            oAppUserProfile.Email = request.Email;
                            oAppUserProfile.AppUserRoleId = new Guid(request.AppUserRoleId);
                            oAppUserProfile.UpdatedBy = request.CreateUpdateBy;
                            oAppUserProfile.UpdatedDate = DateTime.UtcNow;
                            oAppUserProfile.IsActive = request.IsActive;
                        }
                    }
                    await _context.SaveChangesAsync();

                    // Handle AppUser (one-to-one)
                    AppUser? oAppUser = await _context.AppUsers!.FirstOrDefaultAsync(u => u.AppUserProfileId == oAppUserProfile.Id);

                    request.UserName = request.UserName?.Trim().ToLowerInvariant();

                    bool hasUserCredentials = !string.IsNullOrWhiteSpace(request.UserName);

                    if (hasUserCredentials)
                    {
                        //// Username uniqueness check
                        //AppUser? oExistingAppUserWithSameName = await _context.AppUsers!
                        //    .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.AppUserProfileId != oAppUserProfile.Id);

                        //if (oExistingAppUserWithSameName != null)
                        //{
                        //    return new DataResponse { Success = false, Message = ConstantSupplier.EXIST_APP_USER, MessageType = Enum.EnumResponseType.Warning, 
                        //        ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                        //}

                        bool isCreatingUser = oAppUser == null;
                        request.Password = request.Password?.Trim();
                        bool isUpdatingPassword = !string.IsNullOrEmpty(request.Password);

                        string? salt = null;
                        string? hashedPassword = null;

                        if (isCreatingUser || isUpdatingPassword)
                        {
                            if (string.IsNullOrEmpty(request.Password))
                            {
                                return new DataResponse { Success = false, Message = ConstantSupplier.CREATE_UPDATE_APP_USER_REQ_PASSWORD,
                                    MessageType = Enum.EnumResponseType.Warning,
                                    ResponseCode = (int)HttpStatusCode.BadRequest,
                                    Result = request
                                };
                            }

                            salt = BCryptNet.GenerateSalt(13); // or 13 as in your example
                            hashedPassword = BCryptNet.HashPassword(request.Password, salt);
                        }

                        if (isCreatingUser)
                        {
                            oAppUser = new AppUser
                            {
                                Id = Guid.NewGuid(),
                                AppUserProfileId = oAppUserProfile.Id,
                                UserName = request.UserName!,
                                Password = hashedPassword!,
                                SaltKey = salt!,
                                CreatedBy = request.CreateUpdateBy,
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };
                            _context.AppUsers!.Add(oAppUser);
                        }
                        else
                        {
                            oAppUser.UserName = request.UserName!;
                            if (isUpdatingPassword)
                            {
                                oAppUser.Password = hashedPassword!;
                                oAppUser.SaltKey = salt!;
                            }
                            oAppUser.UpdatedBy = request.CreateUpdateBy;
                            oAppUser.UpdatedDate = DateTime.UtcNow;
                            oAppUser.IsActive = request.IsActive;
                        }
                    }
                    // If UserName is empty, do nothing — keep existing AppUser or leave none

                    //await _context.SaveChangesAsync();
                    int isSaveUpdateProfile = await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();
                    if(isSaveUpdateProfile <= 0)
                    {
                        _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, isSaveUpdateProfile));
                        return new DataResponse
                        {
                            Success = false,
                            Message = isCreate ? ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_FAILED : ConstantSupplier.UPDATE_APP_USER_PROFILE_FAILED,
                            MessageType = Enum.EnumResponseType.Error,
                            ResponseCode = (int)HttpStatusCode.BadRequest,
                            Result = request
                        };
                    }

                    string successMsg = isCreate
                        ? ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS
                        : ConstantSupplier.UPDATE_APP_USER_PROFILE_SUCCESS;

                    return new DataResponse { Success = true, Message = successMsg,
                        MessageType = Enum.EnumResponseType.Success,
                        ResponseCode = (int)HttpStatusCode.OK,
                        Result = request
                    };

                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, LogSafe.Serialize(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.CREATE_UPDATE_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                return oDataResponse;
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueUserNameViolation(dbEx))
            {
                await oTrasaction.RollbackAsync();

                return new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.EXIST_APP_USER, // your duplicate username message
                    MessageType = Enum.EnumResponseType.Warning,
                    ResponseCode = (int)HttpStatusCode.BadRequest,
                    Result = request
                };
            }
            catch (Exception)
            {
                await oTrasaction.RollbackAsync();
                throw;
            }

        }

        /// <summary>
        /// <para>EF And ADO.NET Codeblocks: CreateUpdateAppUserProfileAsync</para>
        /// <br>This method is used to registering or creating or updating application user profile.</br>
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> CreateUpdateAppUserProfileAsyncExtn(AppUserProfileRegisterRequest? request)
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_REQ_MSG, LogSafe.Serialize(request, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                if (request != null)
                {
                    switch (request.ActionName)
                    {
                        case ConstantSupplier.SAVE_KEY:
                            AppUserProfile? oExistAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(x => (x.FullName.Trim().ToLower()) == request.FullName.Trim().ToLower() && (x.Email.Trim().ToLower()) == request.Email.Trim().ToLower() && x.AppUserRoleId == new Guid(request.AppUserRoleId));

                            if (Utilities.IsNotNull(oExistAppUserProfile) && !String.IsNullOrWhiteSpace(Convert.ToString(request.Id)))
                            {
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG, LogSafe.Serialize(oExistAppUserProfile, Formatting.Indented)));
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_APP_USER_PROFILE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                return oDataResponse;
                            }

                            AppUserProfile oNewProfileRequest = new()
                            {
                                Id = Guid.NewGuid(),
                                FullName = request.FullName,
                                Address = request.Address,
                                JobTitle = NormalizeJobTitle(request.JobTitle),
                                Email = request.Email,
                                AppUserRoleId = new Guid(request.AppUserRoleId),
                                CreatedBy = request.CreateUpdateBy,
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };

                            #region EF Code block of saving data
                            await _context.AppUserProfiles.AddAsync(oNewProfileRequest);
                            int isSave = await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();

                            request.Id = Convert.ToString(oNewProfileRequest.Id);
                            if (!isSave.Equals(1))
                            {
                                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, oNewProfileRequest));
                            }
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oNewProfileRequest };
                            #endregion

                            #region ADO.NET Codeblock of saving data
                            //List<IDbDataParameter> parameters = new()
                            //{
                            //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.SAVE_KEY, DbType.String),
                            //    _dbmanager.CreateParameter("@Id", oNewProfileRequest.Id, DbType.Guid),
                            //    _dbmanager.CreateParameter("@FullName", oNewProfileRequest.FullName, DbType.String),
                            //    _dbmanager.CreateParameter("@Address", oNewProfileRequest.Address, DbType.String),
                            //    _dbmanager.CreateParameter("@Email", oNewProfileRequest.Email, DbType.String),
                            //    _dbmanager.CreateParameter("@RoleId", oNewProfileRequest.AppUserRoleId, DbType.Guid),
                            //    _dbmanager.CreateParameter("@CreatedBy", oNewProfileRequest.CreatedBy, DbType.String),
                            //    _dbmanager.CreateParameter("@CreatedDate", oNewProfileRequest.CreatedDate, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@UpdatedBy", DBNull.Value, DbType.String),
                            //    _dbmanager.CreateParameter("@UpdatedDate", DBNull.Value, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@IsActive", oNewProfileRequest.IsActive, DbType.Boolean)
                            //};

                            //int isSave = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                            //request.Id = Convert.ToString(oNewProfileRequest.Id);
                            //if (!isSave.Equals(1))
                            //{
                            //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, oNewProfileRequest));
                            //}


                            //oDataResponse = isSave > 0
                            //? new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oNewProfileRequest }
                            //: new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                            #endregion

                            break;

                        case ConstantSupplier.UPDATE_KEY:

                            AppUserProfile? oOldAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(x => x.Id == new Guid(request.Id));

                            if (Utilities.IsNotNull(oOldAppUserProfile))
                            {
                                oOldAppUserProfile.FullName = request.FullName;
                                oOldAppUserProfile.Address = request.Address;
                                oOldAppUserProfile.JobTitle = NormalizeJobTitle(request.JobTitle);
                                oOldAppUserProfile.Email = request.Email;
                                oOldAppUserProfile.AppUserRoleId = new Guid(request.AppUserRoleId);
                                oOldAppUserProfile.UpdatedBy = request.CreateUpdateBy;
                                oOldAppUserProfile.UpdatedDate = DateTime.UtcNow;
                                oOldAppUserProfile.IsActive = request.IsActive;


                                #region EF Code block of saving data
                                int isUpdate = await _context.SaveChangesAsync();
                                await oTrasaction.CommitAsync();
                                if (!isUpdate.Equals(1))
                                {
                                    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, oOldAppUserProfile));
                                }
                                oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.UPDATE_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oOldAppUserProfile };
                                #endregion

                                #region ADO.NET Codeblock of updating data
                                //List<IDbDataParameter> upParameters = new()
                                //{
                                //    _dbmanager.CreateParameter("@ActionName", ConstantSupplier.UPDATE_KEY, DbType.String),
                                //    _dbmanager.CreateParameter("@Id", oOldAppUserProfile.Id, DbType.Guid),
                                //    _dbmanager.CreateParameter("@FullName", oOldAppUserProfile.FullName, DbType.String),
                                //    _dbmanager.CreateParameter("@Address", oOldAppUserProfile.Address, DbType.String),
                                //    _dbmanager.CreateParameter("@Email", oOldAppUserProfile.Email, DbType.String),
                                //    _dbmanager.CreateParameter("@RoleId", oOldAppUserProfile.AppUserRoleId, DbType.Guid),
                                //    _dbmanager.CreateParameter("@CreatedBy", DBNull.Value, DbType.String),
                                //    _dbmanager.CreateParameter("@CreatedDate", DBNull.Value, DbType.DateTime),
                                //    _dbmanager.CreateParameter("@UpdatedBy", oOldAppUserProfile.UpdatedBy, DbType.String),
                                //    _dbmanager.CreateParameter("@UpdatedDate", oOldAppUserProfile.UpdatedDate, DbType.DateTime),
                                //    _dbmanager.CreateParameter("@IsActive", oOldAppUserProfile.IsActive, DbType.Boolean)
                                //};

                                //int isUpdate = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, upParameters.ToArray());
                                //if (!isUpdate.Equals(1))
                                //{
                                //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, isUpdate));
                                //}
                                //oDataResponse = isUpdate > 0
                                //? new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request }
                                //: new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                                #endregion
                            }
                            else
                            {
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                            }
                            break;
                    }
                    return oDataResponse;
                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, LogSafe.Serialize(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.CREATE_UPDATE_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                return oDataResponse;
            }
            catch (Exception)
            {
                await oTrasaction.RollbackAsync();
                throw;
            }

        }

        /// <summary>
        /// <para>EF And ADO.NET Codeblocks: DeleteAppUserProfileAsync</para>
        /// <br>This method simply delete the profile and user details from the database.</br>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> DeleteAppUserProfileAsync(string id)
        {
            using IDbContextTransaction oTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Guid profileId = new(id);

                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FindAsync(profileId);

                if (oAppUserProfile == null)
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_FAILED_RES_MSG, LogSafe.Serialize(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                    return new DataResponse { Success = false, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_NOT_FOUND, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = id };
                }

                int affectedRows = 0;

                /* ============================================================
                   EF CORE CODEBLOCK
                   ============================================================ */
                #region EF Codeblock of deleting data
                AppUser? oAppUser = await _context.AppUsers!.FirstOrDefaultAsync(u => u.AppUserProfileId == profileId);

                if (oAppUser != null)
                {
                    if (_appSettings!.IsUserDelate)
                        _context.AppUsers!.Remove(oAppUser);
                    else
                        oAppUser.IsActive = false;
                }

                if (_appSettings!.IsUserDelate)
                    _context.AppUserProfiles!.Remove(oAppUserProfile);
                else
                    oAppUserProfile.IsActive = false;

                affectedRows = await _context.SaveChangesAsync();

                if (affectedRows <= 0)
                {
                    await oTransaction.RollbackAsync();

                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.DELETE_APP_USER_PROFILE_FAILED,
                        MessageType = Enum.EnumResponseType.Error,
                        ResponseCode = (int)HttpStatusCode.BadRequest,
                        Result = null
                    };
                }
                else
                {
                    await oTransaction.CommitAsync();
                    if (_appSettings!.IsUserDelate)
                        _photoStorage.Delete(oAppUserProfile.ProfilePhotoLocation);
                    if (oAppUser != null)
                        await _refreshTokenService.EndAllSessionsAsync(oAppUser.Id, ConstantSupplier.TOKEN_REVOKE_USER_DELETED);
                    return new DataResponse
                    {
                        Success = true,
                        Message = ConstantSupplier.DELETE_APP_USER_PROFILE_SUCCESS,
                        MessageType = Enum.EnumResponseType.Success,
                        ResponseCode = (int)HttpStatusCode.OK,
                        Result = null
                    };
                }
                #endregion

                /* ============================================================
                   ADO.NET CODEBLOCK (LEGACY – COMMENTED)
                   ============================================================ */
                #region ADO.NET Codeblock of deleting data (Legacy)
                /*
                List<IDbDataParameter> parameters = new()
                {
                    _dbmanager.CreateParameter("@Id", profileId, DbType.Guid),
                    _dbmanager.CreateParameter("@IsDelete", _appSettings.IsUserDelate, DbType.Boolean)
                };

                object rowsAffected = await _dbmanager.DeleteAsync(ConstantSupplier.DELETE_USER_SP_NAME,
                        CommandType.StoredProcedure,
                        parameters.ToArray());

                if (Convert.ToInt32(rowsAffected) <= 0)
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_FAILED_RES_MSG, rowsAffected));

                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.DELETE_APP_USER_PROFILE_FAILED,
                        MessageType = Enum.EnumResponseType.Error,
                        ResponseCode = (int)HttpStatusCode.BadRequest,
                        Result = null
                    };
                }

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.DELETE_APP_USER_PROFILE_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.OK,
                    Result = null
                };
                */
                #endregion
            }
            catch (Exception)
            {
                await oTransaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region AppUserProfile photo
        /// <summary>
        /// Stores the uploaded photo as {PhotoStorage:UserProfileFolder}/{new ProfilePhotoId}.{jpg|png} and saves the id and
        /// location on the profile. The previous photo file is deleted only after the database update succeeded.
        /// </summary>
        /// <summary>Trims the free-text role name / designation; blank becomes null; capped at the column size.</summary>
        private static string? NormalizeJobTitle(string? jobTitle)
        {
            if (string.IsNullOrWhiteSpace(jobTitle)) return null;
            string value = jobTitle.Trim();
            return value.Length > 100 ? value[..100] : value;
        }

        public async Task<DataResponse> UploadAppUserProfilePhotoAsync(string id, Stream content, long length, string? updatedBy)
        {
            if (!Guid.TryParse(id, out Guid profileId))
                return PhotoResponse(false, HttpStatusCode.BadRequest, ConstantSupplier.APP_USER_PROFILE_PHOTO_INVALID_ID);

            AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(p => p.Id == profileId);
            if (oAppUserProfile == null)
                return PhotoResponse(false, HttpStatusCode.NotFound, ConstantSupplier.CREATE_APP_USER_PROFILE_NOT_FOUND);

            StoredPhoto stored;
            try
            {
                stored = await _photoStorage.SaveAsync(_photoSettings.UserProfileFolder, content, length);
            }
            catch (PhotoValidationException ex)
            {
                return PhotoResponse(false, HttpStatusCode.BadRequest, ex.Message);
            }

            string? oldLocation = oAppUserProfile.ProfilePhotoLocation;
            oAppUserProfile.ProfilePhotoId = stored.PhotoId;
            oAppUserProfile.ProfilePhotoLocation = stored.Location;
            oAppUserProfile.UpdatedBy = updatedBy;
            oAppUserProfile.UpdatedDate = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                _photoStorage.Delete(stored.Location);   // no orphan file when the row was not updated
                throw;
            }
            _photoStorage.Delete(oldLocation);

            _securityLogService.LogInfo($"Profile photo {stored.PhotoId} stored for profile {profileId}.");
            return PhotoResponse(true, HttpStatusCode.OK, ConstantSupplier.APP_USER_PROFILE_PHOTO_UPLOAD_SUCCESS,
                new { Id = profileId, ProfilePhotoId = stored.PhotoId });
        }

        /// <summary>The stored photo of a profile, or null when it has none (the client shows the default avatar).</summary>
        public async Task<PhotoFile?> GetAppUserProfilePhotoAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid profileId)) return null;
            var photo = await _context.AppUserProfiles!.AsNoTracking()
                .Where(p => p.Id == profileId && p.ProfilePhotoId != null)
                .Select(p => new { p.ProfilePhotoId, p.ProfilePhotoLocation })
                .FirstOrDefaultAsync();
            if (photo?.ProfilePhotoLocation == null) return null;

            Stream? stream = _photoStorage.OpenRead(photo.ProfilePhotoLocation);
            if (stream == null)
            {
                _securityLogService.LogWarning($"Profile photo file {photo.ProfilePhotoLocation} of profile {profileId} is missing.");
                return null;
            }
            return new PhotoFile(stream, _photoStorage.GetContentType(photo.ProfilePhotoLocation), photo.ProfilePhotoId!.Value);
        }

        /// <summary>Clears the profile photo (back to the default avatar) and deletes its file.</summary>
        public async Task<DataResponse> RemoveAppUserProfilePhotoAsync(string id, string? updatedBy)
        {
            if (!Guid.TryParse(id, out Guid profileId))
                return PhotoResponse(false, HttpStatusCode.BadRequest, ConstantSupplier.APP_USER_PROFILE_PHOTO_INVALID_ID);

            AppUserProfile? oAppUserProfile = await _context.AppUserProfiles!.FirstOrDefaultAsync(p => p.Id == profileId);
            if (oAppUserProfile == null)
                return PhotoResponse(false, HttpStatusCode.NotFound, ConstantSupplier.CREATE_APP_USER_PROFILE_NOT_FOUND);

            string? oldLocation = oAppUserProfile.ProfilePhotoLocation;
            if (oAppUserProfile.ProfilePhotoId != null || oldLocation != null)
            {
                oAppUserProfile.ProfilePhotoId = null;
                oAppUserProfile.ProfilePhotoLocation = null;
                oAppUserProfile.UpdatedBy = updatedBy;
                oAppUserProfile.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _photoStorage.Delete(oldLocation);
            }
            return PhotoResponse(true, HttpStatusCode.OK, ConstantSupplier.APP_USER_PROFILE_PHOTO_REMOVE_SUCCESS, new { Id = profileId });
        }

        private static DataResponse PhotoResponse(bool success, HttpStatusCode code, string message, object? result = null) => new()
        {
            Success = success,
            Message = message,
            MessageType = success ? Enum.EnumResponseType.Success : Enum.EnumResponseType.Warning,
            ResponseCode = (int)code,
            Result = result
        };
        #endregion
    }
}
