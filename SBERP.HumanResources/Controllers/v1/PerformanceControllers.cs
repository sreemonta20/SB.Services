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
    public class AppraisalCycleController : ControllerBase
    {
        private readonly IAppraisalCycleService _service;
        private readonly IHRLogService _log;
        public AppraisalCycleController(IAppraisalCycleService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_APPRAISAL_CYCLES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAppraisalCyclesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllAppraisalCyclesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.CYCLE_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.CYCLE_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllAppraisalCyclesPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_CYCLES_LOOKUP_ROUTE)]
        public async Task<object> GetAppraisalCyclesLookupAsync()
        {
            try { return await _service.GetAppraisalCyclesLookupAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAppraisalCyclesLookupAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_APPRAISAL_CYCLE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAppraisalCycleAsync([FromBody] AppraisalCycleRequest request)
        {
            try { return await _service.CreateUpdateAppraisalCycleAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateAppraisalCycleAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_APPRAISAL_CYCLE_ROUTE)]
        public async Task<object> DeleteAppraisalCycleAsync([FromQuery] string id)
        {
            try { return await _service.DeleteAppraisalCycleAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteAppraisalCycleAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_CYCLE_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetAppraisalCycleStatusAsync([FromBody] CycleActionRequest request)
        {
            try { return await _service.SetAppraisalCycleStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetAppraisalCycleStatusAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.INITIATE_REVIEWS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> InitiateReviewsAsync([FromBody] InitiateReviewsRequest request)
        {
            try { return await _service.InitiateReviewsAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(InitiateReviewsAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class CompetencyController : ControllerBase
    {
        private readonly ICompetencyService _service;
        private readonly IHRLogService _log;
        public CompetencyController(ICompetencyService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_COMPETENCIES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllCompetenciesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllCompetenciesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.COMPETENCY_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.COMPETENCY_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCompetenciesPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_COMPETENCIES_LOOKUP_ROUTE)]
        public async Task<object> GetCompetenciesLookupAsync()
        {
            try { return await _service.GetCompetenciesLookupAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetCompetenciesLookupAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_COMPETENCY_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateCompetencyAsync([FromBody] CompetencyRequest request)
        {
            try { return await _service.CreateUpdateCompetencyAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateCompetencyAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_COMPETENCY_ROUTE)]
        public async Task<object> DeleteCompetencyAsync([FromQuery] string id)
        {
            try { return await _service.DeleteCompetencyAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteCompetencyAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _service;
        private readonly IHRLogService _log;
        public GoalController(IGoalService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_GOALS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllGoalsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllGoalsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.GOAL_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.GOAL_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllGoalsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_GOAL_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateGoalAsync([FromBody] GoalRequest request)
        {
            try { return await _service.CreateUpdateGoalAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateGoalAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.UPDATE_GOAL_PROGRESS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> UpdateGoalProgressAsync([FromBody] GoalProgressRequest request)
        {
            try { return await _service.UpdateGoalProgressAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UpdateGoalProgressAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_GOAL_ROUTE)]
        public async Task<object> DeleteGoalAsync([FromQuery] string id)
        {
            try { return await _service.DeleteGoalAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteGoalAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly IHRLogService _log;
        public ReviewController(IReviewService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_REVIEWS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllReviewsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllReviewsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.REVIEW_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.REVIEW_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllReviewsPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_REVIEW_DETAIL_ROUTE)]
        public async Task<object> GetReviewDetailAsync([FromQuery] string id)
        {
            try { return await _service.GetReviewDetailAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetReviewDetailAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SAVE_REVIEW_RATINGS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SaveReviewRatingsAsync([FromBody] SaveReviewRatingsRequest request)
        {
            try { return await _service.SaveReviewRatingsAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SaveReviewRatingsAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_REVIEW_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetReviewStatusAsync([FromBody] ReviewActionRequest request)
        {
            try { return await _service.SetReviewStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetReviewStatusAsync)); }
        }
    }
}
