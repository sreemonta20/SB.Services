using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Service;
using System.Net;

namespace SBERP.HumanResources.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class SeparationController : ControllerBase
    {
        private readonly ISeparationService _service;
        private readonly IHRLogService _log;
        public SeparationController(ISeparationService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_SEPARATIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllSeparationsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllSeparationsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.SEPARATION_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.SEPARATION_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllSeparationsPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_SEPARATION_DETAIL_ROUTE)]
        public async Task<object> GetSeparationDetailAsync([FromQuery] string id)
        {
            try { return await _service.GetSeparationDetailAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetSeparationDetailAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_SEPARATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateSeparationAsync([FromBody] SeparationRequest request)
        {
            try { return await _service.CreateUpdateSeparationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateSeparationAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_SEPARATION_ROUTE)]
        public async Task<object> DeleteSeparationAsync([FromQuery] string id)
        {
            try { return await _service.DeleteSeparationAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteSeparationAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_SEPARATION_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetSeparationStatusAsync([FromBody] SeparationStatusRequest request)
        {
            try { return await _service.SetSeparationStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetSeparationStatusAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.APPROVE_SEPARATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> ApproveSeparationAsync([FromBody] ApproveSeparationRequest request)
        {
            try { return await _service.ApproveSeparationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApproveSeparationAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SETTLE_SEPARATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SettleSeparationAsync([FromBody] SettleSeparationRequest request)
        {
            try { return await _service.SettleSeparationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SettleSeparationAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_CLEARANCE_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetClearanceStatusAsync([FromBody] ClearanceStatusRequest request)
        {
            try { return await _service.SetClearanceStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetClearanceStatusAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_EXIT_INTERVIEW_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateExitInterviewAsync([FromBody] ExitInterviewRequest request)
        {
            try { return await _service.CreateUpdateExitInterviewAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateExitInterviewAsync)); }
        }
    }
}
