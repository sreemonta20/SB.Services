using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Net;

namespace SBERP.HumanResources.Service
{
    public class PayrollRunService : IPayrollRunService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public PayrollRunService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<PayrollRunResponse>?> GetAllPayrollRunsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<PayrollRunResponse>(_ctx, ConstantSupplier.SP_GET_ALL_PAYROLL_RUNS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllPayrollRunsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> ProcessPayrollRunAsync(ProcessPayrollRequest request)
        {
            try
            {
                var (rows, msg, runId) = await AdoHelper.StatusSpWithExtraAsync(_ctx, ConstantSupplier.SP_PROCESS_PAYROLL_RUN, "RunId",
                    ("@PayMonth", request.PayMonth), ("@PayYear", request.PayYear),
                    ("@PeriodStart", request.PeriodStart), ("@PeriodEnd", request.PeriodEnd),
                    ("@WorkingDays", request.WorkingDays), ("@CreatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, new { runId }) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ProcessPayrollRunAsync)); }
        }

        public async Task<DataResponse> SetPayrollRunStatusAsync(PayrollRunActionRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid run id");
                var (rows, msg, _) = await AdoHelper.StatusSpWithExtraAsync(_ctx, ConstantSupplier.SP_SET_PAYROLL_RUN_STATUS, null,
                    ("@Id", id), ("@StatusId", request.StatusId), ("@ActionBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetPayrollRunStatusAsync)); }
        }

        public async Task<DataResponse> GetPayslipsByRunAsync(string runId)
        {
            try
            {
                if (!Guid.TryParse(runId, out var g)) return Utilities.Warn("Invalid run id");
                var json = await AdoHelper.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_PAYSLIPS_BY_RUN, ("@RunId", g));
                var items = string.IsNullOrWhiteSpace(json) ? new List<object>() : JsonConvert.DeserializeObject<List<object>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.PAYSLIP_FETCH, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayslipsByRunAsync)); }
        }

        public async Task<DataResponse> GetPayslipDetailAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid payslip id");
                var json = await AdoHelper.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_PAYSLIP_DETAIL, ("@Id", g));
                if (string.IsNullOrWhiteSpace(json)) return Utilities.Warn("Payslip not found", code: (int)HttpStatusCode.NotFound);
                var obj = JsonConvert.DeserializeObject<object>(json!);
                return Utilities.Ok(ConstantSupplier.PAYSLIP_FETCH, obj);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayslipDetailAsync)); }
        }
    }

    public class LoanService : ILoanService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public LoanService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<LoanResponse>?> GetAllLoansPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<LoanResponse>(_ctx, ConstantSupplier.SP_GET_ALL_LOANS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllLoansPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateLoanAsync(LoanRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                if (request.InstallmentAmount <= 0 || request.TotalInstallments <= 0) return Utilities.Warn("Installment amount and count must be positive.");

                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid loan id");
                    var l = await _ctx.Loans!.FirstOrDefaultAsync(x => x.Id == id);
                    if (l == null) return Utilities.Warn("Loan not found", code: (int)HttpStatusCode.NotFound);
                    if (l.PaidInstallments > 0) return Utilities.Warn("Loan already has recoveries; can't edit terms.");
                    l.PrincipalAmount = request.PrincipalAmount; l.InstallmentAmount = request.InstallmentAmount;
                    l.TotalInstallments = request.TotalInstallments; l.OutstandingAmount = request.PrincipalAmount;
                    l.StartMonth = request.StartMonth; l.StartYear = request.StartYear; l.Reason = request.Reason;
                    l.UpdatedBy = request.CreateUpdateBy; l.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    var max = await _ctx.Loans!.Where(x => x.LoanCode!.StartsWith("LOAN-"))
                        .Select(x => x.LoanCode!.Substring(5)).ToListAsync();
                    int next = max.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty(0).Max() + 1;

                    await _ctx.Loans!.AddAsync(new Loan
                    {
                        Id = Guid.NewGuid(), LoanCode = $"LOAN-{next:D4}", EmployeeId = empId,
                        PrincipalAmount = request.PrincipalAmount, InstallmentAmount = request.InstallmentAmount,
                        TotalInstallments = request.TotalInstallments, PaidInstallments = 0,
                        OutstandingAmount = request.PrincipalAmount, StartMonth = request.StartMonth, StartYear = request.StartYear,
                        StatusId = (int)Enum.EnumLoanStatus.Active, Reason = request.Reason,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateLoanAsync)); }
        }

        public async Task<DataResponse> SetLoanStatusAsync(LoanActionRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid loan id");
                var l = await _ctx.Loans!.FirstOrDefaultAsync(x => x.Id == id);
                if (l == null) return Utilities.Warn("Loan not found", code: (int)HttpStatusCode.NotFound);
                l.StatusId = request.StatusId; l.UpdatedBy = request.CreateUpdateBy; l.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetLoanStatusAsync)); }
        }
    }
}
