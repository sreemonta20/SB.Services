using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly SBSecurityDBContext _context;
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
        public RoleMenuService(IConfiguration config, SBSecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
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
        public async Task<DataResponse> GetAllRolesAsync()
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            DataResponse? oDataResponse = null;
            try
            {
                List<UserRole> userRoleList = await _context.UserRole.OrderByDescending(x => x.RoleName).ToListAsync();
                if (userRoleList == null)
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_ROLE_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETALLROLES_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_ROLE_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = userRoleList };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }

        public async Task<PagingResult<UserRole>?> GetAllRolesPaginationAsync(PaginationFilter paramRequest)
        {
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALLROLESPAGINATION_REQ_MSG, JsonConvert.SerializeObject(paramRequest, Formatting.Indented)));
            PagingResult<UserRole>? result = null;
            try
            {
                IQueryable<UserRole> source = (from userRole in _context?.UserRole?.OrderBy(a => a.CreatedDate) select userRole).AsQueryable();
                result = await Utilities.GetPagingResult(source, paramRequest.PageNumber, paramRequest.PageSize);
                if (result == null)
                {
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETALLROLESPAGINATION_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<DataResponse> GetRoleByIdAsync(string roleId)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETROLEBYID_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            DataResponse? oDataResponse = null;
            try
            {
                UserRole? userRole = await _context.UserRole.FirstOrDefaultAsync(u => u.Id == new Guid(roleId) && u.IsActive == true);
                if (userRole == null)
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.GET_USER_FAILED, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_GETBYID_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.GET_USER_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = userRole };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }

        public async Task<DataResponse> SaveUpdateRoleAsync(RoleSaveUpdateRequest? roleSaveUpdateRequest)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_SAVEUPDATEROLE_REQ_MSG, JsonConvert.SerializeObject(roleSaveUpdateRequest, Formatting.Indented)));
            DataResponse? oDataResponse = null;
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

                        UserRole? role = await this._context.UserRole.FirstOrDefaultAsync(u => u.RoleName.ToLower() == roleSaveUpdateRequest.RoleName.ToLower());
                        if (role != null && !String.IsNullOrEmpty(Convert.ToString(role.Id)))
                        {
                            oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.EXIST_ROLE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = roleSaveUpdateRequest };
                            _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_SAVEUPDATEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                        }
                        else
                        {
                            await this._context.UserRole.AddAsync(oUserRole);
                            await this._context.SaveChangesAsync();
                            roleSaveUpdateRequest.Id = Convert.ToString(oUserRole.Id);
                            oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.USER_ROLE_SAVE_SUCCESS, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = roleSaveUpdateRequest };
                        }
                        
                        break;
                    case ConstantSupplier.UPDATE_KEY:
                        var oExistUserRole = await this._context.UserRole.FirstOrDefaultAsync(u => u.Id == new Guid(roleSaveUpdateRequest.Id));
                        if (oExistUserRole != null)
                        {
                            oExistUserRole.RoleName = roleSaveUpdateRequest.RoleName;
                            oExistUserRole.Description = roleSaveUpdateRequest.Description;
                            oExistUserRole.UpdatedBy = roleSaveUpdateRequest.CreateUpdateBy;
                            oExistUserRole.UpdatedDate = DateTime.UtcNow;
                            oExistUserRole.IsActive = roleSaveUpdateRequest.IsActive;

                            var isFullNameModified = this._context.Entry(oExistUserRole).Property("RoleName").IsModified;
                            var isUserNameModified = this._context.Entry(oExistUserRole).Property("Description").IsModified;
                            var isUpdatedByModified = this._context.Entry(oExistUserRole).Property("UpdatedBy").IsModified;
                            var isUpdatedDateModified = this._context.Entry(oExistUserRole).Property("UpdatedDate").IsModified;
                            var isIsActive = this._context.Entry(oExistUserRole).Property("IsActive").IsModified;
                            this._context.SaveChanges();
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
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }
        public async Task<DataResponse> DeleteRoleAsync(string roleId)
        {
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_DELETEROLE_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            DataResponse? oDataResponse = null;
            try
            {
                UserRole? oExistUserRole = await _context.UserRole.FindAsync(new Guid(roleId));
                if (oExistUserRole != null)
                {
                    IEnumerable<UserRoleMenu> oUserRoleMenuList = from obj in _context?.UserRoleMenu
                                                                  where obj.Id == oExistUserRole.Id
                                                                  orderby obj.CreatedDate descending
                                                                  select obj;
                    if (_appSettings.IsUserDelate)
                    {
                        _context?.UserRoleMenu?.RemoveRange(oUserRoleMenuList);
                        _context?.UserRole.Remove(oExistUserRole);
                        _context?.SaveChanges();
                    }
                    else
                    {
                        foreach (var item in oUserRoleMenuList)
                        {
                            item.IsActive = false;
                            _context.Entry(item).State = EntityState.Modified;
                            //_context.SaveChanges();
                        }
                        oExistUserRole.IsActive = false;
                        _context.Entry(oExistUserRole).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NOT_EXIST_ROLE, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(string.Format(ConstantSupplier.SERVICE_DELETEROLE_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return oDataResponse;
        }

        public async Task<DataResponse> GetAllMenuByUserIdAsync(string userId)
        {
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SERVICE_GETALLMENUBYUSERID_REQ_MSG, JsonConvert.SerializeObject(userId, Formatting.Indented)));
            DataResponse? oDataResponse;
            try
            {

                List<IDbDataParameter> parameters = new()
                {
                    _dbmanager.CreateParameter("@UserId", new Guid(userId), DbType.Guid)
                };

                string userMenus = (string)await _dbmanager.GetScalarValueAsync(ConstantSupplier.GET_GET_ALL_MENU_BY_USER_ID_SP_NAME, CommandType.StoredProcedure, parameters.ToArray());

                if (userMenus == null)
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


    }
}
