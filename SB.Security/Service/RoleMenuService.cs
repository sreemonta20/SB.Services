﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using SB.DataAccessLayer;
using SB.EmailService.Service;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Persistence;
using System;
using System.Collections.Immutable;
using System.Data;
using System.Net;

namespace SB.Security.Service
{
    /// <summary>
    /// includes all the asynchronous methods for role and menu operation incuding the user GetAllRoles, GetAllRoles using pagination, specific role by id, save or update role, and delete role. 
    /// It implements  <see cref="IRoleMenuService"/>.
    /// </summary>
    public class RoleMenuService : IRoleMenuService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        //private readonly SBSecurityDBContext _context;
        private readonly SecurityDBContext _context;
        private readonly IEmailService _emailService;
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
        public RoleMenuService(IConfiguration config, SecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager)
        {
            _configuration = config;
            _context = context;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
        }
        #endregion

        #region Role related methods
        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllRolesAsync()
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            
            try
            {
                List<AppUserRole>? oAppUserRoleList = await _context.AppUserRoles.OrderByDescending(x => x.RoleName).ToListAsync();
                if (Utilities.IsNull(oAppUserRoleList))
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_ROLE_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_GETALLROLES_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_ROLE_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oAppUserRoleList };
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// It used to get all user roles using pagination
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllRolesPaginationAsync(PaginationFilter paramRequest)
        {
            DataResponse? oDataResponse;
            PagingResult<AppUserRole>? oAppUserRoleList = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALLROLESPAGINATION_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            try
            {
                IQueryable<AppUserRole> oIQueryableAppUserRole = (from userRole in _context?.AppUserRoles?.OrderBy(a => a.CreatedDate) select userRole).AsQueryable();
                oAppUserRoleList = await Utilities.GetPagingResult(oIQueryableAppUserRole, paramRequest.PageNumber, paramRequest.PageSize);
                if (Utilities.IsNotNull(oAppUserRoleList) && oAppUserRoleList.Items.Any())
                {
                    oDataResponse =  new DataResponse { Success = true, Message = ConstantSupplier.GET_ALL_ROLES_PAGINATION_FOUND, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oAppUserRoleList };
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_ROLES_PAGINATION_NOT_FOUND, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = oAppUserRoleList };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETALLROLESPAGINATION_RES_MSG, JsonConvert.SerializeObject(oAppUserRoleList, Formatting.Indented)));
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// It used to get a role by roleId.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetRoleByIdAsync(string roleId)
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETROLEBYID_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            
            try
            {
                AppUserRole? oAppUserRole = await _context.AppUserRoles.FirstOrDefaultAsync(u => u.Id == new Guid(roleId) && u.IsActive == true);
                if (Utilities.IsNull(oAppUserRole))
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_APP_USER_ROLE_FAILED, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_APP_USER_ROLE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oAppUserRole };
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// It used to create and update role based on supplied <see cref="RoleSaveUpdateRequest"/> request model.
        /// </summary>
        /// <param name="roleSaveUpdateRequest"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> SaveUpdateRoleAsync(RoleSaveUpdateRequest? roleSaveUpdateRequest)
        {
            DataResponse? oDataResponse = null;
            AppUserRole? oExistAppUserRole = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUPDATEROLE_REQ_MSG, JsonConvert.SerializeObject(roleSaveUpdateRequest, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                switch (roleSaveUpdateRequest.ActionName)
                {
                    case ConstantSupplier.SAVE_KEY:
                        UserRole oUserRole = new()
                        {
                            Id = Guid.NewGuid(),
                            RoleName = roleSaveUpdateRequest.RoleName,
                            Description = roleSaveUpdateRequest.Description,
                            CreatedBy = roleSaveUpdateRequest.CreateUpdateBy,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = roleSaveUpdateRequest.IsActive
                        };

                        oExistAppUserRole = await _context.AppUserRoles.FirstOrDefaultAsync(u => u.RoleName.ToLower() == roleSaveUpdateRequest.RoleName.ToLower());
                        if (Utilities.IsNotNull(oExistAppUserRole) && !String.IsNullOrEmpty(Convert.ToString(oExistAppUserRole.Id)))
                        {
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_ROLE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = roleSaveUpdateRequest };
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUPDATEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                        }
                        else
                        {
                            await _context.AppUserRoles.AddAsync(oExistAppUserRole);
                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();
                            roleSaveUpdateRequest.Id = Convert.ToString(oExistAppUserRole.Id);
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.USER_ROLE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = roleSaveUpdateRequest };
                        }
                        
                        break;
                    case ConstantSupplier.UPDATE_KEY:
                        oExistAppUserRole = await _context.AppUserRoles.FirstOrDefaultAsync(u => u.Id == new Guid(roleSaveUpdateRequest.Id));
                        if (oExistAppUserRole != null)
                        {
                            oExistAppUserRole.RoleName = roleSaveUpdateRequest.RoleName;
                            oExistAppUserRole.Description = roleSaveUpdateRequest.Description;
                            oExistAppUserRole.UpdatedBy = roleSaveUpdateRequest.CreateUpdateBy;
                            oExistAppUserRole.UpdatedDate = DateTime.UtcNow;
                            oExistAppUserRole.IsActive = roleSaveUpdateRequest.IsActive;

                            _context.Entry(oExistAppUserRole).Property("RoleName").IsModified = true;
                            _context.Entry(oExistAppUserRole).Property("Description").IsModified = true;
                            _context.Entry(oExistAppUserRole).Property("UpdatedBy").IsModified = true;
                            _context.Entry(oExistAppUserRole).Property("UpdatedDate").IsModified = true;
                            _context.Entry(oExistAppUserRole).Property("IsActive").IsModified = true;

                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.USER_ROLE_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = roleSaveUpdateRequest };
                        }
                        else
                        {
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_ROLE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = roleSaveUpdateRequest };
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUPDATEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                        }
                        
                        break;
                    default:
                        break;
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                oTrasaction.Rollback();
                throw;
            }
            
        }

        /// <summary>
        /// It used to delete a role. Delete can be happen either simply making the IsActive false or delete command. It is decided based on user settings in appsettings.json.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> DeleteRoleAsync(string roleId)
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DELETEROLE_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                AppUserRole? oExistAppUserRole = await _context.AppUserRoles.FindAsync(new Guid(roleId));
                if (oExistAppUserRole != null)
                {
                    IEnumerable<AppUserRoleMenu> oAppUserRoleMenuList = from obj in _context?.AppUserRoleMenus
                                                                  where obj.Id == oExistAppUserRole.Id && obj.IsActive == oExistAppUserRole.IsActive
                                                                  orderby obj.CreatedDate descending
                                                                  select obj;
                    if (_appSettings.IsUserDelate)
                    {
                        _context?.AppUserRoleMenus?.RemoveRange(oAppUserRoleMenuList);
                        _context?.AppUserRoles.Remove(oExistAppUserRole);
                        await _context?.SaveChangesAsync();
                        await oTrasaction.CommitAsync();
                    }
                    else
                    {
                        foreach (var oAppUserRoleMenu in oAppUserRoleMenuList)
                        {
                            oAppUserRoleMenu.IsActive = false;
                            _context.Entry(oAppUserRoleMenu).State = EntityState.Modified;
                            //_context.SaveChanges();
                        }
                        oExistAppUserRole.IsActive = false;
                        _context.Entry(oExistAppUserRole).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        await oTrasaction.CommitAsync();
                    }
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.DELETE_ROLE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = roleId };
                    _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_DELETEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_ROLE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_DELETEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        #endregion

        #region All UserMenu related methods

        /// <summary>
        /// It used to get all user menu and their access permission by a specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllMenuByUserIdAsync(string? userId)
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALLMENUBYUSERID_REQ_MSG, JsonConvert.SerializeObject(userId, Formatting.Indented)));

            try
            {

                List<IDbDataParameter> parameters = new()
                {
                    _dbmanager.CreateParameter("@UserId", new Guid(userId), DbType.Guid)
                };

                string userMenus = (string)await _dbmanager.GetScalarValueAsync(ConstantSupplier.GET_GET_ALL_MENU_BY_USER_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (String.IsNullOrWhiteSpace(userMenus))
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_MENU_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETALLMENUBYUSERID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_MENU_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = userMenus };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }

        /// <summary>
        /// <para>ADO.NET Codeblock: GetAllUserMenuPagingWithSearchAsync</para> 
        /// This service method used to get a list of user menu with access permission based on the supplied searchterm, sortcolumnname, sortcolumndirection, page number, and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<AppUserMenu>?> GetAllUserMenuPagingWithSearchAsync(PagingSearchFilter paramRequest)
        {
            try
            {
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
                List<AppUserMenu>? oAppUserMenuList;
                List<IDbDataParameter> parameters = new()
                {
                    _dbmanager.CreateParameter("@SearchTerm", paramRequest.SearchTerm, DbType.String),
                    _dbmanager.CreateParameter("@SortColumnName", paramRequest.SortColumnName, DbType.String),
                    _dbmanager.CreateParameter("@SortColumnDirection", paramRequest.SortColumnDirection, DbType.String),
                    _dbmanager.CreateParameter("@PageIndex", paramRequest.PageNumber, DbType.Int32),
                    _dbmanager.CreateParameter("@PageSize", paramRequest.PageSize, DbType.Int32)
                };
                DataTable oDataTable = await _dbmanager.GetDataTableAsync(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (Utilities.IsNotNull(oDataTable) && oDataTable.Rows.Count > 0)
                {
                    oAppUserMenuList = Utilities.ConvertDataTable<AppUserMenu>(oDataTable);

                    PagingResult<AppUserMenu>? oAppUserMenuResult = Utilities.GetPagingResult(oAppUserMenuList, paramRequest.PageNumber, paramRequest.PageSize);
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALL_USER_MENU_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(oAppUserMenuResult, Formatting.Indented)));
                    return oAppUserMenuResult;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// This method used to get all list data, which are needed to be loaded during the user form initialization.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse> GetUserMenuInitialDataAsync()
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETUSERMENUINITIALDATA_REQ_MSG, ConstantSupplier.NOT_APPLICABLE));

            try
            {
                string userMenuMgtFormInitialData = (string)await _dbmanager.GetScalarValueAsync(ConstantSupplier.GET_USER_MENU_INITIAL_DATA_SP_NAME, CommandType.StoredProcedure);

                if (String.IsNullOrWhiteSpace(userMenuMgtFormInitialData))
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_USER_MENU_FORM_INITIAL_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETUSERMENUINITIALDATA_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                    return oDataResponse;
                }
                oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_LOAD_USER_MENU_FORM_INITIAL_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = userMenuMgtFormInitialData };
                return oDataResponse;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// It used to get all parent menu based on what child menu can be created.
        /// </summary>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllParentMenusAsync()
        {
            DataResponse? oDataResponse;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GET_ALL_PARENT_MENUS_REQ_MSG, JsonConvert.SerializeObject(ConstantSupplier.NOT_APPLICABLE, Formatting.Indented)));
            
            try
            {
                List<(Guid? Id, string? Name)>? parentMenuList = new List<(Guid? Id, string? Name)>();
                List<AppUserMenu>? oAppUserMenuList = await _context.AppUserMenus.ToListAsync();

                foreach (AppUserMenu oAppUserMenu in oAppUserMenuList)
                {
                    if (String.IsNullOrEmpty(oAppUserMenu.ParentId.ToString()))
                    {
                        parentMenuList.Add((oAppUserMenu.Id, oAppUserMenu?.Name));
                    }
                    else if (parentMenuList.Any(x => x.Id == oAppUserMenu.ParentId))
                    {
                        continue;
                    }
                }
                if (parentMenuList.IsNullOrEmpty())
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_PARENT_MENU_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GET_ALL_PARENT_MENUS_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_PARENT_MENU_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = parentMenuList };
                }
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
