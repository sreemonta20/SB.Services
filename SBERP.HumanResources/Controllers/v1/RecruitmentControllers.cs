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
using SBERP.Shared.Authorization;
using System.Net;

namespace SBERP.HumanResources.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Position)]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;
        private readonly IHRLogService _log;
        public PositionController(IPositionService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_POSITIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllPositionsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllPositionsPagingWithSearchAsync(filter);
                if (page == null)
                    return Utilities.Warn(ConstantSupplier.POSITION_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllPositionsPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route("getAllPositions")]
        [AuthenticatedOnly]
        public async Task<object> GetAllPositionsAsync()
        {
            try { return await _service.GetAllPositionsAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllPositionsAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_POSITION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdatePositionAsync([FromBody] PositionRequest request)
        {
            try { return await _service.CreateUpdatePositionAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdatePositionAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_POSITION_ROUTE)]
        public async Task<object> DeletePositionAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeletePositionAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeletePositionAsync)); }
        }

        /// <summary>+1 assign / -1 release a seat (Onboarding / Separation call the SP directly).</summary>
        [HttpPost]
        [Route(ConstantSupplier.SET_POSITION_FILL_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Position, Action = ScreenAction.Update)]
        public async Task<object> SetPositionFillAsync([FromBody] PositionFillRequest request)
        {
            try { return await _service.SetFillAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetPositionFillAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SET_POSITION_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Position, Action = ScreenAction.Update)]
        public async Task<object> SetPositionStatusAsync([FromBody] PositionStatusRequest request)
        {
            try { return await _service.SetStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetPositionStatusAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_POSITION_ORG_TREE_ROUTE)]
        public async Task<object> GetPositionOrgTreeAsync([FromQuery] string? rootPositionId)
        {
            try { return await _service.GetOrgTreeAsync(rootPositionId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPositionOrgTreeAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Requisition)]
    public class RequisitionController : ControllerBase
    {
        private readonly IRequisitionService _service;
        private readonly IHRLogService _log;
        public RequisitionController(IRequisitionService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_REQUISITIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Requisition, Screens.JobPosting)]
        public async Task<object> GetAllRequisitionsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllRequisitionsPagingWithSearchAsync(filter);
                if (page == null)
                    return Utilities.Warn(ConstantSupplier.REQUISITION_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.REQUISITION_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllRequisitionsPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_REQUISITION_BY_ID_ROUTE)]
        public async Task<object> GetRequisitionByIdAsync([FromQuery] string id)
        {
            try { return await _service.GetRequisitionByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetRequisitionByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_REQUISITION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateRequisitionAsync([FromBody] RequisitionRequest request)
        {
            try { return await _service.CreateUpdateRequisitionAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateRequisitionAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SUBMIT_REQUISITION_ROUTE)]
        [RequireScreen(Screens.Requisition, Action = ScreenAction.Update)]
        public async Task<object> SubmitRequisitionAsync([FromBody] RequisitionActionRequest request)
        {
            try { return await _service.SubmitRequisitionAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SubmitRequisitionAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.APPROVE_REQUISITION_ROUTE)]
        [RequireScreen(Screens.Requisition, Action = ScreenAction.Update)]
        public async Task<object> ApproveRequisitionAsync([FromBody] RequisitionActionRequest request)
        {
            try { return await _service.ApproveRequisitionAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApproveRequisitionAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.REJECT_REQUISITION_ROUTE)]
        [RequireScreen(Screens.Requisition, Action = ScreenAction.Update)]
        public async Task<object> RejectRequisitionAsync([FromBody] RequisitionActionRequest request)
        {
            try { return await _service.RejectRequisitionAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(RejectRequisitionAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.JobPosting)]
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostingService _service;
        private readonly IHRLogService _log;
        public JobPostingController(IJobPostingService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_JOBPOSTINGS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.JobPosting, Screens.Pipeline)]
        public async Task<object> GetAllJobPostingsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllJobPostingsPagingWithSearchAsync(filter);
                if (page == null)
                    return Utilities.Warn(ConstantSupplier.JOBPOSTING_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.JOBPOSTING_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllJobPostingsPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_JOBPOSTING_BY_ID_ROUTE)]
        public async Task<object> GetJobPostingByIdAsync([FromQuery] string id)
        {
            try { return await _service.GetJobPostingByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetJobPostingByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_JOBPOSTING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateJobPostingAsync([FromBody] JobPostingRequest request)
        {
            try { return await _service.CreateUpdateJobPostingAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateJobPostingAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.PUBLISH_JOBPOSTING_ROUTE)]
        [RequireScreen(Screens.JobPosting, Action = ScreenAction.Update)]
        public async Task<object> PublishJobPostingAsync([FromQuery] string id, [FromQuery] string? user)
        {
            try { return await _service.PublishJobPostingAsync(id, user); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(PublishJobPostingAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CLOSE_JOBPOSTING_ROUTE)]
        [RequireScreen(Screens.JobPosting, Action = ScreenAction.Update)]
        public async Task<object> CloseJobPostingAsync([FromQuery] string id, [FromQuery] string? user)
        {
            try { return await _service.CloseJobPostingAsync(id, user); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CloseJobPostingAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_APPLICATIONS_BY_POSTING_ROUTE)]
        [RequireScreen(Screens.JobPosting, Screens.Pipeline)]
        public async Task<object> GetApplicationsByPostingAsync([FromQuery] string id)
        {
            try { return await _service.GetApplicationsByPostingAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetApplicationsByPostingAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Pipeline)]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _service;
        private readonly IHRLogService _log;
        public CandidateController(ICandidateService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_CANDIDATES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllCandidatesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllCandidatesPagingWithSearchAsync(filter);
                if (page == null)
                    return Utilities.Warn(ConstantSupplier.CANDIDATE_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.CANDIDATE_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCandidatesPagingWithSearchAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_CANDIDATE_APPLICATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Pipeline, Action = ScreenAction.Create)]
        public async Task<object> ApplyToPostingAsync([FromBody] CandidateApplicationRequest request)
        {
            try { return await _service.ApplyToPostingAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApplyToPostingAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.MOVE_APPLICATION_STAGE_ROUTE)]
        [RequireScreen(Screens.Pipeline, Action = ScreenAction.Update)]
        public async Task<object> MoveApplicationStageAsync([FromBody] MoveStageRequest request)
        {
            try { return await _service.MoveApplicationStageAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(MoveApplicationStageAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Pipeline)]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _service;
        private readonly IHRLogService _log;
        public InterviewController(IInterviewService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_INTERVIEWS_BY_APPLICATION_ROUTE)]
        public async Task<object> GetInterviewsByApplicationAsync([FromQuery] string id)
        {
            try { return await _service.GetInterviewsByApplicationAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetInterviewsByApplicationAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_INTERVIEW_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateInterviewAsync([FromBody] InterviewRequest request)
        {
            try { return await _service.CreateUpdateInterviewAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateInterviewAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Pipeline)]
    public class JobOfferController : ControllerBase
    {
        private readonly IJobOfferService _service;
        private readonly IHRLogService _log;
        public JobOfferController(IJobOfferService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_OFFER_BY_ID_ROUTE)]
        public async Task<object> GetOfferByIdAsync([FromQuery] string id)
        {
            try { return await _service.GetOfferByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetOfferByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_OFFER_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateOfferAsync([FromBody] OfferRequest request)
        {
            try { return await _service.CreateUpdateOfferAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateOfferAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.ISSUE_OFFER_ROUTE)]
        [RequireScreen(Screens.Pipeline, Action = ScreenAction.Update)]
        public async Task<object> IssueOfferAsync([FromBody] OfferActionRequest request)
        {
            try { return await _service.IssueOfferAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(IssueOfferAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.ACCEPT_OFFER_ROUTE)]
        [RequireScreen(Screens.Pipeline, Action = ScreenAction.Update)]
        public async Task<object> AcceptOfferAsync([FromBody] OfferActionRequest request)
        {
            try { return await _service.AcceptOfferAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(AcceptOfferAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.DECLINE_OFFER_ROUTE)]
        [RequireScreen(Screens.Pipeline, Action = ScreenAction.Update)]
        public async Task<object> DeclineOfferAsync([FromBody] OfferActionRequest request)
        {
            try { return await _service.DeclineOfferAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeclineOfferAsync)); }
        }
    }
}
