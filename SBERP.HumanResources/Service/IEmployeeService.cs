using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IEmployeeService
    {
        Task<DataResponse> GetEmployeeInitialDataAsync();
        Task<PagingResult<EmployeeListResponse>?> GetAllEmployeesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetEmployeeByIdAsync(string id);
        Task<DataResponse> CreateUpdateEmployeeAsync(EmployeeRequest request);
        Task<DataResponse> DeleteEmployeeAsync(string id, bool hardDelete = false);
    }
}
