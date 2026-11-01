using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Net;

namespace SBERP.HumanResources.Service
{
    // =========================================================================
    // JobPostingService
    // =========================================================================
    public class JobPostingService : IJobPostingService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public JobPostingService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<JobPostingResponse>?> GetAllJobPostingsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<JobPostingResponse>(_ctx, ConstantSupplier.SP_GET_ALL_JOBPOSTINGS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllJobPostingsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetJobPostingByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid job posting id");
                var jp = await _ctx.JobPostings!.AsNoTracking()
                    .Where(x => x.Id == g)
                    .Select(x => new JobPostingResponse
                    {
                        Id = x.Id, PostingCode = x.PostingCode, Title = x.Title, Description = x.Description,
                        Responsibilities = x.Responsibilities, Requirements = x.Requirements,
                        WorkLocation = x.WorkLocation, IsRemote = x.IsRemote, PostedDate = x.PostedDate,
                        ClosingDate = x.ClosingDate, StatusId = x.StatusId, ManpowerRequisitionId = x.ManpowerRequisitionId
                    })
                    .FirstOrDefaultAsync();
                if (jp == null) return Utilities.Warn(ConstantSupplier.JOBPOSTING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.JOBPOSTING_FETCH_SUCCESS, jp);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetJobPostingByIdAsync)); }
        }

        public async Task<DataResponse> CreateUpdateJobPostingAsync(JobPostingRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName, ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);
                if (!Guid.TryParse(request.ManpowerRequisitionId, out var mrfId))
                    return Utilities.Warn("Invalid requisition id");

                // A posting can only be created against an APPROVED requisition.
                var mrf = await _ctx.ManpowerRequisitions!.AsNoTracking().FirstOrDefaultAsync(m => m.Id == mrfId);
                if (mrf == null) return Utilities.Warn(ConstantSupplier.REQUISITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (isSave && mrf.StatusId != (int)EnumRequisitionStatus.Approved)
                    return Utilities.Warn(ConstantSupplier.JOBPOSTING_REQ_NOT_APPROVED);

                if (isSave)
                {
                    var code = await NextCodeAsync("JP");
                    var jp = new JobPosting
                    {
                        Id = Guid.NewGuid(), PostingCode = code, ManpowerRequisitionId = mrfId, Title = request.Title,
                        Description = request.Description, Responsibilities = request.Responsibilities,
                        Requirements = request.Requirements, EmploymentType = request.EmploymentType,
                        WorkLocation = request.WorkLocation, IsRemote = request.IsRemote,
                        SalaryRangeText = request.SalaryRangeText, PublishChannels = request.PublishChannels,
                        ClosingDate = request.ClosingDate, StatusId = (int)EnumJobPostingStatus.Draft,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = request.IsActive ?? true
                    };
                    await _ctx.JobPostings!.AddAsync(jp);
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.JOBPOSTING_CREATE_SUCCESS, jp.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid id for update");
                    var jp = await _ctx.JobPostings!.FirstOrDefaultAsync(x => x.Id == id);
                    if (jp == null) return Utilities.Warn(ConstantSupplier.JOBPOSTING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);

                    jp.Title = request.Title; jp.Description = request.Description;
                    jp.Responsibilities = request.Responsibilities; jp.Requirements = request.Requirements;
                    jp.EmploymentType = request.EmploymentType; jp.WorkLocation = request.WorkLocation;
                    jp.IsRemote = request.IsRemote; jp.SalaryRangeText = request.SalaryRangeText;
                    jp.PublishChannels = request.PublishChannels; jp.ClosingDate = request.ClosingDate;
                    jp.UpdatedBy = request.CreateUpdateBy; jp.UpdatedDate = DateTime.UtcNow;
                    jp.IsActive = request.IsActive ?? true;

                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.JOBPOSTING_UPDATE_SUCCESS, jp.Id);
                }
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueViolation(dbEx))
            {
                await tx.RollbackAsync();
                return Utilities.Warn(ConstantSupplier.JOBPOSTING_CREATE_SUCCESS);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateJobPostingAsync));
            }
        }

        public async Task<DataResponse> PublishJobPostingAsync(string id, string? user)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid job posting id");
                var jp = await _ctx.JobPostings!.FirstOrDefaultAsync(x => x.Id == g);
                if (jp == null) return Utilities.Warn(ConstantSupplier.JOBPOSTING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);

                jp.StatusId = (int)EnumJobPostingStatus.Published;
                jp.PostedDate = DateTime.UtcNow;
                jp.UpdatedBy = user; jp.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.JOBPOSTING_PUBLISH_SUCCESS, jp.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(PublishJobPostingAsync)); }
        }

        public async Task<DataResponse> CloseJobPostingAsync(string id, string? user)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid job posting id");
                var jp = await _ctx.JobPostings!.FirstOrDefaultAsync(x => x.Id == g);
                if (jp == null) return Utilities.Warn(ConstantSupplier.JOBPOSTING_NOT_FOUND, code: (int)HttpStatusCode.NotFound);

                jp.StatusId = (int)EnumJobPostingStatus.Closed;
                jp.UpdatedBy = user; jp.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.JOBPOSTING_CLOSE_SUCCESS, jp.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CloseJobPostingAsync)); }
        }

        public async Task<DataResponse> GetApplicationsByPostingAsync(string jobPostingId)
        {
            try
            {
                if (!Guid.TryParse(jobPostingId, out var g)) return Utilities.Warn("Invalid job posting id");
                var json = await AdoHelper.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_APPLICATIONS_BY_POSTING, ("@JobPostingId", g));
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<ApplicationCardResponse>()
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApplicationCardResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.APPLICATION_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetApplicationsByPostingAsync)); }
        }

        private async Task<string> NextCodeAsync(string prefix)
        {
            var last = await _ctx.JobPostings!.AsNoTracking()
                .Where(x => x.PostingCode!.StartsWith(prefix + "-"))
                .OrderByDescending(x => x.PostingCode).Select(x => x.PostingCode).FirstOrDefaultAsync();
            int n = 0;
            if (!string.IsNullOrEmpty(last) && int.TryParse(last.Substring(prefix.Length + 1), out var parsed)) n = parsed;
            return $"{prefix}-{(n + 1):D4}";
        }
    }

    // =========================================================================
    // CandidateService — list + apply (SP) + stage move (SP)
    // =========================================================================
    public class CandidateService : ICandidateService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public CandidateService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<CandidateResponse>?> GetAllCandidatesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<CandidateResponse>(_ctx, ConstantSupplier.SP_GET_ALL_CANDIDATES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllCandidatesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> ApplyToPostingAsync(CandidateApplicationRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.JobPostingId, out var postingId))
                    return Utilities.Warn("Invalid job posting id");

                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_SAVE_CANDIDATE_APPLICATION +
                    " @JobPostingId=@p0, @FirstName=@p1, @LastName=@p2, @Email=@p3, @Phone=@p4, " +
                    "@TotalExperienceYears=@p5, @CurrentEmployer=@p6, @ExpectedSalary=@p7, @ResumeUrl=@p8, " +
                    "@Source=@p9, @ReferredByEmployeeId=@p10, @CreatedBy=@p11";
                AddParam(cmd, "@p0", postingId);
                AddParam(cmd, "@p1", request.FirstName);
                AddParam(cmd, "@p2", request.LastName);
                AddParam(cmd, "@p3", request.Email);
                AddParam(cmd, "@p4", request.Phone);
                AddParam(cmd, "@p5", request.TotalExperienceYears);
                AddParam(cmd, "@p6", request.CurrentEmployer);
                AddParam(cmd, "@p7", request.ExpectedSalary);
                AddParam(cmd, "@p8", request.ResumeUrl);
                AddParam(cmd, "@p9", request.Source);
                AddParam(cmd, "@p10", Guid.TryParse(request.ReferredByEmployeeId, out var rb) ? rb : (object?)null);
                AddParam(cmd, "@p11", request.CreateUpdateBy);

                // SP returns a small result set: RowsAffected + Message (+ codes on success)
                using var reader = await cmd.ExecuteReaderAsync();
                int rows = 0; string msg = ConstantSupplier.FAILED_MSG; string? appCode = null;
                if (await reader.ReadAsync())
                {
                    rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                    msg  = reader.GetString(reader.GetOrdinal("Message"));
                    int ac = TryOrdinal(reader, "ApplicationCode");
                    if (ac >= 0 && !reader.IsDBNull(ac)) appCode = reader.GetString(ac);
                }
                await reader.CloseAsync();

                return rows > 0
                    ? Utilities.Ok(ConstantSupplier.APPLICATION_CAPTURE_SUCCESS, appCode)
                    : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApplyToPostingAsync)); }
        }

        public async Task<DataResponse> MoveApplicationStageAsync(MoveStageRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var appId)) return Utilities.Warn("Invalid application id");

                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_MOVE_APPLICATION_STAGE +
                    " @Id=@p0, @StageId=@p1, @ScreeningScore=@p2, @ScreeningNotes=@p3, @RejectionReason=@p4, @UpdatedBy=@p5";
                AddParam(cmd, "@p0", appId);
                AddParam(cmd, "@p1", request.StageId);
                AddParam(cmd, "@p2", request.ScreeningScore);
                AddParam(cmd, "@p3", request.ScreeningNotes);
                AddParam(cmd, "@p4", request.RejectionReason);
                AddParam(cmd, "@p5", request.CreateUpdateBy);
                await cmd.ExecuteNonQueryAsync();

                return Utilities.Ok(ConstantSupplier.APPLICATION_STAGE_UPDATED, request.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(MoveApplicationStageAsync)); }
        }

        private static int TryOrdinal(System.Data.Common.DbDataReader r, string name)
        { try { return r.GetOrdinal(name); } catch { return -1; } }

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p); }
    }

    // =========================================================================
    // InterviewService — rounds + panel (EF) and read via SP
    // =========================================================================
    public class InterviewService : IInterviewService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public InterviewService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetInterviewsByApplicationAsync(string jobApplicationId)
        {
            try
            {
                if (!Guid.TryParse(jobApplicationId, out var g)) return Utilities.Warn("Invalid application id");
                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC dbo." + ConstantSupplier.SP_GET_INTERVIEWS_BY_APPLICATION + " @JobApplicationId=@p0";
                var p = cmd.CreateParameter(); p.ParameterName = "@p0"; p.Value = g; cmd.Parameters.Add(p);
                var raw = (await cmd.ExecuteScalarAsync()) as string;
                var arr = string.IsNullOrWhiteSpace(raw) ? new JArray() : JArray.Parse(raw);
                return Utilities.Ok(ConstantSupplier.INTERVIEW_FETCH_SUCCESS, arr);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetInterviewsByApplicationAsync)); }
        }

        public async Task<DataResponse> CreateUpdateInterviewAsync(InterviewRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName, ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);
                bool isComplete = string.Equals(request.ActionName, "Complete", StringComparison.OrdinalIgnoreCase);
                if (!Guid.TryParse(request.JobApplicationId, out var appId)) return Utilities.Warn("Invalid application id");

                if (isSave)
                {
                    var iv = new Interview
                    {
                        Id = Guid.NewGuid(), JobApplicationId = appId, RoundNumber = request.RoundNumber,
                        RoundName = request.RoundName, InterviewModeId = request.InterviewModeId,
                        ScheduledAt = request.ScheduledAt, DurationMinutes = request.DurationMinutes,
                        Location = request.Location, MeetingLink = request.MeetingLink, IsCompleted = false,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.Interviews!.AddAsync(iv);
                    // bump the application into the Interview stage if not past it
                    var app = await _ctx.JobApplications!.FirstOrDefaultAsync(a => a.Id == appId);
                    if (app != null && app.StageId < (int)EnumApplicationStage.Interview)
                    { app.StageId = (int)EnumApplicationStage.Interview; app.UpdatedDate = DateTime.UtcNow; }
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.INTERVIEW_SAVE_SUCCESS, iv.Id);
                }

                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid interview id");
                var existing = await _ctx.Interviews!.FirstOrDefaultAsync(x => x.Id == id);
                if (existing == null) return Utilities.Warn("Interview not found", code: (int)HttpStatusCode.NotFound);

                if (isComplete)
                {
                    existing.OverallRating = request.OverallRating; existing.Recommendation = request.Recommendation;
                    existing.Strengths = request.Strengths; existing.Weaknesses = request.Weaknesses;
                    existing.Feedback = request.Feedback; existing.IsCompleted = true;
                }
                else
                {
                    existing.RoundName = request.RoundName; existing.InterviewModeId = request.InterviewModeId;
                    existing.ScheduledAt = request.ScheduledAt; existing.DurationMinutes = request.DurationMinutes;
                    existing.Location = request.Location; existing.MeetingLink = request.MeetingLink;
                }
                existing.UpdatedBy = request.CreateUpdateBy; existing.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(isComplete ? ConstantSupplier.INTERVIEW_COMPLETE_SUCCESS : ConstantSupplier.INTERVIEW_SAVE_SUCCESS, existing.Id);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateInterviewAsync));
            }
        }
    }

    // =========================================================================
    // JobOfferService — create/update + issue/accept/decline workflow
    // =========================================================================
    public class JobOfferService : IJobOfferService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public JobOfferService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetOfferByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid offer id");
                var offer = await _ctx.JobOffers!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == g);
                if (offer == null) return Utilities.Warn(ConstantSupplier.OFFER_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.OFFER_FETCH_SUCCESS, offer);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetOfferByIdAsync)); }
        }

        public async Task<DataResponse> CreateUpdateOfferAsync(OfferRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName, ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);
                if (!Guid.TryParse(request.JobApplicationId, out var appId)) return Utilities.Warn("Invalid application id");
                if (!Guid.TryParse(request.CandidateId, out var candId)) return Utilities.Warn("Invalid candidate id");

                Guid? posId   = Guid.TryParse(request.PositionId, out var p) ? p : null;
                Guid? desigId = Guid.TryParse(request.DesignationId, out var dg) ? dg : null;
                Guid? deptId  = Guid.TryParse(request.DepartmentId, out var d) ? d : null;

                if (isSave)
                {
                    var code = await NextCodeAsync("OFR");
                    var offer = new JobOffer
                    {
                        Id = Guid.NewGuid(), OfferCode = code, JobApplicationId = appId, CandidateId = candId,
                        PositionId = posId, DesignationId = desigId, DepartmentId = deptId,
                        OfferedSalary = request.OfferedSalary, SalaryCurrency = request.SalaryCurrency,
                        JoiningDate = request.JoiningDate, OfferValidTill = request.OfferValidTill,
                        OfferLetterUrl = request.OfferLetterUrl, StatusId = (int)EnumOfferStatus.Draft,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.JobOffers!.AddAsync(offer);
                    // move the application into the Offer stage
                    var app = await _ctx.JobApplications!.FirstOrDefaultAsync(a => a.Id == appId);
                    if (app != null) { app.StageId = (int)EnumApplicationStage.Offer; app.UpdatedDate = DateTime.UtcNow; }
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.OFFER_CREATE_SUCCESS, offer.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid id for update");
                    var offer = await _ctx.JobOffers!.FirstOrDefaultAsync(x => x.Id == id);
                    if (offer == null) return Utilities.Warn(ConstantSupplier.OFFER_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    if (offer.StatusId != (int)EnumOfferStatus.Draft) return Utilities.Warn(ConstantSupplier.OFFER_BAD_STATE);

                    offer.PositionId = posId; offer.DesignationId = desigId; offer.DepartmentId = deptId;
                    offer.OfferedSalary = request.OfferedSalary; offer.SalaryCurrency = request.SalaryCurrency;
                    offer.JoiningDate = request.JoiningDate; offer.OfferValidTill = request.OfferValidTill;
                    offer.OfferLetterUrl = request.OfferLetterUrl;
                    offer.UpdatedBy = request.CreateUpdateBy; offer.UpdatedDate = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.OFFER_UPDATE_SUCCESS, offer.Id);
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateOfferAsync));
            }
        }

        public Task<DataResponse> IssueOfferAsync(OfferActionRequest request)
            => TransitionAsync(request, from: new[] { (int)EnumOfferStatus.Draft }, to: (int)EnumOfferStatus.Issued,
                               okMsg: ConstantSupplier.OFFER_ISSUE_SUCCESS, stamp: o =>
                               { o.IssuedDate = DateTime.UtcNow; if (!string.IsNullOrWhiteSpace(request.OfferLetterUrl)) o.OfferLetterUrl = request.OfferLetterUrl; });

        public Task<DataResponse> AcceptOfferAsync(OfferActionRequest request)
            => TransitionAsync(request, from: new[] { (int)EnumOfferStatus.Issued }, to: (int)EnumOfferStatus.Accepted,
                               okMsg: ConstantSupplier.OFFER_ACCEPT_SUCCESS, stamp: o => o.RespondedDate = DateTime.UtcNow,
                               afterAsync: async o =>
                               {
                                   // mark the application Hired — onboarding picks it up from here
                                   var app = await _ctx.JobApplications!.FirstOrDefaultAsync(a => a.Id == o.JobApplicationId);
                                   if (app != null) { app.StageId = (int)EnumApplicationStage.Hired; app.UpdatedDate = DateTime.UtcNow; }
                               });

        public Task<DataResponse> DeclineOfferAsync(OfferActionRequest request)
            => TransitionAsync(request, from: new[] { (int)EnumOfferStatus.Issued }, to: (int)EnumOfferStatus.Declined,
                               okMsg: ConstantSupplier.OFFER_DECLINE_SUCCESS, stamp: o =>
                               { o.RespondedDate = DateTime.UtcNow; o.DeclineReason = request.DeclineReason; });

        private async Task<DataResponse> TransitionAsync(OfferActionRequest request, int[] from, int to,
            string okMsg, Action<JobOffer> stamp, Func<JobOffer, Task>? afterAsync = null)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid offer id");
                var offer = await _ctx.JobOffers!.FirstOrDefaultAsync(x => x.Id == id);
                if (offer == null) return Utilities.Warn(ConstantSupplier.OFFER_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (!from.Contains(offer.StatusId)) return Utilities.Warn(ConstantSupplier.OFFER_BAD_STATE);

                offer.StatusId = to;
                stamp(offer);
                offer.UpdatedBy = request.CreateUpdateBy; offer.UpdatedDate = DateTime.UtcNow;
                if (afterAsync != null) await afterAsync(offer);

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(okMsg, offer.Id);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(TransitionAsync));
            }
        }

        private async Task<string> NextCodeAsync(string prefix)
        {
            var last = await _ctx.JobOffers!.AsNoTracking()
                .Where(x => x.OfferCode!.StartsWith(prefix + "-"))
                .OrderByDescending(x => x.OfferCode).Select(x => x.OfferCode).FirstOrDefaultAsync();
            int n = 0;
            if (!string.IsNullOrEmpty(last) && int.TryParse(last.Substring(prefix.Length + 1), out var parsed)) n = parsed;
            return $"{prefix}-{(n + 1):D4}";
        }
    }
}
