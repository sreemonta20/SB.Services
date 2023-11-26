using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SB.Security.Filter;
using SB.Security.Helper;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Service;
using System.Net;

namespace SB.Security.Controllers
{
    /// <summary>
    /// This API Controller contains specific user, all users, save or update user, and delete a user methods.
    /// </summary>
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class AuthController : ControllerBase
    {
        #region Variable declaration & constructor initialization
        private readonly IAuthService _authService;
        private readonly ISecurityLogService _securityLogService;
        private readonly ITokenService _tokenService;
        public AuthController(IAuthService authService, ISecurityLogService securityLogService, ITokenService tokenService)
        {
            _authService = authService;
            _securityLogService = securityLogService;
            _tokenService = tokenService;
        }
        #endregion

        #region All authentication related methods
        /// POST api/User/login
        /// <summary>
        /// This method authenticate user credential.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="Task{object}"/></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ConstantSupplier.POST_AUTH_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> Login([FromBody] LoginRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.LOGIN_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _authService.AuthenticateUserAsync(request);
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
                _securityLogService.LogError(String.Format(ConstantSupplier.LOGIN_EXCEPTION_MSG, Ex.Message, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));
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
        /// <returns><see cref="Task{object}"/></returns>
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
                    response = await _authService.RefreshTokenAsync(refreshTokenReq);
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
        /// <returns><see cref="Task{object}"/></returns>
        [AllowAnonymous]
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
                    response = await _authService.RevokeAsync(userToken);
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
        #endregion
    }
}
