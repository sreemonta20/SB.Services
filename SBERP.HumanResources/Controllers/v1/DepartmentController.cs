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
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IHRLogService _log;

        public DepartmentController(IDepartmentService service, IHRLogService log)
        {
            _service = service; _log = log;
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_DEPARTMENTS_ROUTE)]
        public async Task<object> GetAllDepartmentsAsync()
        {
            try { return await _service.GetAllDepartmentsAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDepartmentsAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_DEPARTMENTS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllDepartmentsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param)
                             ?? new PagingSearchFilter();
                var page = await _service.GetAllDepartmentsPagingWithSearchAsync(filter);

                if (page == null)
                {
                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.DEPARTMENT_LIST_EMPTY,
                        MessageType = Enum.EnumResponseType.Warning,
                        ResponseCode = (int)HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.DEPARTMENT_FETCH_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.Found,
                    Result = page
                };
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDepartmentsPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_DEPARTMENT_BY_ID_ROUTE)]
        public async Task<object> GetDepartmentByIdAsync([FromQuery] string id)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(GetDepartmentByIdAsync), id));
            try { return await _service.GetDepartmentByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetDepartmentByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_DEPARTMENT_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateDepartmentAsync([FromBody] DepartmentRequest request)
        {
            try { return await _service.CreateUpdateDepartmentAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateDepartmentAsync)); }
        }

        // DELETE api/v1/Employee/deleteDepartment?id={id}&hard=false
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_DEPARTMENT_ROUTE)]
        public async Task<object> DeleteDepartmentAsync([FromQuery] string id)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(DeleteDepartmentAsync), id));
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);

                return await _service.DeleteDepartmentAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteDepartmentAsync)); }
        }
    }
}
