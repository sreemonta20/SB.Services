using Newtonsoft.Json;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;

namespace SBERP.HumanResources.Service
{
    /// <summary>
    /// Leave applications + balances. The balance-moving operations (apply,
    /// approve/reject/cancel) all run through transactional SPs so the
    /// LeaveBalances ledger never drifts. Reads use the JSON-SP pattern.
    /// </summary>
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public LeaveApplicationService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<LeaveApplicationResponse>?> GetAllLeaveApplicationsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<LeaveApplicationResponse>(_ctx, ConstantSupplier.SP_GET_ALL_LEAVE_APPLICATIONS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllLeaveApplicationsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> ApplyLeaveAsync(ApplyLeaveRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                if (!Guid.TryParse(request.LeaveTypeId, out var typeId)) return Utilities.Warn("Invalid leave type id");

                var (rows, msg) = await LeaveAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_APPLY_LEAVE,
                    ("@EmployeeId", empId), ("@LeaveTypeId", typeId),
                    ("@FromDate", request.FromDate), ("@ToDate", request.ToDate),
                    ("@IsHalfDay", request.IsHalfDay),
                    ("@Reason", request.Reason), ("@AttachmentUrl", request.AttachmentUrl),
                    ("@ContactDuringLeave", request.ContactDuringLeave), ("@CreatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApplyLeaveAsync)); }
        }

        public async Task<DataResponse> SetLeaveApplicationStatusAsync(LeaveActionRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid application id");
                var (rows, msg) = await LeaveAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_LEAVE_APPLICATION_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId),
                    ("@ApproverId", Guid.TryParse(request.ApproverId, out var ap) ? ap : (object?)null),
                    ("@ApproverRemarks", request.ApproverRemarks), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetLeaveApplicationStatusAsync)); }
        }

        public async Task<DataResponse> GetEmployeeLeaveBalancesAsync(string employeeId, int year)
        {
            try
            {
                if (!Guid.TryParse(employeeId, out var g)) return Utilities.Warn("Invalid employee id");
                var json = await LeaveAdo.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_EMPLOYEE_LEAVE_BALANCES,
                    ("@EmployeeId", g), ("@Year", year));
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<LeaveBalanceResponse>()
                    : JsonConvert.DeserializeObject<List<LeaveBalanceResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.LEAVE_BALANCE_FETCH, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeLeaveBalancesAsync)); }
        }
    }
}
