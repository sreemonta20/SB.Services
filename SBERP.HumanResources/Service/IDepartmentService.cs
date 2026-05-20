using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IDepartmentService
    {
        Task<DataResponse> GetAllDepartmentsAsync();
        Task<PagingResult<DepartmentResponse>?> GetAllDepartmentsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetDepartmentByIdAsync(string id);
        Task<DataResponse> CreateUpdateDepartmentAsync(DepartmentRequest request);
        Task<DataResponse> DeleteDepartmentAsync(string id, bool hardDelete = false);
    }
}
