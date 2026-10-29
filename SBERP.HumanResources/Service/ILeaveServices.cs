using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    public interface ILeaveTypeService
    {
        Task<PagingResult<LeaveTypeResponse>?> GetAllLeaveTypesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetLeaveTypesLookupAsync();
        Task<DataResponse> CreateUpdateLeaveTypeAsync(LeaveTypeRequest request);
        Task<DataResponse> DeleteLeaveTypeAsync(string id);
    }

    public interface ILeavePolicyService
    {
        Task<PagingResult<LeavePolicyResponse>?> GetAllLeavePoliciesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateLeavePolicyAsync(LeavePolicyRequest request);
        Task<DataResponse> DeleteLeavePolicyAsync(string id);
        Task<DataResponse> GrantEntitlementsAsync(GrantEntitlementsRequest request);
    }

    public interface ILeaveApplicationService
    {
        Task<PagingResult<LeaveApplicationResponse>?> GetAllLeaveApplicationsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> ApplyLeaveAsync(ApplyLeaveRequest request);
        Task<DataResponse> SetLeaveApplicationStatusAsync(LeaveActionRequest request);
        Task<DataResponse> GetEmployeeLeaveBalancesAsync(string employeeId, int year);
    }
}
