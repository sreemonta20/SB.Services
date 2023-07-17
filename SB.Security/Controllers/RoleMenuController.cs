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
        #endregion
    }
}
