using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    /// <summary>
    /// Position + Requisition services. EF Core handles CRUD (change-tracking +
    /// trigger-fired *Log rows); the JSON-returning paging SP backs the list view.
    /// Identical shape to DepartmentService.
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;

        public PositionService(HumanResourcesDBContext ctx, IHRLogService log, IOptions<AppSettings> opts)
        { _ctx = ctx; _log = log; _settings = opts.Value; }

        public async Task<DataResponse> GetAllPositionsAsync()
        {
            try
            {
                var list = await _ctx.Positions!.AsNoTracking()
                    .Where(p => p.IsActive == true)
                    .OrderBy(p => p.Title)
                    .Select(p => new PositionResponse
                    {
                        Id = p.Id, PositionCode = p.PositionCode, Title = p.Title,
                        Grade = p.Grade, BudgetedHeadcount = p.BudgetedHeadcount,
                        FilledHeadcount = p.FilledHeadcount,
                        OpenHeadcount = p.BudgetedHeadcount - p.FilledHeadcount,
                        StatusId = p.StatusId,
                        DepartmentId = p.DepartmentId, DesignationId = p.DesignationId,
                        CompanyId = p.CompanyId, BranchId = p.BranchId, CostCenterId = p.CostCenterId,
                        JobGradeId = p.JobGradeId, ReportsToPositionId = p.ReportsToPositionId,
                        EmploymentType = p.EmploymentType, WorkLocation = p.WorkLocation,
                        MinSalary = p.MinSalary, MaxSalary = p.MaxSalary, SalaryCurrency = p.SalaryCurrency,
                        IsActive = p.IsActive
                    })
                    .ToListAsync();

                if (Utilities.IsNull(list))
                    return Utilities.Warn(ConstantSupplier.POSITION_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllPositionsAsync)); }
        }

        public async Task<PagingResult<PositionResponse>?> GetAllPositionsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try
            {
                return await PagingHelper.ExecuteAsync<PositionResponse>(
                    _ctx, ConstantSupplier.SP_GET_ALL_POSITIONS_PAGING, filter);
            }
            catch (Exception ex)
            {
                Utilities.Exception(ex, _log, nameof(GetAllPositionsPagingWithSearchAsync));
                return null;
            }
        }

        public async Task<DataResponse> CreateUpdatePositionAsync(PositionRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName, ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);
                Guid? deptId    = Utilities.ToGuid(request.DepartmentId);
                Guid? desigId   = Utilities.ToGuid(request.DesignationId);
                Guid? reports   = Utilities.ToGuid(request.ReportsToPositionId);
                Guid? companyId = Utilities.ToGuid(request.CompanyId);
                Guid? branchId  = Utilities.ToGuid(request.BranchId);
                Guid? ccId      = Utilities.ToGuid(request.CostCenterId);
                Guid? gradeId   = Utilities.ToGuid(request.JobGradeId);

                if (request.BudgetedHeadcount < 1) return Utilities.Warn("Budgeted headcount must be at least 1.");
                if (request.MinSalary.HasValue && request.MaxSalary.HasValue && request.MinSalary > request.MaxSalary)
                    return Utilities.Warn("Minimum salary cannot exceed maximum salary.");

                // default the legal entity to the company of the chosen branch / department
                if (companyId == null && branchId != null)
                    companyId = await _ctx.Branches!.Where(b => b.Id == branchId).Select(b => (Guid?)b.CompanyId).FirstOrDefaultAsync();
                if (companyId == null && deptId != null)
                    companyId = await _ctx.Departments!.Where(d => d.Id == deptId).Select(d => (Guid?)d.CompanyId).FirstOrDefaultAsync();

                if (isSave)
                {
                    bool dup = await _ctx.Positions!.AnyAsync(p =>
                        p.Title == request.Title && p.DepartmentId == deptId && p.IsActive == true);
                    if (dup) return Utilities.Warn(ConstantSupplier.POSITION_DUPLICATE);

                    var code = await NextCodeAsync(ConstantSupplier.CODE_PREFIX_POSITION);
                    var pos = new Position
                    {
                        Id = Guid.NewGuid(), PositionCode = code, Title = request.Title,
                        DesignationId = desigId, DepartmentId = deptId, ReportsToPositionId = reports,
                        CompanyId = companyId, BranchId = branchId, CostCenterId = ccId, JobGradeId = gradeId,
                        Grade = request.Grade, BudgetedHeadcount = request.BudgetedHeadcount, FilledHeadcount = 0,
                        StatusId = (int)EnumPositionStatus.Open,
                        EmploymentType = request.EmploymentType, WorkLocation = request.WorkLocation,
                        MinSalary = request.MinSalary, MaxSalary = request.MaxSalary, SalaryCurrency = request.SalaryCurrency,
                        EffectiveFrom = request.EffectiveFrom, EffectiveTo = request.EffectiveTo, Description = request.Description,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = request.IsActive ?? true
                    };
                    await _ctx.Positions!.AddAsync(pos);
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.POSITION_CREATE_SUCCESS, pos.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid id for update");
                    var pos = await _ctx.Positions!.FirstOrDefaultAsync(p => p.Id == id);
                    if (pos == null) return Utilities.Warn(ConstantSupplier.POSITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    if (reports == pos.Id) return Utilities.Warn("A position cannot report to itself.");
                    if (request.BudgetedHeadcount < pos.FilledHeadcount)
                        return Utilities.Warn($"Budgeted headcount cannot be below the {pos.FilledHeadcount} seat(s) already filled.");

                    pos.Title = request.Title; pos.DesignationId = desigId; pos.DepartmentId = deptId;
                    pos.ReportsToPositionId = reports; pos.Grade = request.Grade;
                    pos.CompanyId = companyId; pos.BranchId = branchId; pos.CostCenterId = ccId; pos.JobGradeId = gradeId;
                    pos.BudgetedHeadcount = request.BudgetedHeadcount; pos.EmploymentType = request.EmploymentType;
                    pos.WorkLocation = request.WorkLocation; pos.MinSalary = request.MinSalary;
                    pos.MaxSalary = request.MaxSalary; pos.SalaryCurrency = request.SalaryCurrency;
                    pos.EffectiveFrom = request.EffectiveFrom; pos.EffectiveTo = request.EffectiveTo; pos.Description = request.Description;
                    pos.UpdatedBy = request.CreateUpdateBy; pos.UpdatedDate = DateTime.UtcNow;
                    pos.IsActive = request.IsActive ?? true;

                    // keep status coherent with headcount unless explicitly on hold / closed / draft
                    if (pos.StatusId == (int)EnumPositionStatus.Open || pos.StatusId == (int)EnumPositionStatus.Filled)
                        pos.StatusId = pos.FilledHeadcount >= pos.BudgetedHeadcount
                            ? (int)EnumPositionStatus.Filled : (int)EnumPositionStatus.Open;

                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.POSITION_UPDATE_SUCCESS, pos.Id);
                }
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueViolation(dbEx))
            {
                await tx.RollbackAsync();
                return Utilities.Warn(ConstantSupplier.POSITION_DUPLICATE);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdatePositionAsync));
            }
        }

        public async Task<DataResponse> DeletePositionAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid position id");
                var pos = await _ctx.Positions!.FirstOrDefaultAsync(p => p.Id == g);
                if (pos == null) return Utilities.Warn(ConstantSupplier.POSITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (pos.FilledHeadcount > 0)
                    return Utilities.Warn("Position is occupied; release its holders before removing.");

                bool referenced = await _ctx.ManpowerRequisitions!.AnyAsync(m => m.PositionId == g)
                               || await _ctx.Employees!.AnyAsync(e => e.PositionId == g)
                               || await _ctx.Positions!.AnyAsync(p => p.ReportsToPositionId == g && p.IsActive == true);
                if (referenced)
                {
                    pos.IsActive = false; pos.StatusId = (int)EnumPositionStatus.Closed; pos.UpdatedDate = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync();
                    return Utilities.Warn(ConstantSupplier.POSITION_HAS_REQUISITION, pos.Id);
                }

                bool isHardDelete = await _ctx.HRSettings!.AsNoTracking()
                    .OrderByDescending(x => x.CreatedDate).Select(x => x.IsHardDelete ?? false).FirstOrDefaultAsync();

                if (isHardDelete) _ctx.Positions!.Remove(pos);
                else { pos.IsActive = false; pos.StatusId = (int)EnumPositionStatus.Closed; pos.UpdatedDate = DateTime.UtcNow; }
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.POSITION_DELETE_SUCCESS, pos.Id);
            }
            catch (DbUpdateException dbEx) when (Utilities.IsForeignKeyViolation(dbEx))
            {
                return Utilities.Warn(ConstantSupplier.POSITION_HAS_REQUISITION);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeletePositionAsync)); }
        }

        public async Task<DataResponse> SetFillAsync(PositionFillRequest r)
        {
            try
            {
                if (!Guid.TryParse(r.PositionId, out var id)) return Utilities.Warn("Invalid position id");
                if (r.Delta != 1 && r.Delta != -1) return Utilities.Warn("Delta must be +1 (assign) or -1 (release).");
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_POSITION_FILL,
                    ("@PositionId", id), ("@Delta", r.Delta), ("@UpdatedBy", r.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, id) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetFillAsync)); }
        }

        public async Task<DataResponse> SetStatusAsync(PositionStatusRequest r)
        {
            try
            {
                if (!Guid.TryParse(r.Id, out var id)) return Utilities.Warn("Invalid position id");
                var (rows, msg) = await AdoHelper.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_POSITION_STATUS,
                    ("@Id", id), ("@StatusId", r.StatusId), ("@UpdatedBy", r.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, id) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetStatusAsync)); }
        }

        public async Task<DataResponse> GetOrgTreeAsync(string? rootPositionId)
        {
            try
            {
                var root = Utilities.ToGuid(rootPositionId);
                var json = await AdoHelper.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_POSITION_ORG_TREE, ("@RootPositionId", root));
                var flat = string.IsNullOrWhiteSpace(json) ? new JArray() : JArray.Parse(json!);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, flat);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetOrgTreeAsync)); }
        }

        private async Task<string> NextCodeAsync(string prefix)
        {
            var codes = await _ctx.Positions!.AsNoTracking()
                .Where(p => p.PositionCode!.StartsWith(prefix + "-"))
                .Select(p => p.PositionCode).ToListAsync();
            return AdoHelper.NextCode(prefix, codes);
        }
    }

    public class RequisitionService : IRequisitionService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public RequisitionService(HumanResourcesDBContext ctx, IHRLogService log)
        { _ctx = ctx; _log = log; }

        public async Task<PagingResult<RequisitionResponse>?> GetAllRequisitionsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try
            {
                return await PagingHelper.ExecuteAsync<RequisitionResponse>(
                    _ctx, ConstantSupplier.SP_GET_ALL_REQUISITIONS_PAGING, filter);
            }
            catch (Exception ex)
            {
                Utilities.Exception(ex, _log, nameof(GetAllRequisitionsPagingWithSearchAsync));
                return null;
            }
        }

        public async Task<DataResponse> GetRequisitionByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid requisition id");
                var r = await _ctx.ManpowerRequisitions!.AsNoTracking()
                    .Where(x => x.Id == g)
                    .Select(x => new RequisitionResponse
                    {
                        Id = x.Id, RequisitionCode = x.RequisitionCode, NumberOfVacancies = x.NumberOfVacancies,
                        RequiredByDate = x.RequiredByDate, IsReplacement = x.IsReplacement, StatusId = x.StatusId,
                        PositionId = x.PositionId, PositionTitle = x.Position != null ? x.Position.Title : null,
                        Justification = x.Justification, ApprovalRemark = x.ApprovalRemark
                    })
                    .FirstOrDefaultAsync();
                if (r == null) return Utilities.Warn(ConstantSupplier.REQUISITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.REQUISITION_FETCH_SUCCESS, r);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetRequisitionByIdAsync)); }
        }

        public async Task<DataResponse> CreateUpdateRequisitionAsync(RequisitionRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName, ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);
                Guid posId   = Guid.TryParse(request.PositionId, out var p) ? p : Guid.Empty;
                Guid? deptId  = Guid.TryParse(request.DepartmentId, out var d) ? d : null;
                Guid? desigId = Guid.TryParse(request.DesignationId, out var g) ? g : null;
                Guid? reqBy   = Guid.TryParse(request.RequestedById, out var rb) ? rb : null;
                Guid? replEmp = Guid.TryParse(request.ReplacingEmployeeId, out var re) ? re : null;

                if (isSave)
                {
                    var code = await NextCodeAsync("MRF");
                    var mrf = new ManpowerRequisition
                    {
                        Id = Guid.NewGuid(), RequisitionCode = code, PositionId = posId,
                        DepartmentId = deptId, DesignationId = desigId, NumberOfVacancies = request.NumberOfVacancies,
                        RequestedById = reqBy, Justification = request.Justification, IsReplacement = request.IsReplacement,
                        ReplacingEmployeeId = replEmp, RequiredByDate = request.RequiredByDate,
                        MinExperienceYears = request.MinExperienceYears, RequiredQualification = request.RequiredQualification,
                        BudgetMin = request.BudgetMin, BudgetMax = request.BudgetMax,
                        StatusId = (int)EnumRequisitionStatus.Draft,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = request.IsActive ?? true
                    };
                    await _ctx.ManpowerRequisitions!.AddAsync(mrf);
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.REQUISITION_CREATE_SUCCESS, mrf.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid id for update");
                    var mrf = await _ctx.ManpowerRequisitions!.FirstOrDefaultAsync(x => x.Id == id);
                    if (mrf == null) return Utilities.Warn(ConstantSupplier.REQUISITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    // only Draft/Pending are editable
                    if (mrf.StatusId != (int)EnumRequisitionStatus.Draft && mrf.StatusId != (int)EnumRequisitionStatus.Pending)
                        return Utilities.Warn(ConstantSupplier.REQUISITION_BAD_STATE);

                    mrf.PositionId = posId; mrf.DepartmentId = deptId; mrf.DesignationId = desigId;
                    mrf.NumberOfVacancies = request.NumberOfVacancies; mrf.Justification = request.Justification;
                    mrf.IsReplacement = request.IsReplacement; mrf.ReplacingEmployeeId = replEmp;
                    mrf.RequiredByDate = request.RequiredByDate; mrf.MinExperienceYears = request.MinExperienceYears;
                    mrf.RequiredQualification = request.RequiredQualification;
                    mrf.BudgetMin = request.BudgetMin; mrf.BudgetMax = request.BudgetMax;
                    mrf.UpdatedBy = request.CreateUpdateBy; mrf.UpdatedDate = DateTime.UtcNow;
                    mrf.IsActive = request.IsActive ?? true;

                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.REQUISITION_UPDATE_SUCCESS, mrf.Id);
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateRequisitionAsync));
            }
        }

        public Task<DataResponse> SubmitRequisitionAsync(RequisitionActionRequest request)
            => TransitionAsync(request, fromStatuses: new[] { (int)EnumRequisitionStatus.Draft },
                               toStatus: (int)EnumRequisitionStatus.Pending,
                               okMsg: ConstantSupplier.REQUISITION_SUBMIT_SUCCESS, setApprover: false);

        public Task<DataResponse> ApproveRequisitionAsync(RequisitionActionRequest request)
            => TransitionAsync(request, fromStatuses: new[] { (int)EnumRequisitionStatus.Pending },
                               toStatus: (int)EnumRequisitionStatus.Approved,
                               okMsg: ConstantSupplier.REQUISITION_APPROVE_SUCCESS, setApprover: true);

        public Task<DataResponse> RejectRequisitionAsync(RequisitionActionRequest request)
            => TransitionAsync(request, fromStatuses: new[] { (int)EnumRequisitionStatus.Pending },
                               toStatus: (int)EnumRequisitionStatus.Rejected,
                               okMsg: ConstantSupplier.REQUISITION_REJECT_SUCCESS, setApprover: true);

        private async Task<DataResponse> TransitionAsync(RequisitionActionRequest request, int[] fromStatuses,
                                                         int toStatus, string okMsg, bool setApprover)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid requisition id");
                var mrf = await _ctx.ManpowerRequisitions!.FirstOrDefaultAsync(x => x.Id == id);
                if (mrf == null) return Utilities.Warn(ConstantSupplier.REQUISITION_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (!fromStatuses.Contains(mrf.StatusId)) return Utilities.Warn(ConstantSupplier.REQUISITION_BAD_STATE);

                mrf.StatusId = toStatus;
                if (setApprover)
                {
                    mrf.ApprovedById = Guid.TryParse(request.ApprovedById, out var a) ? a : null;
                    mrf.ApprovedDate = DateTime.UtcNow;
                    mrf.ApprovalRemark = request.ApprovalRemark;
                }
                mrf.UpdatedBy = request.CreateUpdateBy; mrf.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(okMsg, mrf.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(TransitionAsync)); }
        }

        private async Task<string> NextCodeAsync(string prefix)
        {
            var last = await _ctx.ManpowerRequisitions!.AsNoTracking()
                .Where(x => x.RequisitionCode!.StartsWith(prefix + "-"))
                .OrderByDescending(x => x.RequisitionCode).Select(x => x.RequisitionCode).FirstOrDefaultAsync();
            int n = 0;
            if (!string.IsNullOrEmpty(last) && int.TryParse(last.Substring(prefix.Length + 1), out var parsed)) n = parsed;
            return $"{prefix}-{(n + 1):D4}";
        }
    }

    /// <summary>
    /// Shared paging helper — runs a JSON-returning paging SP and shapes the
    /// RowCount/CurrentPage/PageSize/PageCount/Items envelope into PagingResult&lt;T&gt;.
    /// Same logic DepartmentService inlines; centralised so every recruitment
    /// service reuses it.
    /// </summary>
    public static class PagingHelper
    {
        public static async Task<PagingResult<T>?> ExecuteAsync<T>(
            HumanResourcesDBContext ctx, string spName, PagingSearchFilter filter)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"EXEC dbo.{spName} @SearchTerm=@p0, @SortColumnName=@p1, " +
                              "@SortColumnDirection=@p2, @PageNumber=@p3, @PageSize=@p4";
            AddParam(cmd, "@p0", filter.SearchTerm);
            AddParam(cmd, "@p1", filter.SortColumnName);
            AddParam(cmd, "@p2", filter.SortColumnDirection);
            AddParam(cmd, "@p3", filter.PageNumber);
            AddParam(cmd, "@p4", filter.PageSize);

            var raw = (await cmd.ExecuteScalarAsync()) as string;
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var jo = JObject.Parse(raw);
            var items = jo["Items"]?.ToString();
            return new PagingResult<T>
            {
                RowCount    = jo.Value<int>("RowCount"),
                CurrentPage = jo.Value<int>("CurrentPage"),
                PageSize    = jo.Value<int>("PageSize"),
                PageCount   = jo.Value<int>("PageCount"),
                Items       = string.IsNullOrWhiteSpace(items)
                              ? new List<T>()
                              : JsonConvert.DeserializeObject<List<T>>(items) ?? new List<T>()
            };
        }

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
