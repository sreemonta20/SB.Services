using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using SBERP.HumanResources.Service;       // PagingHelper (from Recruitment)
using System.Net;

namespace SBERP.HumanResources.Service
{
    public interface IOnboardingService
    {
        Task<PagingResult<OnboardingResponse>?> GetAllOnboardingsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> GetAcceptedOffersAwaitingOnboardingAsync();
        Task<DataResponse> StartOnboardingAsync(StartOnboardingRequest request);
        Task<DataResponse> GetOnboardingTasksAsync(string onboardingId);
        Task<DataResponse> UpdateOnboardingTaskAsync(UpdateOnboardingTaskRequest request);
        Task<DataResponse> AddOnboardingTaskAsync(AddOnboardingTaskRequest request);
        Task<DataResponse> CompleteOnboardingAsync(OnboardingActionRequest request);
        Task<DataResponse> CancelOnboardingAsync(OnboardingActionRequest request);
    }

    /// <summary>
    /// Onboarding service. The offer→employee conversion + outbox write goes
    /// through SP_OnboardEmployeeFromOffer (atomic, multi-table). Checklist
    /// edits and status transitions use EF. Same DataResponse/Utilities shape.
    /// </summary>
    public class OnboardingService : IOnboardingService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public OnboardingService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<OnboardingResponse>?> GetAllOnboardingsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<OnboardingResponse>(_ctx, ConstantSupplier.SP_GET_ALL_ONBOARDINGS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllOnboardingsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetAcceptedOffersAwaitingOnboardingAsync()
        {
            try
            {
                var json = await ScalarSpAsync(ConstantSupplier.SP_GET_ACCEPTED_OFFERS_AWAITING_ONB);
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<AcceptedOfferResponse>()
                    : JsonConvert.DeserializeObject<List<AcceptedOfferResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.ONBOARDING_OFFERS_FETCH, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAcceptedOffersAwaitingOnboardingAsync)); }
        }

        public async Task<DataResponse> StartOnboardingAsync(StartOnboardingRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.JobOfferId, out var offerId)) return Utilities.Warn("Invalid offer id");

                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_ONBOARD_EMPLOYEE_FROM_OFFER +
                    " @JobOfferId=@p0, @ReportingManagerId=@p1, @WorkLocation=@p2, @CreatedBy=@p3";
                AddParam(cmd, "@p0", offerId);
                AddParam(cmd, "@p1", Guid.TryParse(request.ReportingManagerId, out var rm) ? rm : (object?)null);
                AddParam(cmd, "@p2", request.WorkLocation);
                AddParam(cmd, "@p3", request.CreateUpdateBy);

                using var reader = await cmd.ExecuteReaderAsync();
                int rows = 0; string msg = ConstantSupplier.FAILED_MSG; string? empCode = null; object? empId = null;
                if (await reader.ReadAsync())
                {
                    rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                    msg = reader.GetString(reader.GetOrdinal("Message"));
                    int ec = TryOrdinal(reader, "EmployeeCode");
                    if (ec >= 0 && !reader.IsDBNull(ec)) empCode = reader.GetString(ec);
                    int ei = TryOrdinal(reader, "EmployeeId");
                    if (ei >= 0 && !reader.IsDBNull(ei)) empId = reader.GetGuid(ei);
                }
                await reader.CloseAsync();

                return rows > 0
                    ? Utilities.Ok(ConstantSupplier.ONBOARDING_START_SUCCESS, new { employeeId = empId, employeeCode = empCode })
                    : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(StartOnboardingAsync)); }
        }

        public async Task<DataResponse> GetOnboardingTasksAsync(string onboardingId)
        {
            try
            {
                if (!Guid.TryParse(onboardingId, out var g)) return Utilities.Warn("Invalid onboarding id");
                var json = await ScalarSpAsync(ConstantSupplier.SP_GET_ONBOARDING_TASKS, ("@OnboardingId", g));
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<OnboardingTaskResponse>()
                    : JsonConvert.DeserializeObject<List<OnboardingTaskResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.ONBOARDING_TASKS_FETCH, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetOnboardingTasksAsync)); }
        }

        public async Task<DataResponse> UpdateOnboardingTaskAsync(UpdateOnboardingTaskRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.Id, out var taskId)) return Utilities.Warn("Invalid task id");
                if (!Guid.TryParse(request.OnboardingId, out var onbId)) return Utilities.Warn("Invalid onboarding id");

                var task = await _ctx.OnboardingTasks!.FirstOrDefaultAsync(t => t.Id == taskId);
                if (task == null) return Utilities.Warn("Task not found", code: (int)HttpStatusCode.NotFound);

                task.StatusId = request.StatusId;
                task.CompletedDate = request.StatusId == (int)EnumOnboardingTaskStatus.Done ? DateTime.UtcNow : null;
                task.UpdatedBy = request.CreateUpdateBy; task.UpdatedDate = DateTime.UtcNow;

                // recompute the parent's progress and roll status forward
                var onb = await _ctx.Onboardings!.FirstOrDefaultAsync(o => o.Id == onbId);
                if (onb != null)
                {
                    var tasks = await _ctx.OnboardingTasks!.Where(t => t.OnboardingId == onbId).ToListAsync();
                    onb.ChecklistTotal = tasks.Count;
                    onb.ChecklistDone = tasks.Count(t =>
                        t.Id == taskId
                            ? request.StatusId == (int)EnumOnboardingTaskStatus.Done
                            : t.StatusId == (int)EnumOnboardingTaskStatus.Done);
                    if (onb.StatusId == (int)EnumOnboardingStatus.Initiated && onb.ChecklistDone > 0)
                        onb.StatusId = (int)EnumOnboardingStatus.InProgress;
                    onb.UpdatedBy = request.CreateUpdateBy; onb.UpdatedDate = DateTime.UtcNow;
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.ONBOARDING_TASK_UPDATED, task.Id);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(UpdateOnboardingTaskAsync)); }
        }

        public async Task<DataResponse> AddOnboardingTaskAsync(AddOnboardingTaskRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.OnboardingId, out var onbId)) return Utilities.Warn("Invalid onboarding id");
                var onb = await _ctx.Onboardings!.FirstOrDefaultAsync(o => o.Id == onbId);
                if (onb == null) return Utilities.Warn(ConstantSupplier.ONBOARDING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);

                int nextSort = await _ctx.OnboardingTasks!.Where(t => t.OnboardingId == onbId)
                    .Select(t => (int?)t.SortOrder).MaxAsync() ?? 0;

                var task = new OnboardingTask
                {
                    Id = Guid.NewGuid(), OnboardingId = onbId, Title = request.Title,
                    Description = request.Description, OwnerRole = request.OwnerRole, DueDate = request.DueDate,
                    StatusId = (int)EnumOnboardingTaskStatus.Pending, SortOrder = nextSort + 1,
                    CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                };
                await _ctx.OnboardingTasks!.AddAsync(task);

                onb.ChecklistTotal += 1;
                onb.UpdatedBy = request.CreateUpdateBy; onb.UpdatedDate = DateTime.UtcNow;

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.ONBOARDING_TASK_ADDED, task.Id);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(AddOnboardingTaskAsync)); }
        }

        public Task<DataResponse> CompleteOnboardingAsync(OnboardingActionRequest request)
            => TransitionAsync(request, to: (int)EnumOnboardingStatus.Completed,
                               okMsg: ConstantSupplier.ONBOARDING_COMPLETE_SUCCESS, stampCompleted: true);

        public Task<DataResponse> CancelOnboardingAsync(OnboardingActionRequest request)
            => TransitionAsync(request, to: (int)EnumOnboardingStatus.Cancelled,
                               okMsg: ConstantSupplier.ONBOARDING_CANCEL_SUCCESS, stampCompleted: false);

        private async Task<DataResponse> TransitionAsync(OnboardingActionRequest request, int to, string okMsg, bool stampCompleted)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid onboarding id");
                var onb = await _ctx.Onboardings!.FirstOrDefaultAsync(o => o.Id == id);
                if (onb == null) return Utilities.Warn(ConstantSupplier.ONBOARDING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (onb.StatusId == (int)EnumOnboardingStatus.Completed || onb.StatusId == (int)EnumOnboardingStatus.Cancelled)
                    return Utilities.Warn(ConstantSupplier.ONBOARDING_BAD_STATE);

                onb.StatusId = to;
                if (stampCompleted) onb.CompletedDate = DateTime.UtcNow;
                onb.Remarks = request.Remarks;
                onb.UpdatedBy = request.CreateUpdateBy; onb.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(okMsg, onb.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(TransitionAsync)); }
        }

        // ---- small ADO helpers (same approach as the recruitment services) ----
        private async Task<string?> ScalarSpAsync(string sp, params (string name, object? value)[] ps)
        {
            var conn = _ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ps.Length == 0
                ? $"EXEC dbo.{sp}"
                : $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            return (await cmd.ExecuteScalarAsync()) as string;
        }

        private static int TryOrdinal(System.Data.Common.DbDataReader r, string name)
        { try { return r.GetOrdinal(name); } catch { return -1; } }

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p); }
    }
}
