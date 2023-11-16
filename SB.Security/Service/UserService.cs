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
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Storage;

namespace SB.Security.Service
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
        /// 
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

        #region All service methods

        /// <summary>
        /// <para>EF Codeblock: GetAllUserAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PageResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PageResult<AppUserProfile>> GetAllUserAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                int? count = await _context.AppUserProfiles.CountAsync();
                List<AppUserProfile>? oAppUserProfileList = await _context.AppUserProfiles.OrderByDescending(x => x.CreatedDate).Skip((paramRequest.PageNumber - 1) * paramRequest.PageSize).Take(paramRequest.PageSize).ToListAsync();
                PageResult<AppUserProfile> result = new()
                {
                    Count = Convert.ToInt32(count),
                    PageIndex = paramRequest.PageNumber > 0 ? paramRequest.PageNumber : 1,
                    PageSize = 10,
                    Items = oAppUserProfileList
                };

                if (Utilities.IsNull(result) && !result.Items.Any())
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>EF Codeblock: GetAllUserExtnAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PagingResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<AppUserProfile>> GetAllUserExtnAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                IQueryable<AppUserProfile>? source = (from user in _context?.AppUserProfiles?.OrderBy(a => a.CreatedDate) select user).AsQueryable();
                PagingResult<AppUserProfile> result = await Utilities.GetPagingResult(source, paramRequest.PageNumber, paramRequest.PageSize);

                if (Utilities.IsNull(result) && !result.Items.Any())
                {
                    _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetAllUserAdoAsync</para> 
        /// This service method used to get a list users based on the supplied page number and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<AppUserProfile>?> GetAllUserAdoAsync(PaginationFilter paramRequest)
        {
            PagingResult<AppUserProfile>? result = null;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                List<AppUserProfile> oAppUserProfileList;
                List<IDbDataParameter> parameters = new(){
                _dbmanager.CreateParameter("@PageIndex", paramRequest.PageNumber, DbType.Int32),
                _dbmanager.CreateParameter("@PageSize", paramRequest.PageSize, DbType.Int32)};

                DataTable oDataTable = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_ALL_USER_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (Utilities.IsNotNull(oDataTable) && oDataTable.Rows.Count > 0)
                {
                    oAppUserProfileList = Utilities.ConvertDataTable<AppUserProfile>(oDataTable);
                    result = Utilities.GetPagingResult(oAppUserProfileList, paramRequest.PageNumber, paramRequest.PageSize);
                }
                else
                {
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(string.Empty, Formatting.Indented)));
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// <para>EF Codeblock: GetUserByIdAsync</para> 
        /// This service method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAsync(string id)
        {
            DataResponse oDataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                AppUserProfile? oAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(u => u.Id == new Guid(id) && u.IsActive == true);

                if (oAppUserProfile != null)
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oAppUserProfile };
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetUserByIdAdoAsync</para> 
        /// <para>This service method used to get a specific user details by supplying user id.</para> 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAdoAsync(string id)
        {
            DataResponse oDataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                AppUserProfile? user;
                List<IDbDataParameter> parameters = new() { _dbmanager.CreateParameter("@Id", new Guid(id), DbType.Guid) };
                DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_USER_BY_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (oDT != null && oDT.Rows.Count > 0)
                {
                    user = JArray.FromObject(oDT)[0].ToObject<AppUserProfile>();
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = user };
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_APP_USER_PROFILE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
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
                                CreatedBy = request.CreateUpdatedBy,
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };
                            await _context.AppUserProfiles.AddAsync(oNewProfileRequest);
                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();

                            request.Id = Convert.ToString(oNewProfileRequest.Id);
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_PROFILE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };

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
                            //    _dbmanager.CreateParameter("@RoleId", oSaveUserInfo.RoleId, DbType.Guid),
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

                            break;

                        case ConstantSupplier.UPDATE_KEY:

                            AppUserProfile? oOldAppUserProfile = await _context.AppUserProfiles.FirstOrDefaultAsync(x => x.Id == new Guid(request.Id));

                            if (Utilities.IsNotNull(oOldAppUserProfile))
                            {
                                oOldAppUserProfile.FullName = request.FullName;
                                oOldAppUserProfile.Address = request.Address;
                                oOldAppUserProfile.Email = request.Email;
                                oOldAppUserProfile.AppUserRoleId = new Guid(request.AppUserRoleId);
                                oOldAppUserProfile.UpdatedBy = request.CreateUpdatedBy;
                                oOldAppUserProfile.UpdatedDate = DateTime.UtcNow;
                                oOldAppUserProfile.IsActive = request.IsActive;
                            }

                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.UPDATE_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                            break;

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
                            //    _dbmanager.CreateParameter("@RoleId", dbUserInfo.RoleId, DbType.Guid),
                            //    _dbmanager.CreateParameter("@CreatedBy", DBNull.Value, DbType.String),
                            //    _dbmanager.CreateParameter("@CreatedDate", DBNull.Value, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@UpdatedBy", dbUserInfo.UpdatedBy, DbType.String),
                            //    _dbmanager.CreateParameter("@UpdatedDate", dbUserInfo.UpdatedDate, DbType.DateTime),
                            //    _dbmanager.CreateParameter("@IsActive", oSaveUserInfo.IsActive, DbType.Boolean)
                            //};

                            //int isUpdate = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, upParameters.ToArray());

                            //return isUpdate > 0
                            //? new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request }
                            //: new DataResponse { Success = false, Message = ConstantSupplier.REG_USER_UPDATE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                            #endregion
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
                        //_context.Entry(oExistAppUserProfile).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    await oTrasaction.CommitAsync();

                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.DELETE_APP_USER_PROFILE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oExistAppUserProfile };
                    return oDataResponse;
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
                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();

                            request.Id = Convert.ToString(oNewAppUser.Id);
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.CREATE_APP_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                            
                            #endregion
                            break;

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
                        //    _dbmanager.CreateParameter("@RoleId", oSaveUserInfo.RoleId, DbType.Guid),
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
                            oExistAppUser = await _context.AppUsers.FirstOrDefaultAsync(x => (x.Id == new Guid(request.Id)) && (x.UserName.Trim().ToLower()) == request.UserName.Trim().ToLower());

                            if (Utilities.IsNotNull(oExistAppUser))
                            {
                                //_securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(oldUser, Formatting.Indented)));
                                //oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                //return oDataResponse;
                                oExistAppUser.AppUserProfileId = new Guid(request.AppUserProfileId);
                                oExistAppUser.UserName = request.UserName;
                                oExistAppUser.SaltKey = saltKey;
                                oExistAppUser.Password = BCryptNet.HashPassword(request.Password, saltKey);
                                oExistAppUser.RefreshToken = null;
                                oExistAppUser.RefreshTokenExpiryTime = null;
                                oExistAppUser.UpdatedBy = request.CreateUpdatedBy;
                                oExistAppUser.UpdatedDate = DateTime.UtcNow;
                                oExistAppUser.IsActive = request.IsActive;
                                await _context.SaveChangesAsync();
                                await oTrasaction.CommitAsync();
                                oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.UPDATE_APP_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                            }
                            else
                            {
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.UPDATE_APP_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                            }
                            break;

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
                            //    _dbmanager.CreateParameter("@RoleId", dbUserInfo.RoleId, DbType.Guid),
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
    }
}
