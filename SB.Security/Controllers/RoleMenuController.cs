using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SB.Security.Filter;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
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

        #region All Http methods
        // GET api/RoleMenu/getAllRoles
      
        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns></returns>
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

        // GET api/RoleMenu/getAllMenuByUserId

        /// <summary>
        /// It used to get all user roles.
        /// </summary>
        /// <returns></returns>
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

        // GET api/RoleMenu/getAllParentMenus

        /// <summary>
        /// It used to get all parent menu list.
        /// </summary>
        /// <returns></returns>
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
