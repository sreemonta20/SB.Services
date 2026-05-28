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
    /// EF Core-first implementation. Stored procedures back the paging/search
    /// query because raw JSON output is much cheaper than materializing nested
    /// EF graphs for a list view. CRUD goes through EF for change-tracking +
    /// trigger-fired audit rows.
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;

        public DepartmentService(
            HumanResourcesDBContext ctx,
            IHRLogService log,
            IOptions<AppSettings> opts)
        {
            _ctx = ctx;
            _log = log;
            _settings = opts.Value;
        }

        public async Task<DataResponse> GetAllDepartmentsAsync()
        {
            try
            {
                var list = await _ctx.Departments!
                    .AsNoTracking()
                    .Where(d => d.IsActive == true)
                    .OrderBy(d => d.Name)
                    .Select(d => new DepartmentResponse
                    {
                        Id = d.Id,
                        DepartmentCode = d.DepartmentCode,
                        Name = d.Name,
                        Description = d.Description,
                        ParentDepartmentId = d.ParentDepartmentId,
                        IsActive = d.IsActive
                    })
                    .ToListAsync();

                if (Utilities.IsNull(list))
                    return Utilities.Warn(ConstantSupplier.DEPARTMENT_LIST_EMPTY,
                                          code: (int)HttpStatusCode.NotFound);

                return Utilities.Ok(ConstantSupplier.DEPARTMENT_FETCH_SUCCESS, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDepartmentsAsync)); }
        }

        public async Task<PagingResult<DepartmentResponse>?> GetAllDepartmentsPagingWithSearchAsync(
            PagingSearchFilter filter)
        {
            try
            {
                // Call the JSON-returning SP and shape it into PagingResult<T>.
                var sql = "EXEC dbo." + ConstantSupplier.SP_GET_ALL_DEPARTMENTS_PAGING +
                          " @SearchTerm = {0}, @SortColumnName = {1}, " +
                          "@SortColumnDirection = {2}, @PageNumber = {3}, @PageSize = {4}";

                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql.Replace("{0}", "@p0").Replace("{1}", "@p1")
                                     .Replace("{2}", "@p2").Replace("{3}", "@p3")
                                     .Replace("{4}", "@p4");

                AddParam(cmd, "@p0", filter.SearchTerm);
                AddParam(cmd, "@p1", filter.SortColumnName);
                AddParam(cmd, "@p2", filter.SortColumnDirection);
                AddParam(cmd, "@p3", filter.PageNumber);
                AddParam(cmd, "@p4", filter.PageSize);

                var raw = (string?)await cmd.ExecuteScalarAsync();
                if (string.IsNullOrWhiteSpace(raw)) return null;

                // The SP returns a JSON object with RowCount/CurrentPage/PageSize/PageCount/Items
                var jo = JObject.Parse(raw);
                var items = jo["Items"]?.ToString();

                var result = new PagingResult<DepartmentResponse>
                {
                    RowCount    = jo.Value<int>("RowCount"),
                    CurrentPage = jo.Value<int>("CurrentPage"),
                    PageSize    = jo.Value<int>("PageSize"),
                    PageCount   = jo.Value<int>("PageCount"),
                    Items       = string.IsNullOrWhiteSpace(items)
                                  ? new List<DepartmentResponse>()
                                  : JsonConvert.DeserializeObject<List<DepartmentResponse>>(items)
                };
                return result;
            }
            catch (Exception ex)
            {
                Utilities.Exception(ex, _log, nameof(GetAllDepartmentsPagingWithSearchAsync));
                return null;
            }
        }

        public async Task<DataResponse> GetDepartmentByIdAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g))
                    return Utilities.Warn("Invalid department id");

                var dept = await _ctx.Departments!
                    .AsNoTracking()
                    .Where(d => d.Id == g)
                    .Select(d => new DepartmentResponse
                    {
                        Id = d.Id,
                        DepartmentCode = d.DepartmentCode,
                        Name = d.Name,
                        Description = d.Description,
                        ParentDepartmentId = d.ParentDepartmentId,
                        ParentDepartmentName = d.ParentDepartment != null ? d.ParentDepartment.Name : null,
                        HeadEmployeeId = d.HeadEmployeeId,
                        IsActive = d.IsActive,
                        CreatedDate = d.CreatedDate,
                        UpdatedDate = d.UpdatedDate
                    })
                    .FirstOrDefaultAsync();

                if (dept == null)
                    return Utilities.Warn(ConstantSupplier.DEPARTMENT_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                return Utilities.Ok(ConstantSupplier.DEPARTMENT_FETCH_SUCCESS, dept);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetDepartmentByIdAsync)); }
        }

        public async Task<DataResponse> CreateUpdateDepartmentAsync(DepartmentRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName,
                    ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);

                Guid? parentId = Guid.TryParse(request.ParentDepartmentId, out var p) ? p : null;
                Guid? headId   = Guid.TryParse(request.HeadEmployeeId,    out var h) ? h : null;

                if (isSave)
                {
                    // Reject duplicate code up-front to give a clean error.
                    bool dup = await _ctx.Departments!
                        .AnyAsync(d => d.DepartmentCode == request.DepartmentCode);
                    if (dup) return Utilities.Warn(ConstantSupplier.DEPARTMENT_DUPLICATE);

                    var dept = new Department
                    {
                        Id = Guid.NewGuid(),
                        DepartmentCode     = request.DepartmentCode,
                        Name               = request.Name,
                        Description        = request.Description,
                        ParentDepartmentId = parentId,
                        HeadEmployeeId     = headId,
                        CreatedBy          = request.CreateUpdateBy,
                        CreatedDate        = DateTime.UtcNow,
                        IsActive           = request.IsActive ?? true
                    };
                    await _ctx.Departments!.AddAsync(dept);
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.DEPARTMENT_CREATE_SUCCESS, dept.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id))
                        return Utilities.Warn("Invalid id for update");

                    var dept = await _ctx.Departments!.FirstOrDefaultAsync(d => d.Id == id);
                    if (dept == null)
                        return Utilities.Warn(ConstantSupplier.DEPARTMENT_NOT_FOUND,
                                              code: (int)HttpStatusCode.NotFound);

                    dept.DepartmentCode     = request.DepartmentCode;
                    dept.Name               = request.Name;
                    dept.Description        = request.Description;
                    dept.ParentDepartmentId = parentId;
                    dept.HeadEmployeeId     = headId;
                    dept.UpdatedBy          = request.CreateUpdateBy;
                    dept.UpdatedDate        = DateTime.UtcNow;
                    dept.IsActive           = request.IsActive ?? true;

                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.DEPARTMENT_UPDATE_SUCCESS, dept.Id);
                }
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueViolation(dbEx))
            {
                await tx.RollbackAsync();
                return Utilities.Warn(ConstantSupplier.DEPARTMENT_DUPLICATE);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateDepartmentAsync));
            }
        }

        public async Task<DataResponse> DeleteDepartmentAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g))
                    return Utilities.Warn("Invalid department id");

                var dept = await _ctx.Departments!.FirstOrDefaultAsync(d => d.Id == g);
                if (dept == null)
                    return Utilities.Warn(ConstantSupplier.DEPARTMENT_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                bool hasEmployees = await _ctx.Employees!
                    .AnyAsync(e => e.DepartmentId == g && e.IsActive == true);

                if (hasEmployees)
                {
                    // Soft-delete only — we never break referential integrity.
                    dept.IsActive = false;
                    await _ctx.SaveChangesAsync();
                    return Utilities.Warn(ConstantSupplier.DEPARTMENT_HAS_EMPLOYEES, dept.Id);
                }

                bool isHardDelete = await _ctx.HRSettings!.AsNoTracking()
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => x.IsHardDelete ?? false)
                    .FirstOrDefaultAsync();

                if (isHardDelete)
                {
                    _ctx.Departments!.Remove(dept);
                }
                else
                {
                    dept.IsActive = false;
                }
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.DEPARTMENT_DELETE_SUCCESS, dept.Id);
            }
            catch (DbUpdateException dbEx) when (Utilities.IsForeignKeyViolation(dbEx))
            {
                return Utilities.Warn(ConstantSupplier.DEPARTMENT_HAS_EMPLOYEES);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteDepartmentAsync)); }
        }

        // ----- helpers -----
        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
