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
using System.Security.Claims;

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
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService, ISecurityLogService securityLogService, ITokenService tokenService)
        {
            _userService = userService;
            _securityLogService = securityLogService;
            _tokenService = tokenService;
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
            _securityLogService.LogInfo(ConstantSupplier.GETBYID_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETBYID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            DataResponse response;
            try
            {
                #region EF Codeblock
                response = await _userService.GetUserByIdAsync(id);
                #endregion

                #region ADO.NET Codeblock
                //response = await _userService.GetUserByIdAdoAsync(id);
                #endregion
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.GETBYID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETBYID_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
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
            _securityLogService.LogInfo(ConstantSupplier.GETALL_STARTED_INFO_MSG);
            PaginationFilter oPaginationFilter = new() { PageNumber = pageNumber, PageSize = pageSize };
            DataResponse response;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETBYID_REQ_MSG, JsonConvert.SerializeObject(oPaginationFilter, Formatting.Indented)));
            try
            {
                
               

                #region EF Codeblock
                PageResult<UserInfo>? result = await _userService.GetAllUserAsync(oPaginationFilter);
                if ((result != null) && (result.Count > 0))
                {
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                    return response = new()
                    {
                        Success = true,
                        Message = ConstantSupplier.GET_USER_LIST_SUCCESS,
                        MessageType = Enum.EnumResponseType.Success,
                        ResponseCode = (int)HttpStatusCode.OK,
                        Result = result
                    };
                }
                #endregion

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
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.GETALL_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALL_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
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

        /// <summary>
        /// Refresh token api endpoint, which gets the user information from the expired access token and validates 
        /// the refresh token against the user. Once the validation is successful, we generate a new access token and 
        /// refresh token and the new refresh token is saved against the user in DB.
        /// </summary>
        /// <param name="refreshTokenReq"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ConstantSupplier.REFRESH_TOKEN_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> RefreshToken([FromBody] RefreshTokenRequest refreshTokenReq)
        {
            _securityLogService.LogInfo(ConstantSupplier.REFRESHTOKEN_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
            DataResponse response;

            try
            {
                if (refreshTokenReq is null)
                {
                    response = new() { Success = false, Message = ConstantSupplier.REFRESHTOKEN_INVALID_CREDENTIAL, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(String.Format(ConstantSupplier.REFRESHTOKEN_FAILED_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
                }
                else
                {
                    response = await _userService.RefreshTokenAsync(refreshTokenReq);
                }
            }
            catch (Exception Ex)
            {
                DataResponse exResponse = new()
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
                _securityLogService.LogError(String.Format(ConstantSupplier.REFRESHTOKEN_EXCEPTION_MSG, Ex.Message, JsonConvert.SerializeObject(exResponse, Formatting.Indented)));
                return exResponse;
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.REFRESHTOKEN_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        /// <summary>
        /// It invalidates the refresh token.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        //[HttpPost]
        [HttpGet]
        [Route(ConstantSupplier.REVOKE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> Revoke([FromQuery] string userToken)
        {
            _securityLogService.LogInfo(ConstantSupplier.REVOKE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.REVOKE_STARTED_INFO_MSG, JsonConvert.SerializeObject(userToken, Formatting.Indented)));
            DataResponse response;

            try
            {
                if (userToken is null)
                {
                    response = new() { Success = false, Message = ConstantSupplier.NULL_TOKEN, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.BadRequest, Result = null };
                    _securityLogService.LogError(String.Format(ConstantSupplier.REFRESHTOKEN_FAILED_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
                }
                else
                {
                    response = await _userService.RevokeAsync(userToken);
                }
            }
            catch (Exception Ex)
            {
                response = new()
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
                _securityLogService.LogError(String.Format(ConstantSupplier.REVOKE_EXCEPTION_MSG, Ex.Message, JsonConvert.SerializeObject(response, Formatting.Indented)));
                return response;
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.REVOKE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
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
            _securityLogService.LogInfo(ConstantSupplier.SAVEUP_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.RegisterUserAsync(request);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
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
            _securityLogService.LogInfo(ConstantSupplier.DELUSER_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DELUSER_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.DeleteUserAsync(id);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.DELUSER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DELUSER_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        
        #endregion
    }
}
