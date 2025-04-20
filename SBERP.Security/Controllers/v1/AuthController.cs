using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Service;
using System.Net;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SBERP.Security.Controllers.v1
{
    /// <summary>
    /// This API Controller contains login, register, resetpassword, refreshToken, and revoke methods.
    /// </summary>
    //[ApiVersion("1.0")] // Specify the version
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
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
        
        /// POST api/v1/Auth/authenticateUser
        /// <summary>
        /// This method authenticate user credential.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="Task{object}"/></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ConstantSupplier.POST_AUTH_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> AuthenticateUserAsync([FromBody] LoginRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.LOGIN_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.LOGIN_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _authService.AuthenticateUserAsync(request);
                if (!response.Success)
                {
                    return StatusCode(response.ResponseCode, response);
                }
                return Ok(response);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Controller: Exception in AuthenticateUserAsync: {Ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.LOGIN_API_EXCEPTION_MSG,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                });
            }
        }

        /// GET api/v1/Auth/getAppUserProfileMenu
        /// <summary>
        /// This method authenticate user credential.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="Task{object}"/></returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_USER_PROFILE_MENU_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAppUserProfileMenuAsync()
        {
            _securityLogService.LogInfo(ConstantSupplier.APP_USER_PROFILE_MENU_STARTED_INFO_MSG);
            DataResponse response;
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                response = await _authService.GetAppUserProfileMenuAsync(userId!);
                if (!response.Success)
                {
                    return StatusCode(response.ResponseCode, response);
                }
                return Ok(response);
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Controller: Exception in GetAppUserProfileMenuAsync: {Ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.APP_USER_PROFILE_MENU_API_EXCEPTION_MSG,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                });
            }
        }

        /// POST api/v1/Auth/refreshToken
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
        public async Task<object> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenReq)
        {
            _securityLogService.LogInfo(ConstantSupplier.REFRESHTOKEN_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.REFRESHTOKEN_REQ_MSG, JsonConvert.SerializeObject(refreshTokenReq, Formatting.Indented)));
            DataResponse response;

            try
            {
                response = await _authService.RefreshTokenAsync(refreshTokenReq);
                if (!response.Success)
                {
                    return StatusCode(response.ResponseCode, response);
                }
                return Ok(response);

            }
            catch (Exception Ex)
            {
                _securityLogService.LogError($"Controller: Exception in RefreshTokenAsync: {Ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.REFRESH_TOKEN_API_EXCEPTION_MSG,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                });
            }
        }


        /// POST api/v1/Auth/revoke
        /// <summary>
        /// It invalidates the refresh token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="Task{object}"/></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(ConstantSupplier.REVOKE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> RevokeAsync([FromBody] RevokeRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.REVOKE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.REVOKE_STARTED_INFO_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;

            _securityLogService.LogInfo($"Revoke token request received for User: {request.UserName}");

            try
            {
                response = await _authService.RevokeAsync(request.UserName!);

                if (!response.Success)
                {
                    return StatusCode(response.ResponseCode, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _securityLogService.LogError($"Controller: Exception in RevokeAsync: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new DataResponse
                {
                    Success = false,
                    Message = ConstantSupplier.REVOKE_API_EXCEPTION_MSG,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
        #endregion
    }
}
