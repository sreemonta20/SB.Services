using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IDesignationService
    {
        Task<DataResponse> GetAllDesignationsAsync();
        Task<PagingResult<DesignationResponse>?> GetAllDesignationsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateDesignationAsync(DesignationRequest request);
        Task<DataResponse> DeleteDesignationAsync(string id);
    }
}
