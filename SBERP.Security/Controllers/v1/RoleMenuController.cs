using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Service;
using System.Net;

namespace SBERP.Security.Controllers.v1
{
    /// <summary>
    /// This API Controller contains GetAllAppUserRoles,GetAllAppUserRolesPagination, GetAppUserRolesById, CreateUpdateAppUserRole, 
    /// DeleteAppUserRole, GetAllAppUserMenuPagingWithSearch, GetAllAppUserMenuByUserId, CreateUpdateAppUserMenu, DeleteAppUserMenu, 
    /// GetAllParentMenus, GetAppUserRoleMenuInitialData, and GetAllAppUserRoleMenusPagingWithSearch methods.
    /// </summary>
    //[ApiVersion("1.0")] // Specify the version
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
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

        #region AppUserRole related all http methods 
        // GET api/RoleMenu/getAllAppUserRoles

        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_ROLES_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppUserRolesAsync()
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALLROLES_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAllAppUserRolesAsync();
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLROLES_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLROLES_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLROLES_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/v1/RoleMenu/getAllAppUserRolesPagination

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
        public async Task<object> GetAllAppUserRolesPaginationAsync(int pageNumber = 0, int pageSize = 0)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.GETALLROLES_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAllAppUserRolesPaginationAsync(new PaginationFilter(pageNumber, pageSize));
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLROLES_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLROLES_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLROLES_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAppUserRolesById

        /// <summary>
        /// It used to get a role by roleId.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ROLE_BY_ID_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAppUserRolesByIdAsync([FromQuery] string roleId)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.GETROLEBYID_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETROLEBYID_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAppUserRolesByIdAsync(roleId);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETROLEBYID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETROLEBYID_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLROLESPAGINATION_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // POST api/RoleMenu/createUpdateAppUserRole

        /// <summary>
        /// It used to create and update role based on supplied <see cref="RoleSaveUpdateRequest"/> request model.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_SAVE_UPDATE_ROLE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUserRoleAsync(RoleSaveUpdateRequest roleSaveUpdateRequest)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.SAVEUPDATEROLE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUPDATEROLE_REQ_MSG, JsonConvert.SerializeObject(roleSaveUpdateRequest, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.CreateUpdateAppUserRoleAsync(roleSaveUpdateRequest);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVEUPDATEROLE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVEUPDATEROLE_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUPDATEROLE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/deleteAppUserRole

