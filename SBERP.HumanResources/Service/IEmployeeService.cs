using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface IEmployeeService
    {
        Task<DataResponse> GetEmployeeInitialDataAsync();
        Task<DataResponse> GetAllEmployeesAsync();
        Task<PagingResult<EmployeeListResponse>?> GetAllEmployeesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetEmployeeByIdAsync(string id);
        Task<DataResponse> CreateUpdateEmployeeAsync(EmployeeRequest request);
        Task<DataResponse> DeleteEmployeeAsync(string id);
        Task<DataResponse> UploadEmployeePhotoAsync(string id, Stream content, long length, string? updatedBy);
        Task<SBERP.Shared.Storage.PhotoFile?> GetEmployeePhotoAsync(string id);
        Task<DataResponse> RemoveEmployeePhotoAsync(string id, string? updatedBy);
        Task<DataResponse> GetMyEmployeeBriefAsync(string? appUserId, string? employeeId);
    }
}
