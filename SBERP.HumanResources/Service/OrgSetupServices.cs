using Microsoft.EntityFrameworkCore;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Net;

namespace SBERP.HumanResources.Service
{
    public interface ICostCenterService
    {
        Task<DataResponse> GetAllCostCentersAsync();
        Task<PagingResult<CostCenterResponse>?> GetAllCostCentersPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateCostCenterAsync(CostCenterRequest request);
        Task<DataResponse> DeleteCostCenterAsync(string id);
    }

    public interface IJobGradeService
    {
        Task<DataResponse> GetAllJobGradesAsync();
        Task<PagingResult<JobGradeResponse>?> GetAllJobGradesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateJobGradeAsync(JobGradeRequest request);
        Task<DataResponse> DeleteJobGradeAsync(string id);
    }

    public interface IHolidayCalendarService
    {
        Task<DataResponse> GetAllHolidayCalendarsAsync();
        Task<PagingResult<HolidayCalendarResponse>?> GetAllHolidayCalendarsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateHolidayCalendarAsync(HolidayCalendarRequest request);
        Task<DataResponse> DeleteHolidayCalendarAsync(string id);
        Task<DataResponse> GetHolidayDatesAsync(string calendarId);
        Task<DataResponse> CreateUpdateHolidayDateAsync(HolidayDateRequest request);
        Task<DataResponse> DeleteHolidayDateAsync(string id);
    }

