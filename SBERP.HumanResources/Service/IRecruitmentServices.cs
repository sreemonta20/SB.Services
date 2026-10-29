using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    // =========================================================================
    // Recruitment service contracts. Same split as IDepartmentService:
    //  - GetAll*PagingWithSearch -> PagingResult<T>? (JSON SP)
    //  - everything else -> DataResponse (EF CRUD / workflow)
    // =========================================================================

    public interface IPositionService
    {
        Task<PagingResult<PositionResponse>?> GetAllPositionsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetAllPositionsAsync();
        Task<DataResponse> CreateUpdatePositionAsync(PositionRequest request);
        Task<DataResponse> DeletePositionAsync(string id);
        Task<DataResponse> SetFillAsync(PositionFillRequest request);
        Task<DataResponse> SetStatusAsync(PositionStatusRequest request);
        Task<DataResponse> GetOrgTreeAsync(string? rootPositionId);
    }

    public interface IRequisitionService
    {
        Task<PagingResult<RequisitionResponse>?> GetAllRequisitionsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetRequisitionByIdAsync(string id);
        Task<DataResponse> CreateUpdateRequisitionAsync(RequisitionRequest request);
        Task<DataResponse> SubmitRequisitionAsync(RequisitionActionRequest request);
        Task<DataResponse> ApproveRequisitionAsync(RequisitionActionRequest request);
        Task<DataResponse> RejectRequisitionAsync(RequisitionActionRequest request);
    }

    public interface IJobPostingService
    {
        Task<PagingResult<JobPostingResponse>?> GetAllJobPostingsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetJobPostingByIdAsync(string id);
        Task<DataResponse> CreateUpdateJobPostingAsync(JobPostingRequest request);
        Task<DataResponse> PublishJobPostingAsync(string id, string? user);
        Task<DataResponse> CloseJobPostingAsync(string id, string? user);
        Task<DataResponse> GetApplicationsByPostingAsync(string jobPostingId);
    }

    public interface ICandidateService
    {
        Task<PagingResult<CandidateResponse>?> GetAllCandidatesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> ApplyToPostingAsync(CandidateApplicationRequest request);
        Task<DataResponse> MoveApplicationStageAsync(MoveStageRequest request);
    }

    public interface IInterviewService
    {
        Task<DataResponse> GetInterviewsByApplicationAsync(string jobApplicationId);
        Task<DataResponse> CreateUpdateInterviewAsync(InterviewRequest request);
    }

    public interface IJobOfferService
    {
        Task<DataResponse> GetOfferByIdAsync(string id);
        Task<DataResponse> CreateUpdateOfferAsync(OfferRequest request);
        Task<DataResponse> IssueOfferAsync(OfferActionRequest request);
        Task<DataResponse> AcceptOfferAsync(OfferActionRequest request);
        Task<DataResponse> DeclineOfferAsync(OfferActionRequest request);
    }
}
