using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface ICompanyService
    {
        Task<DataResponse> GetAllCompaniesAsync();
        Task<PagingResult<CompanyResponse>?> GetAllCompaniesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetCompanyByIdAsync(string id);
        Task<DataResponse> CreateUpdateCompanyAsync(CompanyRequest request);
        Task<DataResponse> DeleteCompanyAsync(string id);
    }
}
