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
using Org.BouncyCastle.Asn1.Ocsp;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Storage;

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

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        public UserService(IConfiguration config, SecurityDBContext context, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager)
        {
            _configuration = config;
            _context = context;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
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
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
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
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_RES_MSG, JsonConvert.SerializeObject(oExistAppUser, Formatting.Indented)));
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
                                RefreshToken = null,
                                RefreshTokenExpiryTime = null,
                                CreatedBy = request.CreateUpdatedBy,
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };

                            #region EF Codeblock of saving data
                            await _context.AppUsers.AddAsync(oNewAppUser);
                            int isSave = await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();

                            request.Id = Convert.ToString(oNewAppUser.Id);
                            if (!isSave.Equals(1))
                            {
                                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, JsonConvert.SerializeObject(oNewAppUser, Formatting.Indented)));
                            }

                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oNewAppUser };

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
                            //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, JsonConvert.SerializeObject(oNewAppUser, Formatting.Indented)));
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
                                oExistAppUser.RefreshToken = null;
                                oExistAppUser.RefreshTokenExpiryTime = null;
                                oExistAppUser.UpdatedBy = request.CreateUpdatedBy;
                                oExistAppUser.UpdatedDate = DateTime.UtcNow;
                                oExistAppUser.IsActive = request.IsActive;

                                #region EF Codeblock of saving data
                                await _context.SaveChangesAsync();
                                await oTrasaction.CommitAsync();
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
                                //    _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, JsonConvert.SerializeObject(oExistAppUser, Formatting.Indented)));
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
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
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
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
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
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_PROFILE_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(oAppUserProfileResult, Formatting.Indented)));
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
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                IQueryable<AppUserProfileResponse> query =
                from profile in _context.AppUserProfiles!
                join role in _context.AppUserRoles!
                    on profile.AppUserRoleId equals role.Id
                where profile.Id == new Guid(id)
                select new AppUserProfileResponse
                {
                    Id = profile.Id,
                    FullName = profile.FullName,
                    Address = profile.Address,
                    Email = profile.Email,
                    AppUserRoleId = profile.AppUserRoleId,
                    RoleName = role.RoleName,
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
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
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
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
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
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GET_USER_PROFILE_BY_ID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
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
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
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
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_APP_USER_PROFILE_RES_MSG, JsonConvert.SerializeObject(oExistAppUserProfile, Formatting.Indented)));
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_APP_USER_PROFILE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                return oDataResponse;
                            }

                            AppUserProfile oNewProfileRequest = new()
                            {
                                Id = Guid.NewGuid(),
                                FullName = request.FullName,
                                Address = request.Address,
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
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
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
        /// <para>EF & ADO.NET Codeblocks: DeleteAppUserProfileAsync</para>
        /// This method simply delete the user details from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> DeleteAppUserProfileAsync(string id)
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DEL_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                AppUserProfile? oExistAppUserProfile = await _context.AppUserProfiles.FindAsync(new Guid(id));

                if (Utilities.IsNotNull(oExistAppUserProfile))
                {

                    #region EF Codeblock of deleting data
                    if (_appSettings.IsUserDelate)
                    {
                        _context.AppUserProfiles.Remove(oExistAppUserProfile);
                    }
                    else
                    {
                        oExistAppUserProfile.IsActive = false;
                    }


                    await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();

                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.DELETE_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oExistAppUserProfile };
                    return oDataResponse;
                    #endregion

                    #region ADO.NET Codeblock of deleting data
                    //List<IDbDataParameter> parameters = new()
                    //        {
                    //            _dbmanager.CreateParameter("@Id", oExistAppUserProfile.Id, DbType.Guid),
                    //            _dbmanager.CreateParameter("@IsDelete", _appSettings.IsUserDelate? true: false, DbType.Boolean)
                    //        };

                    //object isDelete = await _dbmanager.DeleteAsync(ConstantSupplier.DELETE_USER_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                    //if (!isDelete.Equals(1))
                    //{
                    //    _securityLogService.LogError(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_FAILED_RES_MSG, isDelete));
                    //}

                    //return Convert.ToInt32(isDelete) > 0
                    //? new DataResponse { Success = true, Message = ConstantSupplier.DELETE_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oExistAppUserProfile }
                    //: new DataResponse { Success = false, Message = ConstantSupplier.DELETE_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    #endregion
                }
                _securityLogService.LogError(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.DELETE_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = id };
                return oDataResponse;
            }
            catch (Exception)
            {
                await oTrasaction.RollbackAsync();
                throw;
            }
            
        }

        
        #endregion
    }
}
