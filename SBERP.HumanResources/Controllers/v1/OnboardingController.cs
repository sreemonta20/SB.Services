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
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _service;
        private readonly IHRLogService _log;
        public OnboardingController(IOnboardingService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_ONBOARDINGS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllOnboardingsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllOnboardingsPagingWithSearchAsync(filter);
                if (page == null)
                    return Utilities.Warn(ConstantSupplier.ONBOARDING_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ONBOARDING_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllOnboardingsPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ACCEPTED_OFFERS_ROUTE)]
        public async Task<object> GetAcceptedOffersAwaitingOnboardingAsync()
        {
            try { return await _service.GetAcceptedOffersAwaitingOnboardingAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAcceptedOffersAwaitingOnboardingAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.START_ONBOARDING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> StartOnboardingAsync([FromBody] StartOnboardingRequest request)
        {
            try { return await _service.StartOnboardingAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(StartOnboardingAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ONBOARDING_TASKS_ROUTE)]
        public async Task<object> GetOnboardingTasksAsync([FromQuery] string id)
        {
            try { return await _service.GetOnboardingTasksAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetOnboardingTasksAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.UPDATE_ONBOARDING_TASK_ROUTE)]
        public async Task<object> UpdateOnboardingTaskAsync([FromBody] UpdateOnboardingTaskRequest request)
        {
            try { return await _service.UpdateOnboardingTaskAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UpdateOnboardingTaskAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.ADD_ONBOARDING_TASK_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> AddOnboardingTaskAsync([FromBody] AddOnboardingTaskRequest request)
        {
            try { return await _service.AddOnboardingTaskAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(AddOnboardingTaskAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.COMPLETE_ONBOARDING_ROUTE)]
        public async Task<object> CompleteOnboardingAsync([FromBody] OnboardingActionRequest request)
        {
            try { return await _service.CompleteOnboardingAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CompleteOnboardingAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CANCEL_ONBOARDING_ROUTE)]
        public async Task<object> CancelOnboardingAsync([FromBody] OnboardingActionRequest request)
        {
            try { return await _service.CancelOnboardingAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CancelOnboardingAsync)); }
        }
    }
}
