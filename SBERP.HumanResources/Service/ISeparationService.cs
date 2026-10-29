using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface ISeparationService
    {
        Task<PagingResult<SeparationResponse>?> GetAllSeparationsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetSeparationDetailAsync(string id);
        Task<DataResponse> CreateUpdateSeparationAsync(SeparationRequest request);
        Task<DataResponse> DeleteSeparationAsync(string id);
        Task<DataResponse> SetSeparationStatusAsync(SeparationStatusRequest request);
        Task<DataResponse> ApproveSeparationAsync(ApproveSeparationRequest request);
        Task<DataResponse> SettleSeparationAsync(SettleSeparationRequest request);
        Task<DataResponse> SetClearanceStatusAsync(ClearanceStatusRequest request);
        Task<DataResponse> CreateUpdateExitInterviewAsync(ExitInterviewRequest request);
    }
}
