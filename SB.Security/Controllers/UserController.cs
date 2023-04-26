using SB.Security.Filter;
using SB.Security.Helper;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Nodes;
using SB.Security.Models.Base;

namespace SB.Security.Controllers
{
    /// <summary>
    /// This API Controller contains specific user, all users, save or update user, and delete a user methods.
    /// </summary>
    [Authorize]
    [Route(ConstantSupplier.USER_CTRLER_ROUTE_NAME)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class UserController : ControllerBase
    {
        #region Variable declaration & constructor initialization
        private readonly IUserService _userService;
        private readonly ISecurityLogService _securityLogService;

        public UserController(IUserService userService, ISecurityLogService securityLogService)
        {
            _userService = userService;
            _securityLogService = securityLogService;
        }
        #endregion

        #region All Http methods

        // GET api/User/getUserbyId
        /// <summary>
        /// This method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetUserbyId([FromQuery] string id)
        {
            DataResponse response;
            try
            {
                response = await _userService.GetUserByIdAsync(id);
                #region ADO.NET Codeblock
                //response = await _userService.GetUserByIdAdoAsync(id);
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(Ex.Message);
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            return response;
        }

        // GET api/User/getAllUsers
        /// <summary>
        /// This method used to get a list users based on the supplied page number and page size.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>object</returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllUsers(int pageNumber, int pageSize)
        {
            DataResponse response;
            try
            {
                PaginationFilter oPaginationFilter = new() { PageNumber = pageNumber, PageSize = pageSize };
                PageResult<UserInfo>? result = await _userService.GetAllUserAsync(oPaginationFilter);
                if ((result != null) && (result.Count > 0))
                {
                    return response = new()
                    {
                        Success = true,
                        Message = ConstantSupplier.GET_USER_LIST_SUCCESS,
                        MessageType = Enum.EnumResponseType.Success,
                        ResponseCode = (int)HttpStatusCode.OK,
                        Result = result
                    };
                }
                #region ADO.NET Codeblock
                //PagingResult<UserInfo>? result = await _userService.GetAllUserAdoAsync(oPaginationFilter);
                //if ((result != null) && (result.RowCount > 0))
                //{
                //    return response = new()
                //    {
                //        Success = true,
                //        Message = ConstantSupplier.GET_USER_LIST_SUCCESS,
                //        MessageType = Enum.EnumResponseType.Success,
                //        ResponseCode = (int)HttpStatusCode.OK,
                //        Result = result
                //    };
                //}
                #endregion

                response = new()
                {
                    Success = true,
                    Message = ConstantSupplier.GET_USER_LIST_FAILED,
                    MessageType = Enum.EnumResponseType.Warning,
                    ResponseCode = (int)HttpStatusCode.BadRequest,
                    Result = result
                };

            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(Ex.Message);
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            return response;
        }

        // POST api/User/login
        /// <summary>
        /// This method authenticate user credential.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ConstantSupplier.POST_AUTH_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> Login([FromBody] LoginRequest request)
        //public async Task<object> Login([FromBody] string request)
        {
            //LoginRequest request
            _securityLogService.LogInfo(ConstantSupplier.LOGIN_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.LOGIN_REQ_MSG,JsonConvert.SerializeObject(request,Formatting.Indented)));
            DataResponse response;
            try
            {

                //var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(request);
                //response = await _userService.AuthenticateUserAsync(loginRequest);
                response = await _userService.AuthenticateUserAsync(request);
            }
            catch (Exception Ex)
            {
                var oDataResponse = new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
                _securityLogService.LogError(String.Format(ConstantSupplier.LOGIN_EXCEPTION_MSG,Ex.Message, JsonConvert.SerializeObject(oDataResponse,Formatting.Indented)));
                return oDataResponse;
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.LOGIN_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }


        // POST api/User/registerUser
        /// <summary>
        /// This method is being used for registering new user and updating old user. Except the password update during the updating details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_PUT_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> RegisterUser(UserRegisterRequest request)
        {
            DataResponse response;
            try
            {
                response = await _userService.RegisterUserAsync(request);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(Ex.Message);
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            return response;
        }

        // DELETE api/User/deleteUser
        /// <summary>
        /// It deletes user details by supplying the user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpDelete]
        [Route(ConstantSupplier.DEL_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteUser([FromQuery] string id)
        {
            DataResponse response;
            try
            {
                response = await _userService.DeleteUserAsync(id);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(Ex.Message);
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            return response;
        }
        #endregion
    }
}
