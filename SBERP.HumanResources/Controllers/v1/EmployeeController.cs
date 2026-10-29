using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Service;
using System.Net;
using System.Security.Claims;

namespace SBERP.HumanResources.Controllers.v1
{
    /// <summary>
    /// Employee CRUD. Routes match the ones registered in SBERP.Gateway/ocelot.json.
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IHRLogService _log;

        public EmployeeController(IEmployeeService employeeService, IHRLogService log)
        {
            _employeeService = employeeService;
            _log = log;
        }

        // GET api/v1/Employee/getAllEmployees — lightweight picker list
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_EMPLOYEES_ROUTE)]
        public async Task<object> GetAllEmployeesAsync()
        {
            try { return await _employeeService.GetAllEmployeesAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllEmployeesAsync)); }
        }

        // GET api/v1/Employee/getEmployeeInitialData
        [HttpGet]
        [Route(ConstantSupplier.GET_EMPLOYEE_INITIAL_DATA_ROUTE)]
        public async Task<object> GetEmployeeInitialDataAsync()
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_STARTED, nameof(GetEmployeeInitialDataAsync)));
            try
            {
                var resp = await _employeeService.GetEmployeeInitialDataAsync();
                _log.LogInfo(string.Format(ConstantSupplier.LOG_API_RES, nameof(GetEmployeeInitialDataAsync),
                    JsonConvert.SerializeObject(resp, Formatting.Indented)));
                return resp;
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeInitialDataAsync)); }
        }

        // GET api/v1/Employee/getAllEmployeesPagingWithSearch?param={...}
        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_EMPLOYEES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllEmployeesPagingWithSearchAsync([FromQuery] string param)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ,
                nameof(GetAllEmployeesPagingWithSearchAsync), param));
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param)
                             ?? new PagingSearchFilter();
                var page = await _employeeService.GetAllEmployeesPagingWithSearchAsync(filter);

                if (page == null)
                {
                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.EMPLOYEE_LIST_EMPTY,
                        MessageType = Enum.EnumResponseType.Warning,
                        ResponseCode = (int)HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.EMPLOYEE_FETCH_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.Found,
                    Result = page
                };
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllEmployeesPagingWithSearchAsync)); }
        }

        // GET api/v1/Employee/getEmployeeById
        [HttpGet]
        [Route(ConstantSupplier.GET_EMPLOYEE_BY_ID_ROUTE)]
        public async Task<object> GetEmployeeByIdAsync([FromQuery] string id)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(GetEmployeeByIdAsync), id));
            try { return await _employeeService.GetEmployeeByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeByIdAsync)); }
        }

        // POST api/v1/Employee/createEmployee
        [HttpPost]
        [Route(ConstantSupplier.CREATE_EMPLOYEE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateEmployeeAsync([FromBody] EmployeeRequest request)
        {
            request.ActionName ??= ConstantSupplier.SAVE_KEY;
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(CreateEmployeeAsync),
                JsonConvert.SerializeObject(request, Formatting.Indented)));
            try { return await _employeeService.CreateUpdateEmployeeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateEmployeeAsync)); }
        }

        // PUT api/v1/Employee/updateEmployee
        [HttpPut]
        [Route(ConstantSupplier.UPDATE_EMPLOYEE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> UpdateEmployeeAsync([FromBody] EmployeeRequest request)
        {
            request.ActionName ??= ConstantSupplier.UPDATE_KEY;
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(UpdateEmployeeAsync),
                JsonConvert.SerializeObject(request, Formatting.Indented)));
            try { return await _employeeService.CreateUpdateEmployeeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UpdateEmployeeAsync)); }
        }

        // DELETE api/v1/Employee/deleteEmployee?id={id}&hard=false
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_EMPLOYEE_ROUTE)]
        public async Task<object> DeleteEmployeeAsync([FromQuery] string id)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(DeleteEmployeeAsync), id));
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);

                return await _employeeService.DeleteEmployeeAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteEmployeeAsync)); }
        }

        // POST api/v1/Employee/uploadEmployeePhoto  (multipart/form-data: Id, File, CreateUpdateBy)
        [HttpPost]
        [Route(ConstantSupplier.UPLOAD_EMPLOYEE_PHOTO_ROUTE)]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(ConstantSupplier.EMPLOYEE_PHOTO_REQUEST_LIMIT)]
        [RequestFormLimits(MultipartBodyLengthLimit = ConstantSupplier.EMPLOYEE_PHOTO_REQUEST_LIMIT)]
        public async Task<object> UploadEmployeePhotoAsync([FromForm] EmployeePhotoRequest request)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(UploadEmployeePhotoAsync), request?.Id));
            if (request?.File == null || request.File.Length == 0)
                return Utilities.Warn(ConstantSupplier.EMPLOYEE_PHOTO_FILE_REQUIRED);
            try
            {
                await using var content = request.File.OpenReadStream();
                return await _employeeService.UploadEmployeePhotoAsync(request.Id!, content, request.File.Length, request.CreateUpdateBy);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UploadEmployeePhotoAsync)); }
        }

        // GET api/v1/Employee/getEmployeePhoto?id={id} — image/jpeg|png, or 204 when there is no photo
        [HttpGet]
        [Route(ConstantSupplier.GET_EMPLOYEE_PHOTO_ROUTE)]
        public async Task<IActionResult> GetEmployeePhotoAsync([FromQuery] string id)
        {
            var photo = await _employeeService.GetEmployeePhotoAsync(id);
            if (photo == null)
                return NoContent();
            Response.Headers.CacheControl = "private, no-cache";
            return File(photo.Content, photo.ContentType, lastModified: null,
                entityTag: new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{photo.PhotoId:N}\""));
        }

        // DELETE api/v1/Employee/removeEmployeePhoto?id={id}&updatedBy={profileId}
        [HttpDelete]
        [Route(ConstantSupplier.REMOVE_EMPLOYEE_PHOTO_ROUTE)]
        public async Task<object> RemoveEmployeePhotoAsync([FromQuery] string id, [FromQuery] string? updatedBy)
        {
            try { return await _employeeService.RemoveEmployeePhotoAsync(id, updatedBy); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(RemoveEmployeePhotoAsync)); }
        }

        // GET api/v1/Employee/getMyEmployeeBrief?employeeId={AppUserProfiles.EmployeeId}
        // Employee linked to the signed-in user (by the token's user id, else the given employee id); result null when none.
        [HttpGet]
        [Route(ConstantSupplier.GET_MY_EMPLOYEE_BRIEF_ROUTE)]
        public async Task<object> GetMyEmployeeBriefAsync([FromQuery] string? employeeId)
        {
            try
            {
                string? appUserId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return await _employeeService.GetMyEmployeeBriefAsync(appUserId, employeeId);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetMyEmployeeBriefAsync)); }
        }
    }
}
