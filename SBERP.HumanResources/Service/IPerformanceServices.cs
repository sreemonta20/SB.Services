using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IAppraisalCycleService
    {
        Task<PagingResult<AppraisalCycleResponse>?> GetAllAppraisalCyclesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetAppraisalCyclesLookupAsync();
        Task<DataResponse> CreateUpdateAppraisalCycleAsync(AppraisalCycleRequest request);
        Task<DataResponse> DeleteAppraisalCycleAsync(string id);
        Task<DataResponse> SetAppraisalCycleStatusAsync(CycleActionRequest request);
        Task<DataResponse> InitiateReviewsAsync(InitiateReviewsRequest request);
    }

    public interface ICompetencyService
    {
        Task<PagingResult<CompetencyResponse>?> GetAllCompetenciesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetCompetenciesLookupAsync();
        Task<DataResponse> CreateUpdateCompetencyAsync(CompetencyRequest request);
        Task<DataResponse> DeleteCompetencyAsync(string id);
    }

    public interface IGoalService
    {
        Task<PagingResult<GoalResponse>?> GetAllGoalsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateGoalAsync(GoalRequest request);
        Task<DataResponse> UpdateGoalProgressAsync(GoalProgressRequest request);
        Task<DataResponse> DeleteGoalAsync(string id);
    }

    public interface IReviewService
    {
        Task<PagingResult<ReviewResponse>?> GetAllReviewsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetReviewDetailAsync(string id);
        Task<DataResponse> SaveReviewRatingsAsync(SaveReviewRatingsRequest request);
        Task<DataResponse> SetReviewStatusAsync(ReviewActionRequest request);
    }
}
