using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IBranchService
    {
        Task<DataResponse> GetAllBranchesAsync();
        Task<DataResponse> GetAllBranchesByCompanyIdAsync(string companyId);
        Task<PagingResult<BranchResponse>?> GetAllBranchesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetBranchByIdAsync(string id);
        Task<DataResponse> CreateUpdateBranchAsync(BranchRequest request);
        Task<DataResponse> DeleteBranchAsync(string id);
    }
}
