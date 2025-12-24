using SBERP.Security.Filter;
using SBERP.Security.Helper;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using SBERP.Security.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Nodes;
using SBERP.Security.Models.Base;
using System.Security.Claims;

namespace SBERP.Security.Controllers.v1
{
    /// <summary>
    /// This API Controller contains specific create and update user, get all user profile, get all user profile by id, save or update user profile, and delete user profile methods.
    /// </summary>
    //[ApiVersion("1.0")] // Specify the version
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
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

        #region AppUser related all http methods
        // POST api/v1/User/createUpdateAppUser
        /// <summary>
        /// <br>This method is used to create or update application user for getting user credential to use the application. Also it creates salt key to save</br> 
        /// <br>the password with proper hasing during the creating and updating the application user.</br>
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_APP_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUserAsync(AppUserRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.SAVEUP_APP_USER_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUP_APP_USER_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.CreateUpdateAppUserAsync(request);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVEUP_APP_USER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUP_APP_USER_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }
        #endregion

        #region AppUserProfile related all http methods
        // GET api/v1/RoleMenu/getAllAppUserProfilePagingWithSearch
        /// <summary>
        /// It used to get all user profile based on the search text or term.Sample param:{"SearchTerm":"Admin","SortColumnName":"","SortColumnDirection":"ASC","PageNumber":1,"PageSize":10}
        /// </summary>
        /// <param name="param"></param>
        /// <returns>
        /// <see cref="Task{object}"/>
        /// </returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_USER_PROFILE_PAGING_WITH_SEARCH_TERM_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppUserProfilePagingWithSearchAsync([FromQuery] string param)
        {
            _securityLogService.LogInfo(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_STARTED_INFO_MSG);
            DataResponse response;
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_REQ_MSG, JsonConvert.SerializeObject(param, Formatting.Indented)));
            try
            {
                #region ADO.NET Codeblock
                PagingSearchFilter? oPagingSearchFilter = JsonConvert.DeserializeObject<PagingSearchFilter>(param);
                PagingResult<AppUserProfileResponse>? userprofileList = await _userService.GetAllAppUserProfilePagingWithSearchAsync(oPagingSearchFilter);
                if (Utilities.IsNull(userprofileList))
                {
                    return new DataResponse { Success = false, Message = ConstantSupplier.GET_ALL_USER_PROFILE_PAGING_SEARCH_RESULT_EMPTY_MSG, MessageType = Enum.EnumResponseType.Error, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                }
                response = new DataResponse { Success = true, Message = ConstantSupplier.GET_ALL_USER_PROFILE_PAGING_SEARCH_RESULT_SUCCESS_MSG, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.Found, Result = userprofileList };
                #endregion
            }
            catch (Exception Ex)
            {
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Success = false,
                    Message = Ex.Message,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GETALL_USER_PROFILE_PAGING_SEARCH_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // GET api/v1/User/getAppUserProfileById
        /// <summary>
        /// This method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_APP_USER_PROFILE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAppUserProfileByIdAsync([FromQuery] string id)
        {
            _securityLogService.LogInfo(ConstantSupplier.GET_USER_PROFILE_BY_ID_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_USER_PROFILE_BY_ID_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            DataResponse response;
            try
            {
                #region EF Codeblock
                response = await _userService.GetAppUserProfileByIdAsync(id);
                #endregion

                #region ADO.NET Codeblock
                //response = await _userService.GetAppUserProfileByIdAdoAsync(id);
                #endregion
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_USER_PROFILE_BY_ID_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.GET_USER_PROFILE_BY_ID_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.GET_USER_PROFILE_BY_ID_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // POST api/v1/User/createUpdateAppUserProfile
        /// <summary>
        /// This method is being used for registering new user and updating old user. Except the password update during the updating details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_APP_USER_PROFILE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUserProfileAsync(AppUserProfileRegisterRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.SAVEUP_APP_USER_PROFILE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.CreateUpdateAppUserProfileAsync(request);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                _securityLogService.LogError(string.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_INNER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // DELETE api/v1/User/deleteAppUserProfile
        /// <summary>
        /// It deletes user details by supplying the user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpDelete]
        [Route(ConstantSupplier.DEL_APP_USER_PROFILE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteAppUserProfileAsync([FromQuery] string id)
        {
            _securityLogService.LogInfo(ConstantSupplier.DEL_APP_USER_PROFILE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DEL_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.DeleteAppUserProfileAsync(id);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(string.Format(ConstantSupplier.DEL_APP_USER_PROFILE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(string.Format(ConstantSupplier.DEL_APP_USER_PROFILE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }
        #endregion
    }
}
