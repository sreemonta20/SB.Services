using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Data.Common;
using System.Net;

namespace SBERP.HumanResources.Service
{
    /// <summary>ADO helper for the separation workflow SPs — JSON scalar reads
    /// plus the RowsAffected/Message reader for the transactional SPs.</summary>
    internal static class SeparationAdo
    {
        public static async Task<string?> ScalarSpAsync(HumanResourcesDBContext ctx, string sp, params (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ps.Length == 0 ? $"EXEC dbo.{sp}"
                : $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            return (await cmd.ExecuteScalarAsync()) as string;
        }

        public static async Task<(int rows, string msg)> StatusSpAsync(HumanResourcesDBContext ctx, string sp, params (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            using var reader = await cmd.ExecuteReaderAsync();
            int rows = 0; string msg = ConstantSupplier.FAILED_MSG;
            if (await reader.ReadAsync())
            {
                rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                msg = reader.GetString(reader.GetOrdinal("Message"));
            }
            await reader.CloseAsync();
            return (rows, msg);
        }

        public static void AddParam(DbCommand cmd, string name, object? value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p); }
    }

    public class SeparationService : ISeparationService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public SeparationService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<SeparationResponse>?> GetAllSeparationsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<SeparationResponse>(_ctx, ConstantSupplier.SP_GET_ALL_SEPARATIONS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllSeparationsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetSeparationDetailAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid separation id");
                var json = await SeparationAdo.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_SEPARATION_DETAIL, ("@Id", g));
                if (string.IsNullOrWhiteSpace(json)) return Utilities.Warn("Separation not found", code: (int)HttpStatusCode.NotFound);
                var obj = JsonConvert.DeserializeObject<object>(json!);
                return Utilities.Ok(ConstantSupplier.SEPARATION_FETCH, obj);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetSeparationDetailAsync)); }
        }

        public async Task<DataResponse> CreateUpdateSeparationAsync(SeparationRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid separation id");
                    var s = await _ctx.Separations!.FirstOrDefaultAsync(x => x.Id == id);
                    if (s == null) return Utilities.Warn("Separation not found", code: (int)HttpStatusCode.NotFound);
                    if (s.StatusId >= (int)Enum.EnumSeparationStatus.Approved)
                        return Utilities.Warn("This separation has progressed past Pending and can't be edited.");
                    s.EmployeeId = empId; s.SeparationTypeId = request.SeparationTypeId; s.Reason = request.Reason;
                    s.NoticeDate = request.NoticeDate; s.RequestedLastWorkingDate = request.RequestedLastWorkingDate;
                    s.NoticePeriodDays = request.NoticePeriodDays; s.IsNoticeWaived = request.IsNoticeWaived;
                    s.UpdatedBy = request.CreateUpdateBy; s.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    // one active separation per employee (StatusId < Completed and IsActive)
                    bool active = await _ctx.Separations!.AnyAsync(x => x.EmployeeId == empId && x.IsActive == true
                        && x.StatusId < (int)Enum.EnumSeparationStatus.Completed);
                    if (active) return Utilities.Warn(ConstantSupplier.SEPARATION_ACTIVE_EXISTS);

                    var max = await _ctx.Separations!.Where(x => x.SeparationCode!.StartsWith("SEP-"))
                        .Select(x => x.SeparationCode!.Substring(4)).ToListAsync();
                    int next = max.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty(0).Max() + 1;

                    await _ctx.Separations!.AddAsync(new Separation
                    {
                        Id = Guid.NewGuid(), SeparationCode = $"SEP-{next:D4}", EmployeeId = empId,
                        SeparationTypeId = request.SeparationTypeId, StatusId = (int)Enum.EnumSeparationStatus.Draft,
                        Reason = request.Reason, NoticeDate = request.NoticeDate,
                        RequestedLastWorkingDate = request.RequestedLastWorkingDate,
                        LastWorkingDate = request.RequestedLastWorkingDate,
                        NoticePeriodDays = request.NoticePeriodDays, IsNoticeWaived = request.IsNoticeWaived,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateSeparationAsync)); }
        }

        public async Task<DataResponse> DeleteSeparationAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid separation id");
                var s = await _ctx.Separations!.FirstOrDefaultAsync(x => x.Id == g);
                if (s == null) return Utilities.Warn("Separation not found", code: (int)HttpStatusCode.NotFound);
                if (s.StatusId >= (int)Enum.EnumSeparationStatus.Approved)
                    return Utilities.Warn("This separation has progressed and can't be removed; cancel it instead.");
                s.IsActive = false; s.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteSeparationAsync)); }
        }

        public async Task<DataResponse> SetSeparationStatusAsync(SeparationStatusRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid separation id");
                var (rows, msg) = await SeparationAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_SEPARATION_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetSeparationStatusAsync)); }
        }

        public async Task<DataResponse> ApproveSeparationAsync(ApproveSeparationRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid separation id");
                var (rows, msg) = await SeparationAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_APPROVE_SEPARATION,
                    ("@Id", id), ("@LastWorkingDate", request.LastWorkingDate), ("@ApprovedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApproveSeparationAsync)); }
        }

        public async Task<DataResponse> SettleSeparationAsync(SettleSeparationRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid separation id");
                var (rows, msg) = await SeparationAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SETTLE_SEPARATION,
                    ("@Id", id), ("@SettlementRemarks", request.SettlementRemarks),
                    ("@IsRehireEligible", request.IsRehireEligible), ("@SettledBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SettleSeparationAsync)); }
        }

        public async Task<DataResponse> SetClearanceStatusAsync(ClearanceStatusRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid clearance id");
                var (rows, msg) = await SeparationAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_CLEARANCE_STATUS,
                    ("@Id", id), ("@ClearanceStatusId", request.ClearanceStatusId),
                    ("@Remarks", request.Remarks), ("@DuesAmount", request.DuesAmount), ("@ClearedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetClearanceStatusAsync)); }
        }

        public async Task<DataResponse> CreateUpdateExitInterviewAsync(ExitInterviewRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.SeparationId, out var sepId)) return Utilities.Warn("Invalid separation id");
                Guid? interviewerId = Guid.TryParse(request.InterviewerId, out var iv) ? iv : (Guid?)null;
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid exit interview id");
                    var x = await _ctx.ExitInterviews!.FirstOrDefaultAsync(e => e.Id == id);
                    if (x == null) return Utilities.Warn("Exit interview not found", code: (int)HttpStatusCode.NotFound);
                    x.InterviewDate = request.InterviewDate; x.InterviewerId = interviewerId;
                    x.PrimaryReason = request.PrimaryReason; x.OverallExperienceRating = request.OverallExperienceRating;
                    x.WouldRecommend = request.WouldRecommend; x.Feedback = request.Feedback; x.Suggestions = request.Suggestions;
                    x.UpdatedBy = request.CreateUpdateBy; x.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    bool exists = await _ctx.ExitInterviews!.AnyAsync(e => e.SeparationId == sepId && e.IsActive == true);
                    if (exists) return Utilities.Warn(ConstantSupplier.EXIT_ALREADY_EXISTS);

                    await _ctx.ExitInterviews!.AddAsync(new ExitInterview
                    {
                        Id = Guid.NewGuid(), SeparationId = sepId, InterviewDate = request.InterviewDate,
                        InterviewerId = interviewerId, PrimaryReason = request.PrimaryReason,
                        OverallExperienceRating = request.OverallExperienceRating, WouldRecommend = request.WouldRecommend,
                        Feedback = request.Feedback, Suggestions = request.Suggestions,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });

                    // flag the separation so the list shows the exit interview is done
                    var sep = await _ctx.Separations!.FirstOrDefaultAsync(s => s.Id == sepId);
                    if (sep != null) { sep.ExitInterviewDone = true; sep.UpdatedBy = request.CreateUpdateBy; sep.UpdatedDate = DateTime.UtcNow; }
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateExitInterviewAsync)); }
        }
    }
}
