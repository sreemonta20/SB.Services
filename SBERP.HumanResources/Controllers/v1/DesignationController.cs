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
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _service;
        private readonly IHRLogService _log;

        public DesignationController(IDesignationService service, IHRLogService log)
        {
            _service = service; _log = log;
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_DESIGNATIONS_ROUTE)]
        public async Task<object> GetAllDesignationsAsync()
        {
            try { return await _service.GetAllDesignationsAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDesignationsAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_DESIGNATIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllDesignationsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param)
                             ?? new PagingSearchFilter();
                var page = await _service.GetAllDesignationsPagingWithSearchAsync(filter);

                if (page == null)
                {
                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.DESIGNATION_LIST_EMPTY,
                        MessageType = Enum.EnumResponseType.Warning,
                        ResponseCode = (int)HttpStatusCode.NotFound,
                        Result = null
                    };
                }

                return new DataResponse
                {
                    Success = true,
                    Message = "Designations fetched successfully.",
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.Found,
                    Result = page
                };
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDesignationsPagingWithSearchAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_DESIGNATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateDesignationAsync([FromBody] DesignationRequest request)
        {
            try { return await _service.CreateUpdateDesignationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateDesignationAsync)); }
        }

        // DELETE api/v1/Employee/deleteDesignation?id={id}&hard=false
        [HttpDelete]
        [Route(ConstantSupplier.DELETE_DESIGNATION_ROUTE)]
        public async Task<object> DeleteDesignationAsync([FromQuery] string id, [FromQuery] bool hard = false)
        {
            _log.LogInfo(string.Format(ConstantSupplier.LOG_API_REQ, nameof(DeleteDesignationAsync), id));
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);

                return await _service.DeleteDesignationAsync(id, hard);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteDesignationAsync)); }
        }
    }
}
