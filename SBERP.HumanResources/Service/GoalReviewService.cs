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
    public class GoalService : IGoalService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public GoalService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<GoalResponse>?> GetAllGoalsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<GoalResponse>(_ctx, ConstantSupplier.SP_GET_ALL_GOALS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllGoalsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateGoalAsync(GoalRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.AppraisalCycleId, out var cycleId)) return Utilities.Warn("Invalid cycle id");
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid goal id");
                    var g = await _ctx.Goals!.FirstOrDefaultAsync(x => x.Id == id);
                    if (g == null) return Utilities.Warn("Goal not found", code: (int)HttpStatusCode.NotFound);
                    g.AppraisalCycleId = cycleId; g.EmployeeId = empId; g.Title = request.Title; g.Description = request.Description;
                    g.Weight = request.Weight; g.TargetValue = request.TargetValue; g.ProgressPercent = request.ProgressPercent;
                    g.StatusId = request.StatusId; g.StartDate = request.StartDate; g.DueDate = request.DueDate;
                    g.UpdatedBy = request.CreateUpdateBy; g.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    var max = await _ctx.Goals!.Where(x => x.GoalCode!.StartsWith("GL-"))
                        .Select(x => x.GoalCode!.Substring(3)).ToListAsync();
                    int next = max.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty(0).Max() + 1;

                    await _ctx.Goals!.AddAsync(new Goal
                    {
                        Id = Guid.NewGuid(), GoalCode = $"GL-{next:D4}", AppraisalCycleId = cycleId, EmployeeId = empId,
                        Title = request.Title, Description = request.Description, Weight = request.Weight,
                        TargetValue = request.TargetValue, ProgressPercent = request.ProgressPercent,
                        StatusId = request.StatusId <= 0 ? (int)Enum.EnumGoalStatus.Draft : request.StatusId,
                        StartDate = request.StartDate, DueDate = request.DueDate,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateGoalAsync)); }
        }

        public async Task<DataResponse> UpdateGoalProgressAsync(GoalProgressRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid goal id");
                var g = await _ctx.Goals!.FirstOrDefaultAsync(x => x.Id == id);
                if (g == null) return Utilities.Warn("Goal not found", code: (int)HttpStatusCode.NotFound);
                g.ProgressPercent = request.ProgressPercent;
                if (request.StatusId > 0) g.StatusId = request.StatusId;
                g.UpdatedBy = request.CreateUpdateBy; g.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UpdateGoalProgressAsync)); }
        }

        public async Task<DataResponse> DeleteGoalAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid goal id");
                var goal = await _ctx.Goals!.FirstOrDefaultAsync(x => x.Id == g);
                if (goal == null) return Utilities.Warn("Goal not found", code: (int)HttpStatusCode.NotFound);
                goal.IsActive = false; goal.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteGoalAsync)); }
        }
    }

    public class ReviewService : IReviewService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public ReviewService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<ReviewResponse>?> GetAllReviewsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<ReviewResponse>(_ctx, ConstantSupplier.SP_GET_ALL_REVIEWS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllReviewsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetReviewDetailAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid review id");
                var json = await AdoHelper.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_REVIEW_DETAIL, ("@Id", g));
                if (string.IsNullOrWhiteSpace(json)) return Utilities.Warn("Review not found", code: (int)HttpStatusCode.NotFound);
                var obj = JsonConvert.DeserializeObject<object>(json!);
                return Utilities.Ok(ConstantSupplier.REVIEW_FETCH, obj);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetReviewDetailAsync)); }
        }

        public async Task<DataResponse> SaveReviewRatingsAsync(SaveReviewRatingsRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.ReviewId, out var id)) return Utilities.Warn("Invalid review id");
                var ratingsJson = JsonConvert.SerializeObject(request.Ratings ?? new List<ReviewRatingItem>());
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_SAVE_REVIEW_RATINGS,
                    ("@ReviewId", id), ("@Ratings", ratingsJson),
                    ("@SelfComments", request.SelfComments), ("@ManagerComments", request.ManagerComments),
                    ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SaveReviewRatingsAsync)); }
        }

        public async Task<DataResponse> SetReviewStatusAsync(ReviewActionRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid review id");
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_REVIEW_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId),
                    ("@Acknowledgement", request.Acknowledgement), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetReviewStatusAsync)); }
        }
    }
}
