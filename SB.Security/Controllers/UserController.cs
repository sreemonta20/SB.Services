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
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME)]
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

        // GET api/User/getAllUsers
        /// <summary>
        /// This method used to get a list users based on the supplied page number and page size.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>object</returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_APP_USER_PROFILE_ROUTE_NAME)]
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
                PageResult<AppUserProfile>? result = await _userService.GetAllUserAsync(oPaginationFilter);
                if ((result != null) && (result.Count > 0))
                {
                    _securityLogService.LogInfo(String.Format(ConstantSupplier.GETALL_RES_MSG, JsonConvert.SerializeObject(result, Formatting.Indented)));
                    return response = new()
                    {
                        Success = true,
                        Message = ConstantSupplier.GET_APP_USER_PROFILE_LIST_SUCCESS,
                        MessageType = Enum.EnumResponseType.Success,
                        ResponseCode = (int)HttpStatusCode.OK,
                        Result = result
                    };
                }
                #endregion

                #region ADO.NET Codeblock
                //PagingResult<AppUserProfile>? result = await _userService.GetAllUserAdoAsync(oPaginationFilter);
                //if ((result != null) && (result.RowCount > 0))
                //{
                //    return response = new()
                //    {
                //        Success = true,
                //        Message = ConstantSupplier.GET_APP_USER_PROFILE_LIST_SUCCESS,
                //        MessageType = Enum.EnumResponseType.Success,
                //        ResponseCode = (int)HttpStatusCode.OK,
                //        Result = result
                //    };
                //}
                #endregion

                response = new()
                {
                    Success = true,
                    Message = ConstantSupplier.GET_APP_USER_PROFILE_LIST_FAILED,
                    MessageType = Enum.EnumResponseType.Warning,
                    ResponseCode = (int)HttpStatusCode.BadRequest,
                    Result = result
                };

            }
            catch (Exception Ex)
            {
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

        // GET api/User/getUserbyId
        /// <summary>
        /// This method used to get a specific user details by supplying user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpGet]
        [Route(ConstantSupplier.GET_APP_USER_PROFILE_ROUTE_NAME)]
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

        // POST api/User/createUpdateAppUserProfile
        /// <summary>
        /// This method is being used for registering new user and updating old user. Except the password update during the updating details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_APP_USER_PROFILE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUserProfile(AppUserProfileRegisterRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.SAVEUP_APP_USER_PROFILE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.CreateUpdateAppUserProfileAsync(request);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_APP_USER_PROFILE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // DELETE api/User/deletAppUserProfile
        /// <summary>
        /// It deletes user details by supplying the user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>object</returns>
        [HttpDelete]
        [Route(ConstantSupplier.DEL_APP_USER_PROFILE_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DeleteAppUserProfile([FromQuery] string id)
        {
            _securityLogService.LogInfo(ConstantSupplier.DEL_APP_USER_PROFILE_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_REQ_MSG, JsonConvert.SerializeObject(id, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.DeleteAppUserProfileAsync(id);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.DEL_APP_USER_PROFILE_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        // POST api/User/createUpdateAppUser
        /// <summary>
        /// <br>This method is used to create or update application user for getting user credential to use the application. Also it creates salt key to save</br> 
        /// <br>the password with proper hasing during the creating and updating the application user.</br>
        /// </summary>
        /// <param name="request"></param>
        /// <returns>object</returns>
        [HttpPost]
        [Route(ConstantSupplier.POST_APP_USER_ROUTE_NAME)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppUser(AppUserRequest request)
        {
            _securityLogService.LogInfo(ConstantSupplier.SAVEUP_APP_USER_STARTED_INFO_MSG);
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_APP_USER_REQ_MSG, JsonConvert.SerializeObject(request, Formatting.Indented)));
            DataResponse response;
            try
            {
                response = await _userService.CreateUpdateAppUserAsync(request);
            }
            catch (Exception Ex)
            {
                //_securityLogService.LogError(Ex.Message);
                _securityLogService.LogError(String.Format(ConstantSupplier.SAVEUP_APP_USER_EXCEPTION_MSG, JsonConvert.SerializeObject(Ex.Message, Formatting.Indented)));
                return new DataResponse
                {
                    Message = Ex.Message,
                    Success = false,
                    MessageType = Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Result = null
                };
            }
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SAVEUP_APP_USER_RES_MSG, JsonConvert.SerializeObject(response, Formatting.Indented)));
            return response;
        }

        #endregion
    }
}
