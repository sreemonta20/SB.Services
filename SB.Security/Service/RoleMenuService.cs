using Dapper;
using MailKit.Search;
using Microsoft.Data.SqlClient;
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
using System.Drawing.Printing;
using System.Net;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SB.Security.Service
{
    /// <summary>
    /// It implements <see cref="IRoleMenuService"/> all the asynchronous methods for role and menu operation incuding the user GetAllRoles, GetAllRoles using pagination, specific role by id, save or update role, and delete role. 
    /// </summary>
    public class RoleMenuService : IRoleMenuService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        /// <summary>
        /// ADO.NET Database manager
        /// </summary>
        private readonly IDatabaseManager _dbmanager;
        /// <summary>
        /// Entity Framework Database context
        /// </summary>
        private readonly SecurityDBContext _context;
        /// <summary>
        /// Dapper Database connection
        /// </summary>
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        public RoleMenuService(IConfiguration config, SecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager, IDbConnection dbConnection)
        {
            _configuration = config;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
            _context = context;
            _dbConnection = dbConnection;

        }
        #endregion

        #region AppUserRole related methods
        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllAppUserRolesAsync()
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
        public async Task<DataResponse> GetAllAppUserRolesPaginationAsync(PaginationFilter paramRequest)
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
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_ALL_ROLES_PAGINATION_FOUND, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = oAppUserRoleList };
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
        public async Task<DataResponse> GetAppUserRolesByIdAsync(string roleId)
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
        public async Task<DataResponse> CreateUpdateAppUserRoleAsync(RoleSaveUpdateRequest? roleSaveUpdateRequest)
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
                        AppUserRole oAppUserRole = new()
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
                            await _context.AppUserRoles.AddAsync(oAppUserRole);
                            await _context.SaveChangesAsync();
                            await oTrasaction.CommitAsync();
                            roleSaveUpdateRequest.Id = Convert.ToString(oAppUserRole.Id);
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

                            _context.AppUserRoles.Update(oExistAppUserRole);
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
        public async Task<DataResponse> DeleteAppUserRoleAsync(string roleId)
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
                                                                        where obj.AppUserRoleId == oExistAppUserRole.Id && obj.IsActive == oExistAppUserRole.IsActive
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
                            //_context.Entry(oAppUserRoleMenu).State = EntityState.Modified;
                            _context.SaveChangesAsync();
                        }
                        oExistAppUserRole.IsActive = false;
                        //_context.Entry(oExistAppUserRole).State = EntityState.Modified;
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

        #region All AppUserMenu related methods
        /// <summary>
        /// <para>ADO.NET Codeblock: GetAllAppUserMenuPagingWithSearchAsync</para> 
        /// This service method used to get a list of user menu with access permission based on the supplied searchterm, sortcolumnname, sortcolumndirection, page number, and page size.
        /// <br/> And retriving result as PagingResult<![CDATA[<T>]]>.
        /// </summary>
        /// <param name="paramRequest"></param>
        /// <returns>PageResult<![CDATA[<T>]]></returns>
        public async Task<PagingResult<AppUserMenu>?> GetAllAppUserMenuPagingWithSearchAsync(PagingSearchFilter paramRequest)
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
        /// It used to get all user menu and their access permission by a specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> GetAllAppUserMenuByUserIdAsync(string? userId)
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
        /// It used to create and update role based on supplied <see cref="AppUserMenuRequest"/> request model.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        public async Task<DataResponse> CreateUpdateAppUserMenuAsync(AppUserMenuRequest? request)
        {
            DataResponse? oDataResponse = null;
            AppUserMenu? oExistAppUserMenu = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVE_UPDATE_USER_MENU_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                switch (request?.ActionName)
                {
                    case ConstantSupplier.SAVE_KEY:
                        AppUserMenu oAppUserMenu = new()
                        {
                            Id = Guid.NewGuid(),
                            Name = request.Name,
                            IsHeader = request.IsHeader,
                            IsModule = request.IsModule,
                            IsComponent = request.IsComponent,
                            CssClass = request.CssClass,
                            RouteLink = request.RouteLink,
                            RouteLinkClass = request.RouteLinkClass,
                            Icon = request.Icon,
                            Remark = request.Remark,
                            ParentId = new Guid(request.ParentId),
                            DropdownIcon = request.DropdownIcon,
                            SerialNo = request.SerialNo,
                            CreatedBy = request.CreateUpdateBy,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = request.IsActive
                        };

                        oExistAppUserMenu = await _context.AppUserMenus.FirstOrDefaultAsync(um => um.Name.ToLower() == request.Name.ToLower());
                        if (Utilities.IsNotNull(oExistAppUserMenu) && !String.IsNullOrEmpty(Convert.ToString(oExistAppUserMenu.Id)))
                        {
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER_MENU_WITH_SAME_NAME, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = request };
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVE_UPDATE_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                            return oDataResponse;
                        }
                        else
                        {
                            #region EF
                            //await _context.AppUserMenus.AddAsync(oAppUserMenu);
                            //await _context.SaveChangesAsync();
                            //await oTrasaction.CommitAsync();
                            //request.Id = Convert.ToString(oAppUserMenu.Id);
                            #endregion

                            #region ADO.NET
                            List<IDbDataParameter> parameters = new()
                            {
                                _dbmanager.CreateParameter("@ActionName", ConstantSupplier.SAVE_KEY, DbType.String),
                                _dbmanager.CreateParameter("@Id", oAppUserMenu.Id, DbType.Guid),
                                _dbmanager.CreateParameter("@Name", oAppUserMenu.Name, DbType.String),
                                _dbmanager.CreateParameter("@IsHeader", oAppUserMenu.IsHeader, DbType.Boolean),
                                _dbmanager.CreateParameter("@IsModule", oAppUserMenu.IsModule, DbType.Boolean),
                                _dbmanager.CreateParameter("@IsComponent", oAppUserMenu.IsComponent, DbType.Boolean),
                                _dbmanager.CreateParameter("@CssClass", oAppUserMenu.CssClass, DbType.String),
                                _dbmanager.CreateParameter("@RouteLink", oAppUserMenu.RouteLink, DbType.String),
                                _dbmanager.CreateParameter("@RouteLinkClass", oAppUserMenu.RouteLinkClass, DbType.String),
                                _dbmanager.CreateParameter("@Icon", oAppUserMenu.Icon, DbType.String),
                                _dbmanager.CreateParameter("@Remark", oAppUserMenu.Remark, DbType.String),
                                _dbmanager.CreateParameter("@ParentId", oAppUserMenu.ParentId, DbType.Guid),
                                _dbmanager.CreateParameter("@DropdownIcon", oAppUserMenu.DropdownIcon, DbType.String),
                                _dbmanager.CreateParameter("@SerialNo", oAppUserMenu.SerialNo, DbType.String),
                                _dbmanager.CreateParameter("@DropdownIcon", oAppUserMenu.DropdownIcon, DbType.String),
                                _dbmanager.CreateParameter("@CreatedBy", oAppUserMenu.CreatedBy, DbType.String),
                                _dbmanager.CreateParameter("@CreatedDate", oAppUserMenu.CreatedDate, DbType.DateTime),
                                _dbmanager.CreateParameter("@UpdatedBy", DBNull.Value, DbType.String),
                                _dbmanager.CreateParameter("@UpdatedDate", DBNull.Value, DbType.DateTime),
                                _dbmanager.CreateParameter("@IsActive", oAppUserMenu.IsActive, DbType.Boolean)
                            };

                            int rowAffected = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_MENU_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                            request.Id = Convert.ToString(oAppUserMenu.Id);
                            if (rowAffected.Equals(0))
                            {
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_USER_MENU, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.Found, Result = request };
                                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_SAVE_UPDATE_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                                return oDataResponse;
                            }
                            #endregion

                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.USER_MENU_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.Created, Result = request };
                        }

                        break;
                    case ConstantSupplier.UPDATE_KEY:
                        oExistAppUserMenu = await _context.AppUserMenus.FindAsync(new Guid(request.Id));
                        if (Utilities.IsNull(oExistAppUserMenu))
                        {
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_USER_MENU, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = request };
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVE_UPDATE_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                            return oDataResponse;
                        }
                        else
                        {
                            #region EF
                            //oExistAppUserMenu.Name = request.Name;
                            //oExistAppUserMenu.IsHeader = request.IsHeader;
                            //oExistAppUserMenu.IsModule = request.IsModule;
                            //oExistAppUserMenu.IsComponent = request.IsComponent;
                            //oExistAppUserMenu.CssClass = request.CssClass;
                            //oExistAppUserMenu.RouteLink = request.RouteLink;
                            //oExistAppUserMenu.RouteLinkClass = request.RouteLinkClass;
                            //oExistAppUserMenu.Icon = request.Icon;
                            //oExistAppUserMenu.Remark = request.Remark;
                            //oExistAppUserMenu.ParentId = new Guid(request.ParentId);
                            //oExistAppUserMenu.DropdownIcon = request.DropdownIcon;
                            //oExistAppUserMenu.SerialNo = request.SerialNo;
                            //oExistAppUserMenu.UpdatedBy = request.CreateUpdateBy;
                            //oExistAppUserMenu.UpdatedDate = DateTime.UtcNow;
                            //oExistAppUserMenu.IsActive = request.IsActive;

                            //await _context.SaveChangesAsync();
                            //await oTrasaction.CommitAsync();
                            #endregion

                            #region ADO.NET
                            List<IDbDataParameter> parameters = new()
                            {
                                _dbmanager.CreateParameter("@ActionName", ConstantSupplier.UPDATE_KEY, DbType.String),
                                _dbmanager.CreateParameter("@Id", new Guid(request.Id), DbType.Guid),
                                _dbmanager.CreateParameter("@Name", request.Name, DbType.String),
                                _dbmanager.CreateParameter("@IsHeader", request.IsHeader, DbType.Boolean),
                                _dbmanager.CreateParameter("@IsModule", request.IsModule, DbType.Boolean),
                                _dbmanager.CreateParameter("@IsComponent", request.IsComponent, DbType.Boolean),
                                _dbmanager.CreateParameter("@CssClass", request.CssClass, DbType.String),
                                _dbmanager.CreateParameter("@RouteLink", request.RouteLink, DbType.String),
                                _dbmanager.CreateParameter("@RouteLinkClass", request.RouteLinkClass, DbType.String),
                                _dbmanager.CreateParameter("@Icon", request.Icon, DbType.String),
                                _dbmanager.CreateParameter("@Remark", request.Remark, DbType.String),
                                _dbmanager.CreateParameter("@ParentId", request.ParentId, DbType.Guid),
                                _dbmanager.CreateParameter("@DropdownIcon", request.DropdownIcon, DbType.String),
                                _dbmanager.CreateParameter("@SerialNo", request.SerialNo, DbType.String),
                                _dbmanager.CreateParameter("@DropdownIcon", request.DropdownIcon, DbType.String),
                                _dbmanager.CreateParameter("@CreatedBy", DBNull.Value, DbType.String),
                                _dbmanager.CreateParameter("@CreatedDate", DBNull.Value, DbType.DateTime),
                                _dbmanager.CreateParameter("@UpdatedBy", request.CreateUpdateBy, DbType.String),
                                _dbmanager.CreateParameter("@UpdatedDate", DateTime.UtcNow, DbType.DateTime),
                                _dbmanager.CreateParameter("@IsActive", request.IsActive, DbType.Boolean)
                            };

                            int rowAffected = await _dbmanager.InsertExecuteScalarTransAsync(ConstantSupplier.POST_SAVE_UPDATE_USER_MENU_SP_NAME, CommandType.StoredProcedure, IsolationLevel.ReadCommitted, parameters.ToArray());

                            if (rowAffected.Equals(0))
                            {
                                oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_USER_MENU, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = request };
                                _securityLogService.LogError(String.Format(ConstantSupplier.SERVICE_SAVE_UPDATE_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                                return oDataResponse;
                            }
                            #endregion

                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.USER_MENU_UPDATE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = request };
                        }

                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                oTrasaction.Rollback();
                throw;
            }
            return oDataResponse;
        }

        /// <summary>
        /// It used to delete a user menu. Delete can be happen either simply making the IsActive false or delete command. It is decided based on user settings in appsettings.json.
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns>
        /// <see cref="Task{DataResponse}"/>
        /// </returns>
        public async Task<DataResponse> DeleteAppUserMenuAsync(string menuId)
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DELETE_APP_USER_MENU_REQ_MSG, JsonConvert.SerializeObject(menuId, Formatting.Indented)));
            using IDbContextTransaction oTrasaction = _context.Database.BeginTransaction();
            try
            {
                //AppUserMenu? oExistAppUserMenu = await _context.AppUserMenus.FindAsync(new Guid(menuId));
                AppUserMenu? oExistAppUserMenu = await _context.AppUserMenus.FirstOrDefaultAsync(um => um.Id == new Guid(menuId));
                if (Utilities.IsNotNull(oExistAppUserMenu))
                {
                    if (oExistAppUserMenu.IsActive.Equals(true))
                    {
                        #region EF
                        //IEnumerable<AppUserRoleMenu> oAppUserRoleMenuList = from obj in _context?.AppUserRoleMenus
                        //                                                    where obj.AppUserMenuId == oExistAppUserMenu.Id && obj.IsActive == oExistAppUserMenu.IsActive
                        //                                                    orderby obj.CreatedDate descending
                        //                                                    select obj;
                        //if (!oAppUserRoleMenuList.Any())
                        //{
                        //    if (_appSettings.IsUserDelate)
                        //    {
                        //        _context?.AppUserMenus.Remove(oExistAppUserMenu);
                        //        await _context?.SaveChangesAsync();
                        //        await oTrasaction.CommitAsync();
                        //    }
                        //    else
                        //    {
                        //        oExistAppUserMenu.IsActive = false;
                        //        await _context.SaveChangesAsync();
                        //        await oTrasaction.CommitAsync();
                        //    }
                        //    oDataResponse = new DataResponse { Success = true, Message = _appSettings.IsUserDelate? ConstantSupplier.DELETE_APP_USER_MENU_REMOVED_SUCCESS : ConstantSupplier.DELETE_APP_USER_MENU_INACTIVATED_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = menuId };
                        //}
                        //else
                        //{
                        //    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.DELETE_APP_USER_MENU_BUT_EXIST_ROLE_MENU, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                        //    _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_DELETE_APP_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                        //    return oDataResponse;
                        //}
                        #endregion

                        #region ADO.NET
                        List<IDbDataParameter> parameters = new()
                        {
                            _dbmanager.CreateParameter("@IsDelete", _appSettings.IsUserDelate, DbType.Boolean),
                            _dbmanager.CreateParameter("@MenuId", menuId, DbType.String)
                        };
                        string result = (string)await _dbmanager.GetScalarValueAsync(ConstantSupplier.DELETE_USER_MENU_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());
                        SPResponseRequest sPResponseRequest = JsonConvert.DeserializeObject<SPResponseRequest>(result);
                        if (Utilities.IsNotNull(sPResponseRequest))
                        {
                            if (Utilities.IsNotNull(sPResponseRequest.success) && Convert.ToBoolean(sPResponseRequest.success).Equals(true))
                            {
                                oDataResponse = new DataResponse { Success = true, Message = sPResponseRequest.message, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = menuId };
                            }
                            else
                            {
                                oDataResponse = new DataResponse { Success = false, Message = sPResponseRequest.message, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                                _securityLogService.LogError(string.Format(ConstantSupplier.SERVICE_DELETE_APP_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                                return oDataResponse;
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format(ConstantSupplier.ERROR_DELETE_MSG, "Menu"));
                        }
                        #endregion
                    }
                    else
                    {
                        oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_BUT_DEACTIVATED_APP_USER_MENU, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                        _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_DELETE_APP_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                        return oDataResponse;
                    }
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_APP_USER_MENU, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_DELETE_APP_USER_MENU_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                    return oDataResponse;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
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

        #region All AppUserRoleMenu related methods
        /// <summary>
        /// This method used to get all list data, which are needed to be loaded during the user form initialization.
        /// </summary>
        /// <returns></returns>
        public async Task<DataResponse> GetAppUserRoleMenuInitialDataAsync()
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


        public async Task<PagingResult<AppUserRoleMenuResponse>?> GetAllAppUserRoleMenusPagingWithSearchAsync(PagingSearchFilter paramRequest)
        {
            try
            {
                _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
                var parameters = new
                {
                    SearchTerm = paramRequest.SearchTerm?? "",
                    SortColumnName = paramRequest.SortColumnName ?? "",
                    SortColumnDirection = paramRequest.SortColumnDirection ?? "",
                    PageIndex = paramRequest.PageNumber,
                    PageSize = paramRequest.PageSize
                };

                var results = await _dbConnection.QueryAsync<AppUserRoleMenuResponse>(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_SP_NAME, parameters,commandType: CommandType.StoredProcedure);
                if (results.Any())
                {
                    foreach (var result in results)
                    {
                        result.IsView = result.IsView ?? false;
                        result.IsCreate = result.IsCreate ?? false;
                        result.IsUpdate = result.IsUpdate ?? false;
                        result.IsDelete = result.IsDelete ?? false;
                    }
                    PagingResult<AppUserRoleMenuResponse>? oAppUserRoleMenuResult = Utilities.GetPagingResult(results.ToList(), paramRequest.PageNumber, paramRequest.PageSize);
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(oAppUserRoleMenuResult, Formatting.Indented)));
                    return oAppUserRoleMenuResult;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
    }
}
