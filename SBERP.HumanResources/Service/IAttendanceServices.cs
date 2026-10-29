using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Service;

namespace SBERP.HumanResources.Service
{
    public interface IShiftService
    {
        Task<PagingResult<ShiftResponse>?> GetAllShiftsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateShiftAsync(ShiftRequest request);
        Task<DataResponse> DeleteShiftAsync(string id);
    }

    public interface IRosterService
    {
        Task<DataResponse> CreateUpdateRosterAsync(ShiftRosterRequest request);
        Task<DataResponse> GetEmployeeRosterAsync(string employeeId);
        Task<DataResponse> DeleteRosterAsync(string id);
    }

    public interface IOvertimeService
    {
        Task<PagingResult<OvertimeResponse>?> GetAllOvertimePagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateOvertimeAsync(OvertimeRequestRequest request);
        Task<DataResponse> SetOvertimeStatusAsync(AttendanceApprovalRequest request);
    }

    public interface IRegularizationService
    {
        Task<PagingResult<RegularizationResponse>?> GetAllRegularizationsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateRegularizationAsync(RegularizationRequestRequest request);
        Task<DataResponse> SetRegularizationStatusAsync(AttendanceApprovalRequest request);
    }

    public interface IAttendanceSummaryService
    {
        Task<DataResponse> GetMonthlyAttendanceSummaryAsync(DateTime month, string? departmentId, string? employeeId);
    }
}
