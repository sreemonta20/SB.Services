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

namespace SB.Security.Service
{
    /// <summary>
    /// includes all the methods for user operation incuding the getAllUser, getByUserId, registerUser, and DeleteUser . It implements  <see cref="IUserService"/>.
    /// </summary>
    public class UserService : IUserService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        private readonly SBSecurityDBContext _context;
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
        public UserService(IConfiguration config, SBSecurityDBContext context, IOptions<AppSettings> options,
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
        public async Task<PageResult<UserInfo>> GetAllUserAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                int count = await _context.UserInfo.CountAsync();
                List<UserInfo> Items = await _context.UserInfo.OrderByDescending(x => x.CreatedDate).Skip((paramRequest.PageNumber - 1) * paramRequest.PageSize).Take(paramRequest.PageSize).ToListAsync();
                PageResult<UserInfo> result = new()
                {
                    Count = count,
                    PageIndex = paramRequest.PageNumber > 0 ? paramRequest.PageNumber : 1,
                    PageSize = 10,
                    Items = Items
                };
                if (!result.Items.Any())
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
        public async Task<PagingResult<UserInfo>> GetAllUserExtnAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                IQueryable<UserInfo> source = (from user in _context?.UserInfo?.OrderBy(a => a.CreatedDate) select user).AsQueryable();
                PagingResult<UserInfo> result = await Utilities.GetPagingResult(source, paramRequest.PageNumber, paramRequest.PageSize);

                if (!result.Items.Any())
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
        public async Task<PagingResult<UserInfo>?> GetAllUserAdoAsync(PaginationFilter paramRequest)
        {
            PagingResult<UserInfo>? result = null;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALL_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                List<UserInfo> oUserList;
                List<IDbDataParameter> parameters = new(){
                _dbmanager.CreateParameter("@PageIndex", paramRequest.PageNumber, DbType.Int32),
                _dbmanager.CreateParameter("@PageSize", paramRequest.PageSize, DbType.Int32)};

                DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_ALL_USER_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (oDT != null && oDT.Rows.Count > 0)
                {
                    oUserList = Utilities.ConvertDataTable<UserInfo>(oDT);
                    result = Utilities.GetPagingResult(oUserList, paramRequest.PageNumber, paramRequest.PageSize);
                }
                else
                {
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETALL_RES_MSG, JsonConvert.SerializeObject(string.Empty, Formatting.Indented)));
                }

            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// <para>EF Codeblock: GetUserByIdAsync</para> 
        /// This service method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAsync(string id)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                UserInfo? user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == new Guid(id) && u.IsActive == true);

                if (user != null)
                {
                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = user };
                }
                else
                {
                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetUserByIdAdoAsync</para> 
        /// <para>This service method used to get a specific user details by supplying user id.</para> 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> GetUserByIdAdoAsync(string id)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                UserInfo? user;
                List<IDbDataParameter> parameters = new() { _dbmanager.CreateParameter("@Id", new Guid(id), DbType.Guid) };
                DataTable oDT = await _dbmanager.GetDataTableAsync(ConstantSupplier.GET_USER_BY_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());
                
                if (oDT != null && oDT.Rows.Count > 0)
                {
                    user = JArray.FromObject(oDT)[0].ToObject<UserInfo>();
                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = user };
                }
                else
                {
                    dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_USER_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(dataResponse, Formatting.Indented)));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
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
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUP_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            try
            {
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
                                RoleId = new Guid(request.RoleId),
                                CreatedBy = Convert.ToString(_context.UserInfo.FirstOrDefault(s => s.UserRole.Equals(ConstantSupplier.ADMIN)).Id),
                                CreatedDate = DateTime.UtcNow,
                                IsActive = request.IsActive
                            };

                            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName);
                            if (user != null && !String.IsNullOrEmpty(Convert.ToString(user.Id)))
                            {
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(user, Formatting.Indented)));
                                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                return dataResponse;
                            }

                            #region EF Codeblock of saving data
                            await _context.UserInfo.AddAsync(oSaveUserInfo);
                            await _context.SaveChangesAsync();

                            request.Id = Convert.ToString(oSaveUserInfo.Id);
                            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                            return dataResponse;
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

                            var oldUser = await _context.UserInfo.FirstOrDefaultAsync(u => u.UserName == request.UserName);

                            if ((oldUser != null) && (oldUser.Id != new Guid(request.Id)))
                            {
                                _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUP_RES_MSG, JsonConvert.SerializeObject(oldUser, Formatting.Indented)));
                                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                                return dataResponse;
                            }


                            var dbUserInfo = _context.UserInfo.FirstOrDefault(s => s.Id.Equals(new Guid(request.Id)));
                            dbUserInfo.FullName = request.FullName;
                            dbUserInfo.UserName = request.UserName;
                            dbUserInfo.Email = request.Email;
                            //dbUserInfo.UserRole = request.UserRole;
                            //dbUserInfo.UserRole = new Guid(request.UserRole);
                            dbUserInfo.RoleId = new Guid(request.RoleId);
                            dbUserInfo.UpdatedBy = Convert.ToString(_context.UserInfo.FirstOrDefault(s => s.UserRole.Equals(ConstantSupplier.ADMIN)).Id);
                            dbUserInfo.UpdatedDate = DateTime.UtcNow;
                            dbUserInfo.IsActive = request.IsActive;

                            #region EF Codeblock of updating data
                            var isFullNameModified = _context.Entry(dbUserInfo).Property("FullName").IsModified;
                            var isUserNameModified = _context.Entry(dbUserInfo).Property("UserName").IsModified;
                            var isEmailModified = _context.Entry(dbUserInfo).Property("Email").IsModified;
                            var isRoleIdModified = _context.Entry(dbUserInfo).Property("RoleId").IsModified;
                            var isUpdatedByModified = _context.Entry(dbUserInfo).Property("UpdatedBy").IsModified;
                            var isUpdatedDateModified = _context.Entry(dbUserInfo).Property("UpdatedDate").IsModified;
                            var isIsActive = _context.Entry(dbUserInfo).Property("IsActive").IsModified;
                            _context.SaveChanges();

                            dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.REG_USER_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                            return dataResponse;
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

                }
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_FAILED_RES_MSG, JsonConvert.SerializeObject(ConstantSupplier.REQ_OR_DATA_NULL, Formatting.Indented)));
                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.REG_USER_SAVE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }

        /// <summary>
        /// <para>EF & ADO.NET Codeblocks: DeleteUserAsync</para>
        /// This method simply delete the user details from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DataResponse</returns>
        public async Task<DataResponse> DeleteUserAsync(string id)
        {
            DataResponse dataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DELUSER_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            try
            {
                UserInfo? oUserInfo = await _context.UserInfo.FindAsync(new Guid(id));

                if (oUserInfo != null)
                {

                    #region EF Codeblock of deleting data
                    if (_appSettings.IsUserDelate)
                    {
                        _context.UserInfo.Remove(oUserInfo);
                    }
                    else
                    {
                        UserInfo oUserInfoUp = await _context.UserInfo.FindAsync(id);
                        oUserInfoUp.IsActive = false;
                        _context.Entry(oUserInfoUp).State = EntityState.Modified;
                        _context.SaveChanges();
                    }

                    await _context.SaveChangesAsync();
                    dataResponse = new DataResponse { Success = true, Message = ConstantSupplier.DELETE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oUserInfo };
                    return dataResponse;
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
                dataResponse = new DataResponse { Success = false, Message = ConstantSupplier.DELETE_FAILED, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = id };
            }
            catch (Exception)
            {
                throw;
            }
            return dataResponse;
        }
        #endregion
    }
}
