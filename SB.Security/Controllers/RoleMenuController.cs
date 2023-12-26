using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SB.Security.Filter;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Service;
using System.Net;

namespace SB.Security.Controllers
{
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class RoleMenuController : ControllerBase
    {
        #region Variable declaration & constructor initialization
        private readonly IRoleMenuService _roleMenuService;
        private readonly ISecurityLogService _securityLogService;
        public RoleMenuController(IRoleMenuService roleMenuService, ISecurityLogService securityLogService)
        {
            _roleMenuService = roleMenuService;
            _securityLogService = securityLogService;
        }
        #endregion

        #region Role related all http methods 
        // GET api/RoleMenu/getAllRoles

        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_ROLES_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllRoles()
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALLROLES_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAllRolesAsync();
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLROLES_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLROLES_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLROLES_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAllRolesPagination

        /// <summary>
        /// It used to get all user roles using pagination
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_ROLES_PAGINATION_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllRolesPagination(int pageNo = 0, int pageSize = 0)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.GETALLROLES_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.GetAllRolesPaginationAsync(new PaginationFilter(pageNo, pageSize));
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLROLES_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLROLES_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLROLES_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getRoleById

        /// <summary>
        /// It used to get a role by roleId.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ROLE_BY_ID_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetRoleById([FromQuery] string roleId)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.GETROLEBYID_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETROLEBYID_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.GetRoleByIdAsync(roleId);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETROLEBYID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETROLEBYID_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLROLESPAGINATION_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // POST api/RoleMenu/createUpdateRole

        /// <summary>
        /// It used to create and update role based on supplied <see cref="RoleSaveUpdateRequest"/> request model.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.POST_SAVE_UPDATE_ROLE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateRole(RoleSaveUpdateRequest roleSaveUpdateRequest)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.SAVEUPDATEROLE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUPDATEROLE_REQ_MSG, JsonConvert.SerializeObject(roleSaveUpdateRequest, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.SaveUpdateRoleAsync(roleSaveUpdateRequest);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUPDATEROLE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUPDATEROLE_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUPDATEROLE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // DELETE api/RoleMenu/deleteRole

        /// <summary>
        /// It used to delete a role. Delete can be happen either simply making the IsActive false or delete command. It is decided based on user settings in appsettings.json.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.DELETE_ROLE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteRole([FromQuery] string roleId)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.DELETEROLE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DELETEROLE_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.DeleteRoleAsync(roleId);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.DELETEROLE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.DELETEROLE_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DELETEROLE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        #endregion

        #region Role Menu realted all methods
        // GET api/RoleMenu/getAllMenuByUserId

        /// <summary>
        /// It used to get all user menu and their access permission by a specific user
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_MENU_BY_USER_ID_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllMenuByUserId([FromQuery] string userId)
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALLMENUBYUSERID_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLMENUBYUSERID_REQ_MSG, JsonConvert.SerializeObject(userId, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAllMenuByUserIdAsync(userId);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLMENUBYUSERID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALLMENUBYUSERID_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALLMENUBYUSERID_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAllUserMenuPagingWithSearchTerm

        /// <summary>
        /// It used to get all user menu based on the search text or term.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_USER_MENU_PAGING_WITH_SEARCH_TERM_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllUserMenuPagingWithSearchTerm([FromQuery] string param)
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(param, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                dynamic? paramRequest = JsonConvert.DeserializeObject(param);
                PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(paramRequest[0].ToString());
                PagingResult<AppUserMenu>? usermenuList = await _roleMenuService.GetAllUserMenuPagingWithSearchAsync(oPagingSearchFilter);
                if (Utilities.IsNull(usermenuList))
                {
                    return new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                }
                response = new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = usermenuList };
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getUserMenuInitialData

        /// <summary>
        /// This method used to get all list data, which are needed to be loaded during the user form initialization.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_USER_MENU_INITIAL_DATA_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetUserMenuInitialData()
        {
            _securityLogService.LogInfo(ConstantSupplier.GETUSERMENUINITIALDATA_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETUSERMENUINITIALDATA_REQ_MSG, ConstantSupplier.NOT_APPLICABLE));
            try
            {
                #region ADO.NET Codeblock
                response = await _roleMenuService.GetUserMenuInitialDataAsync();
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GETUSERMENUINITIALDATA_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GETUSERMENUINITIALDATA_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETUSERMENUINITIALDATA_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAllParentMenus

        /// <summary>
        /// It used to get all parent menu list.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_PARENT_MENUS_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllParentMenus()
        {
            _securityLogService.LogInfo(ConstantSupplier.GET_ALL_PARENT_MENUS_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_REQ_MSG, JsonConvert.SerializeObject(ConstantSupplier.NOT_APPLICABLE, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.GetAllParentMenusAsync();
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(String.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(String.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }
        #endregion

    }
}
