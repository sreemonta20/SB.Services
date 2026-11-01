using Microsoft.EntityFrameworkCore;
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
    public class AppraisalCycleService : IAppraisalCycleService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public AppraisalCycleService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<AppraisalCycleResponse>?> GetAllAppraisalCyclesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<AppraisalCycleResponse>(_ctx, ConstantSupplier.SP_GET_ALL_APPRAISAL_CYCLES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllAppraisalCyclesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetAppraisalCyclesLookupAsync()
        {
            try
            {
                var items = await _ctx.AppraisalCycles!.Where(c => c.IsActive == true)
                    .OrderByDescending(c => c.PeriodStart)
                    .Select(c => new AppraisalCycleResponse
                    {
                        Id = c.Id, CycleCode = c.CycleCode, Name = c.Name, PeriodStart = c.PeriodStart,
                        PeriodEnd = c.PeriodEnd, RatingScaleMax = c.RatingScaleMax, StatusId = c.StatusId, IsActive = c.IsActive
                    }).ToListAsync();
                return Utilities.Ok(ConstantSupplier.CYCLE_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAppraisalCyclesLookupAsync)); }
        }

        public async Task<DataResponse> CreateUpdateAppraisalCycleAsync(AppraisalCycleRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                bool dup = await _ctx.AppraisalCycles!.AnyAsync(c => c.CycleCode == request.CycleCode && (!isUpdate || c.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Cycle code already exists.");

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid cycle id");
                    var c = await _ctx.AppraisalCycles!.FirstOrDefaultAsync(x => x.Id == id);
                    if (c == null) return Utilities.Warn("Cycle not found", code: (int)HttpStatusCode.NotFound);
                    c.CycleCode = request.CycleCode; c.Name = request.Name; c.PeriodStart = request.PeriodStart;
                    c.PeriodEnd = request.PeriodEnd; c.GoalDueDate = request.GoalDueDate; c.ReviewDueDate = request.ReviewDueDate;
                    c.RatingScaleMax = request.RatingScaleMax; c.Description = request.Description;
                    c.UpdatedBy = request.CreateUpdateBy; c.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    await _ctx.AppraisalCycles!.AddAsync(new AppraisalCycle
                    {
                        Id = Guid.NewGuid(), CycleCode = request.CycleCode, Name = request.Name,
                        PeriodStart = request.PeriodStart, PeriodEnd = request.PeriodEnd,
                        GoalDueDate = request.GoalDueDate, ReviewDueDate = request.ReviewDueDate,
                        RatingScaleMax = request.RatingScaleMax <= 0 ? 5 : request.RatingScaleMax,
                        StatusId = (int)Enum.EnumAppraisalCycleStatus.Draft, Description = request.Description,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateAppraisalCycleAsync)); }
        }

        public async Task<DataResponse> DeleteAppraisalCycleAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid cycle id");
                var c = await _ctx.AppraisalCycles!.FirstOrDefaultAsync(x => x.Id == g);
                if (c == null) return Utilities.Warn("Cycle not found", code: (int)HttpStatusCode.NotFound);
                bool inUse = await _ctx.Goals!.AnyAsync(x => x.AppraisalCycleId == g)
                          || await _ctx.Reviews!.AnyAsync(x => x.AppraisalCycleId == g);
                if (inUse) return Utilities.Warn(ConstantSupplier.CYCLE_IN_USE);
                c.IsActive = false; c.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteAppraisalCycleAsync)); }
        }

        public async Task<DataResponse> SetAppraisalCycleStatusAsync(CycleActionRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid cycle id");
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_APPRAISAL_CYCLE_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetAppraisalCycleStatusAsync)); }
        }

        public async Task<DataResponse> InitiateReviewsAsync(InitiateReviewsRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.CycleId, out var id)) return Utilities.Warn("Invalid cycle id");
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_INITIATE_REVIEWS_FOR_CYCLE,
                    ("@CycleId", id), ("@CreatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(InitiateReviewsAsync)); }
        }
    }

    public class CompetencyService : ICompetencyService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public CompetencyService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<CompetencyResponse>?> GetAllCompetenciesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<CompetencyResponse>(_ctx, "SP_GetAllCompetenciesPagingWithSearch", filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllCompetenciesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetCompetenciesLookupAsync()
        {
            try
            {
                var items = await _ctx.Competencies!.Where(c => c.IsActive == true)
                    .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
                    .Select(c => new CompetencyResponse { Id = c.Id, CompetencyCode = c.CompetencyCode, Name = c.Name, Description = c.Description, SortOrder = c.SortOrder, IsActive = c.IsActive })
                    .ToListAsync();
                return Utilities.Ok(ConstantSupplier.COMPETENCY_FETCH, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetCompetenciesLookupAsync)); }
        }

        public async Task<DataResponse> CreateUpdateCompetencyAsync(CompetencyRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                bool dup = await _ctx.Competencies!.AnyAsync(c => c.CompetencyCode == request.CompetencyCode && (!isUpdate || c.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Competency code already exists.");

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid competency id");
                    var c = await _ctx.Competencies!.FirstOrDefaultAsync(x => x.Id == id);
                    if (c == null) return Utilities.Warn("Competency not found", code: (int)HttpStatusCode.NotFound);
                    c.CompetencyCode = request.CompetencyCode; c.Name = request.Name; c.Description = request.Description;
                    c.SortOrder = request.SortOrder; c.UpdatedBy = request.CreateUpdateBy; c.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    await _ctx.Competencies!.AddAsync(new Competency
                    {
                        Id = Guid.NewGuid(), CompetencyCode = request.CompetencyCode, Name = request.Name,
                        Description = request.Description, SortOrder = request.SortOrder,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateCompetencyAsync)); }
        }

        public async Task<DataResponse> DeleteCompetencyAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid competency id");
                var c = await _ctx.Competencies!.FirstOrDefaultAsync(x => x.Id == g);
                if (c == null) return Utilities.Warn("Competency not found", code: (int)HttpStatusCode.NotFound);
                c.IsActive = false; c.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteCompetencyAsync)); }
        }
    }
}