        /// <summary>
        /// It used to delete a role. Delete can be happen either simply making the IsActive false or delete command. It is decided based on user settings in appsettings.json.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        //[HttpGet]
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_ROLE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteAppUserRoleAsync([FromQuery] string roleId)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.DELETEROLE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DELETEROLE_REQ_MSG, JsonConvert.SerializeObject(roleId, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.DeleteAppUserRoleAsync(roleId);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.DELETEROLE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.DELETEROLE_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DELETEROLE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        #endregion

        #region AppUserMenu related all http methods
        // GET api/RoleMenu/getAllAppUserMenuPagingWithSearch 
        /// <summary>
        /// It used to get all user menu based on the search text or term.Sample param:{"SearchTerm":"Admin","SortColumnName":"","SortColumnDirection":"ASC","PageNumber":1,"PageSize":10}
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_USER_MENU_PAGING_WITH_SEARCH_TERM_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppUserMenuPagingWithSearchAsync([FromQuery] string param)
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(param, Formatting.Indented)));
            try
            {
                #region ADO.NET Codeblock
                //dynamic? paramRequest = JsonConvert.DeserializeObject(param);
                //PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(paramRequest[0].ToString());
                //dynamic? paramRequest = JsonConvert.DeserializeObject(param);
                //PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(paramRequest.ToString());
                PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(param);
                PagingResult<AppUserMenuResponse>? usermenuList = await _roleMenuService.GetAllAppUserMenuPagingWithSearchAsync(oPagingSearchFilter);
                if (Utilities.IsNull(usermenuList))
                {
                    return new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                }
                response = new DataResponse { Success = true, Message = ConstantSupplier.GET_ALL_USER_MENU_PAGING_SEARCH_RESULT_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.Found, Result = usermenuList };
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALL_USER_MENU_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAllAppUserMenuByUserId
        /// <summary>
        /// It used to get all user menu and their access permission by a specific user
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_MENU_BY_USER_ID_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppUserMenuByUserIdAsync([FromQuery] string userId)
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALLMENUBYUSERID_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLMENUBYUSERID_REQ_MSG, JsonConvert.SerializeObject(userId, Formatting.Indented)));
            try
            {
                #region EF Codeblock
                response = await _roleMenuService.GetAllAppUserMenuByUserIdAsync(userId);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLMENUBYUSERID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALLMENUBYUSERID_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALLMENUBYUSERID_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // POST api/RoleMenu/createUpdateAppUserMenu
        /// <summary>
        /// It used to create and update role based on supplied <see cref="AppUserMenuRequest"/> request model.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_SAVE_UPDATE_USER_MENU_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUserMenuAsync(AppUserMenuRequest request)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.SAVE_UPDATE_USER_MENU_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVE_UPDATE_USER_MENU_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            try
            {
                #region ADO.NET & EF Codeblock
                response = await _roleMenuService.CreateUpdateAppUserMenuAsync(request);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVE_UPDATE_USER_MENU_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVE_UPDATE_USER_MENU_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVE_UPDATE_USER_MENU_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/deleteAppUserMenu
        /// <summary>
        /// It used to delete a user menu. Delete can be happen either simply making the IsActive false or delete command. It is decided based on user settings in appsettings.json.
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns><see cref="Task{object}"/></returns>
        //[HttpGet]
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_USER_MENU_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteAppUserMenuAsync([FromQuery] string menuId)
        {
            DataResponse response;
            _securityLogService.LogInfo(ConstantSupplier.DELETE_APP_USER_MENU_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DELETE_APP_USER_MENU_REQ_MSG, JsonConvert.SerializeObject(menuId, Formatting.Indented)));
            try
            {
                #region ADO.NET & EF Codeblock
                response = await _roleMenuService.DeleteAppUserMenuAsync(menuId);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.DELETE_APP_USER_MENU_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.DELETE_APP_USER_MENU_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DELETE_APP_USER_MENU_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
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
        public async Task<object> GetAllParentMenusAsync()
        {
            _securityLogService.LogInfo(ConstantSupplier.GET_ALL_PARENT_MENUS_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_REQ_MSG, JsonConvert.SerializeObject(ConstantSupplier.NOT_APPLICABLE, Formatting.Indented)));
            try
            {
                response = await _roleMenuService.GetAllParentMenusAsync();
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_ALL_PARENT_MENUS_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        #endregion

        #region AppUserRoleMenu realted all http methods

        // GET api/RoleMenu/getAppUserRoleMenuInitialData
        /// <summary>
        /// This method used to get all list data, which are needed to be loaded during the user form initialization.
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_USER_MENU_INITIAL_DATA_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAppUserRoleMenuInitialDataAsync()
        {
            _securityLogService.LogInfo(ConstantSupplier.GETUSERMENUINITIALDATA_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETUSERMENUINITIALDATA_REQ_MSG, ConstantSupplier.NOT_APPLICABLE));
            try
            {
                #region ADO.NET Codeblock
                response = await _roleMenuService.GetAppUserRoleMenuInitialDataAsync();
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETUSERMENUINITIALDATA_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETUSERMENUINITIALDATA_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETUSERMENUINITIALDATA_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/RoleMenu/getAllAppUserRoleMenusPagingWithSearch
        /// <summary>
        /// It used to get all user menu based on the search text or term. Sample param:{"SearchTerm":"Admin","SortColumnName":"","SortColumnDirection":"ASC","PageNumber":1,"PageSize":10}
        /// </summary>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_WITH_SEARCH_TERM_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppUserRoleMenusPagingWithSearchAsync([FromQuery] string param)
        {
            _securityLogService.LogInfo(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(param, Formatting.Indented)));
            try
            {
                #region ADO.NET Codeblock
                dynamic? paramRequest = JsonConvert.DeserializeObject(param);
                PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(paramRequest.ToString());
                PagingResult<AppUserRoleMenuResponse>? appUserRoleMenuResponseList = await _roleMenuService.GetAllAppUserRoleMenusPagingWithSearchAsync(oPagingSearchFilter);
                if (Utilities.IsNull(appUserRoleMenuResponseList))
                {
                    return new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                }
                response = new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = appUserRoleMenuResponseList };
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_ALL_APP_USER_ROLE_MENU_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }
        #endregion
    }
}
