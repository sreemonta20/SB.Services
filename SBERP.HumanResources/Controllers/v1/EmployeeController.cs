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

        // GET api/v1/Employee/getEmployeeById/{id}
        [HttpGet]
        [Route(ConstantSupplier.GET_EMPLOYEE_BY_ID_ROUTE)]
        public async Task<object> GetEmployeeByIdAsync(string id)
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

        // DELETE api/v1/Employee/deleteEmployee/{id}?hard=false
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_EMPLOYEE_ROUTE)]
        public async Task<object> DeleteEmployeeAsync(string id, [FromQuery] bool hard = false)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(DeleteEmployeeAsync), id));
            try { return await _employeeService.DeleteEmployeeAsync(id, hard); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteEmployeeAsync)); }
        }
    }
}