    // =========================================================================
    // Cost Center
    // =========================================================================
    public class CostCenterService : ICostCenterService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public CostCenterService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetAllCostCentersAsync()
        {
            try
            {
                var list = await _ctx.CostCenters!.AsNoTracking().Where(x => x.IsActive == true).OrderBy(x => x.Name)
                    .Select(x => new CostCenterResponse
                    {
                        Id = x.Id, CostCenterCode = x.CostCenterCode, Name = x.Name, CompanyId = x.CompanyId,
                        BranchId = x.BranchId, ParentCostCenterId = x.ParentCostCenterId, IsActive = x.IsActive
                    }).ToListAsync();
                if (Utilities.IsNull(list)) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCostCentersAsync)); }
        }

        public async Task<PagingResult<CostCenterResponse>?> GetAllCostCentersPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<CostCenterResponse>(_ctx, ConstantSupplier.SP_GET_ALL_COSTCENTERS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllCostCentersPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateCostCenterAsync(CostCenterRequest r)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var companyId = Utilities.ToGuid(r.CompanyId);
                var branchId  = Utilities.ToGuid(r.BranchId);
                var parentId  = Utilities.ToGuid(r.ParentCostCenterId);
                var ownerId   = Utilities.ToGuid(r.OwnerEmployeeId);
                bool isUpdate = string.Equals(r.ActionName, ConstantSupplier.UPDATE_KEY, StringComparison.OrdinalIgnoreCase);
                var excludeId = isUpdate ? Utilities.ToGuid(r.Id) : null;

                if (await _ctx.CostCenters!.AnyAsync(x => x.Name == r.Name && x.CompanyId == companyId && x.IsActive == true && x.Id != excludeId))
                    return Utilities.Warn(ConstantSupplier.COSTCENTER_DUPLICATE);

                CostCenter entity;
                if (isUpdate)
                {
                    if (excludeId == null) return Utilities.Warn("Invalid cost center id");
                    if (parentId == excludeId) return Utilities.Warn("A cost center cannot be its own parent.");
                    entity = (await _ctx.CostCenters!.FirstOrDefaultAsync(x => x.Id == excludeId))!;
                    if (entity == null) return Utilities.Warn(ConstantSupplier.COSTCENTER_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    entity.UpdatedBy = r.CreateUpdateBy; entity.UpdatedDate = DateTime.UtcNow;
                    entity.IsActive = r.IsActive ?? entity.IsActive;
                }
                else
                {
                    var codes = await _ctx.CostCenters!.AsNoTracking().Select(x => x.CostCenterCode).ToListAsync();
                    entity = new CostCenter
                    {
                        Id = Guid.NewGuid(), CostCenterCode = AdoHelper.NextCode(ConstantSupplier.CODE_PREFIX_COSTCENTER, codes),
                        CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.CostCenters!.AddAsync(entity);
                }
                entity.Name = r.Name; entity.CompanyId = companyId; entity.BranchId = branchId;
                entity.ParentCostCenterId = parentId; entity.OwnerEmployeeId = ownerId; entity.Description = r.Description;

                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.COSTCENTER_SAVE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateCostCenterAsync)); }
        }

        public async Task<DataResponse> DeleteCostCenterAsync(string id)
        {
            try
            {
                var gid = Utilities.ToGuid(id);
                var entity = await _ctx.CostCenters!.FirstOrDefaultAsync(x => x.Id == gid);
                if (entity == null) return Utilities.Warn(ConstantSupplier.COSTCENTER_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                bool inUse = await _ctx.Employees!.AnyAsync(e => e.CostCenterId == gid && e.IsActive == true)
                          || await _ctx.Positions!.AnyAsync(p => p.CostCenterId == gid && p.IsActive == true)
                          || await _ctx.CostCenters!.AnyAsync(c => c.ParentCostCenterId == gid && c.IsActive == true);
                if (inUse) return Utilities.Warn("Cost center is still used by employees, positions or child cost centers.");
                entity.IsActive = false; entity.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.COSTCENTER_DELETE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteCostCenterAsync)); }
        }
    }

    // =========================================================================
    // Job Grade
    // =========================================================================
    public class JobGradeService : IJobGradeService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public JobGradeService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetAllJobGradesAsync()
        {
            try
            {
                var list = await _ctx.JobGrades!.AsNoTracking().Where(x => x.IsActive == true).OrderBy(x => x.GradeLevel)
                    .Select(x => new JobGradeResponse
                    {
                        Id = x.Id, GradeCode = x.GradeCode, Name = x.Name, GradeLevel = x.GradeLevel, Band = x.Band,
                        RefMinSalary = x.RefMinSalary, RefMidSalary = x.RefMidSalary, RefMaxSalary = x.RefMaxSalary,
                        CurrencyCode = x.CurrencyCode, IsActive = x.IsActive
                    }).ToListAsync();
                if (Utilities.IsNull(list)) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllJobGradesAsync)); }
        }

        public async Task<PagingResult<JobGradeResponse>?> GetAllJobGradesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<JobGradeResponse>(_ctx, ConstantSupplier.SP_GET_ALL_JOBGRADES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllJobGradesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateJobGradeAsync(JobGradeRequest r)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (r.RefMinSalary.HasValue && r.RefMaxSalary.HasValue && r.RefMinSalary > r.RefMaxSalary)
                    return Utilities.Warn("Reference minimum salary cannot exceed the maximum.");
                bool isUpdate = string.Equals(r.ActionName, ConstantSupplier.UPDATE_KEY, StringComparison.OrdinalIgnoreCase);
                var excludeId = isUpdate ? Utilities.ToGuid(r.Id) : null;
                if (await _ctx.JobGrades!.AnyAsync(x => (x.Name == r.Name || x.GradeLevel == r.GradeLevel) && x.IsActive == true && x.Id != excludeId))
                    return Utilities.Warn(ConstantSupplier.JOBGRADE_DUPLICATE);

                JobGrade entity;
                if (isUpdate)
                {
                    entity = (await _ctx.JobGrades!.FirstOrDefaultAsync(x => x.Id == excludeId))!;
                    if (entity == null) return Utilities.Warn(ConstantSupplier.JOBGRADE_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    entity.UpdatedBy = r.CreateUpdateBy; entity.UpdatedDate = DateTime.UtcNow;
                    entity.IsActive = r.IsActive ?? entity.IsActive;
                }
                else
                {
                    var codes = await _ctx.JobGrades!.AsNoTracking().Select(x => x.GradeCode).ToListAsync();
                    entity = new JobGrade
                    {
                        Id = Guid.NewGuid(), GradeCode = AdoHelper.NextCode(ConstantSupplier.CODE_PREFIX_JOBGRADE, codes),
                        CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.JobGrades!.AddAsync(entity);
                }
                entity.Name = r.Name; entity.GradeLevel = r.GradeLevel; entity.Band = r.Band;
                entity.RefMinSalary = r.RefMinSalary; entity.RefMidSalary = r.RefMidSalary; entity.RefMaxSalary = r.RefMaxSalary;
                entity.CurrencyCode = r.CurrencyCode; entity.Description = r.Description;

                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.JOBGRADE_SAVE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateJobGradeAsync)); }
        }

        public async Task<DataResponse> DeleteJobGradeAsync(string id)
        {
            try
            {
                var gid = Utilities.ToGuid(id);
                var entity = await _ctx.JobGrades!.FirstOrDefaultAsync(x => x.Id == gid);
                if (entity == null) return Utilities.Warn(ConstantSupplier.JOBGRADE_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                bool inUse = await _ctx.Employees!.AnyAsync(e => e.JobGradeId == gid && e.IsActive == true)
                          || await _ctx.Positions!.AnyAsync(p => p.JobGradeId == gid && p.IsActive == true);
                if (inUse) return Utilities.Warn("Job grade is still assigned to employees or positions.");
                entity.IsActive = false; entity.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.JOBGRADE_DELETE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteJobGradeAsync)); }
        }
    }

    // =========================================================================
    // Holiday Calendar (+ dates)
    // =========================================================================
    public class HolidayCalendarService : IHolidayCalendarService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public HolidayCalendarService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetAllHolidayCalendarsAsync()
        {
            try
            {
                var list = await _ctx.HolidayCalendars!.AsNoTracking().Where(x => x.IsActive == true)
                    .OrderByDescending(x => x.Year).ThenBy(x => x.Name)
                    .Select(x => new HolidayCalendarResponse
                    {
                        Id = x.Id, CalendarCode = x.CalendarCode, Name = x.Name, Year = x.Year, Country = x.Country,
                        HolidayCount = x.HolidayDates!.Count(d => d.IsActive == true), IsActive = x.IsActive
                    }).ToListAsync();
                if (Utilities.IsNull(list)) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllHolidayCalendarsAsync)); }
        }

        public async Task<PagingResult<HolidayCalendarResponse>?> GetAllHolidayCalendarsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<HolidayCalendarResponse>(_ctx, ConstantSupplier.SP_GET_ALL_HOLIDAYCALENDARS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllHolidayCalendarsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateHolidayCalendarAsync(HolidayCalendarRequest r)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isUpdate = string.Equals(r.ActionName, ConstantSupplier.UPDATE_KEY, StringComparison.OrdinalIgnoreCase);
                var excludeId = isUpdate ? Utilities.ToGuid(r.Id) : null;
                if (await _ctx.HolidayCalendars!.AnyAsync(x => x.Name == r.Name && x.IsActive == true && x.Id != excludeId))
                    return Utilities.Warn("A holiday calendar with the same name already exists.");

                HolidayCalendar entity;
                if (isUpdate)
                {
                    entity = (await _ctx.HolidayCalendars!.FirstOrDefaultAsync(x => x.Id == excludeId))!;
                    if (entity == null) return Utilities.Warn(ConstantSupplier.CALENDAR_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    entity.UpdatedBy = r.CreateUpdateBy; entity.UpdatedDate = DateTime.UtcNow;
                    entity.IsActive = r.IsActive ?? entity.IsActive;
                }
                else
                {
                    var codes = await _ctx.HolidayCalendars!.AsNoTracking().Select(x => x.CalendarCode).ToListAsync();
                    entity = new HolidayCalendar
                    {
                        Id = Guid.NewGuid(), CalendarCode = AdoHelper.NextCode(ConstantSupplier.CODE_PREFIX_CALENDAR, codes),
                        CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.HolidayCalendars!.AddAsync(entity);
                }
                entity.Name = r.Name; entity.Year = r.Year; entity.Country = r.Country; entity.Description = r.Description;

                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.CALENDAR_SAVE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateHolidayCalendarAsync)); }
        }

        public async Task<DataResponse> DeleteHolidayCalendarAsync(string id)
        {
            try
            {
                var gid = Utilities.ToGuid(id);
                var entity = await _ctx.HolidayCalendars!.FirstOrDefaultAsync(x => x.Id == gid);
                if (entity == null) return Utilities.Warn(ConstantSupplier.CALENDAR_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                if (await _ctx.Branches!.AnyAsync(b => b.HolidayCalendarId == gid && b.IsActive == true))
                    return Utilities.Warn("Calendar is assigned to one or more branches; reassign them first.");
                entity.IsActive = false; entity.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.CALENDAR_DELETE_SUCCESS, entity.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteHolidayCalendarAsync)); }
        }

        public async Task<DataResponse> GetHolidayDatesAsync(string calendarId)
        {
            try
            {
                var cid = Utilities.ToGuid(calendarId);
                if (cid == null) return Utilities.Warn("Invalid calendar id");
                var list = await (from d in _ctx.HolidayDates!.AsNoTracking()
                                  join t in _ctx.HolidayTypes!.AsNoTracking() on d.HolidayTypeId equals t.Id into tj
                                  from t in tj.DefaultIfEmpty()
                                  where d.HolidayCalendarId == cid && d.IsActive == true
                                  orderby d.HolidayDate1
                                  select new HolidayDateResponse
                                  {
                                      Id = d.Id, HolidayCalendarId = d.HolidayCalendarId, Name = d.Name,
                                      HolidayDate = d.HolidayDate1, HolidayTypeId = d.HolidayTypeId,
                                      HolidayTypeName = t != null ? t.Name : null, IsRecurring = d.IsRecurring,
                                      Description = d.Description
                                  }).ToListAsync();
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetHolidayDatesAsync)); }
        }

        public async Task<DataResponse> CreateUpdateHolidayDateAsync(HolidayDateRequest r)
        {
            try
            {
                var calId = Utilities.ToGuid(r.HolidayCalendarId);
                if (calId == null || !await _ctx.HolidayCalendars!.AnyAsync(c => c.Id == calId))
                    return Utilities.Warn(ConstantSupplier.CALENDAR_NOT_FOUND);
                var date = r.HolidayDate.Date;
                bool isUpdate = string.Equals(r.ActionName, ConstantSupplier.UPDATE_KEY, StringComparison.OrdinalIgnoreCase);
                var excludeId = isUpdate ? Utilities.ToGuid(r.Id) : null;
                if (await _ctx.HolidayDates!.AnyAsync(h => h.HolidayCalendarId == calId && h.HolidayDate1 == date && h.IsActive == true && h.Id != excludeId))
                    return Utilities.Warn("That date is already a holiday in this calendar.");

                HolidayDate h;
                if (isUpdate)
                {
                    h = (await _ctx.HolidayDates!.FirstOrDefaultAsync(x => x.Id == excludeId))!;
                    if (h == null) return Utilities.Warn(ConstantSupplier.HOLIDAY_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                    h.UpdatedBy = r.CreateUpdateBy; h.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    h = new HolidayDate { Id = Guid.NewGuid(), HolidayCalendarId = calId.Value, CreatedBy = r.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true };
                    await _ctx.HolidayDates!.AddAsync(h);
                }
                h.Name = r.Name; h.HolidayDate1 = date; h.HolidayTypeId = r.HolidayTypeId < 1 ? 1 : r.HolidayTypeId;
                h.IsRecurring = r.IsRecurring; h.Description = r.Description;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.HOLIDAY_SAVE_SUCCESS, h.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateHolidayDateAsync)); }
        }

        public async Task<DataResponse> DeleteHolidayDateAsync(string id)
        {
            try
            {
                var gid = Utilities.ToGuid(id);
                var h = await _ctx.HolidayDates!.FirstOrDefaultAsync(x => x.Id == gid);
                if (h == null) return Utilities.Warn(ConstantSupplier.HOLIDAY_NOT_FOUND, code: (int)HttpStatusCode.NotFound);
                _ctx.HolidayDates!.Remove(h);   // child rows of a calendar — no audit trail needed
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.HOLIDAY_DELETE_SUCCESS, gid);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteHolidayDateAsync)); }
        }
    }
}
