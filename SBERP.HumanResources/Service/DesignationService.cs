using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Net;

namespace SBERP.HumanResources.Service
{
    public class DesignationService : IDesignationService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public DesignationService(HumanResourcesDBContext ctx, IHRLogService log)
        {
            _ctx = ctx; _log = log;
        }

        public async Task<DataResponse> GetAllDesignationsAsync()
        {
            try
            {
                var list = await _ctx.Designations!
                    .AsNoTracking()
                    .Where(d => d.IsActive == true)
                    .OrderBy(d => d.Name)
                    .Select(d => new DesignationResponse
                    {
                        Id = d.Id, Name = d.Name, Code = d.Code,
                        Grade = d.Grade, IsActive = d.IsActive
                    })
                    .ToListAsync();

                if (Utilities.IsNull(list))
                    return Utilities.Warn(ConstantSupplier.DESIGNATION_LIST_EMPTY,
                                          code: (int)HttpStatusCode.NotFound);

                return Utilities.Ok("Designations fetched successfully.", list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllDesignationsAsync)); }
        }

        public async Task<PagingResult<DesignationResponse>?> GetAllDesignationsPagingWithSearchAsync(
            PagingSearchFilter filter)
        {
            try
            {
                var conn = _ctx.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    "EXEC dbo." + ConstantSupplier.SP_GET_ALL_DESIGNATIONS_PAGING +
                    " @SearchTerm=@p0, @SortColumnName=@p1, " +
                    "@SortColumnDirection=@p2, @PageNumber=@p3, @PageSize=@p4";

                AddParam(cmd, "@p0", filter.SearchTerm);
                AddParam(cmd, "@p1", filter.SortColumnName);
                AddParam(cmd, "@p2", filter.SortColumnDirection);
                AddParam(cmd, "@p3", filter.PageNumber);
                AddParam(cmd, "@p4", filter.PageSize);

                var raw = (string?)await cmd.ExecuteScalarAsync();
                if (string.IsNullOrWhiteSpace(raw)) return null;

                var jo = JObject.Parse(raw);
                var items = jo["Items"]?.ToString();

                return new PagingResult<DesignationResponse>
                {
                    RowCount    = jo.Value<int>("RowCount"),
                    CurrentPage = jo.Value<int>("CurrentPage"),
                    PageSize    = jo.Value<int>("PageSize"),
                    PageCount   = jo.Value<int>("PageCount"),
                    Items       = string.IsNullOrWhiteSpace(items)
                                  ? new List<DesignationResponse>()
                                  : JsonConvert.DeserializeObject<List<DesignationResponse>>(items)
                };
            }
            catch (Exception ex)
            {
                Utilities.Exception(ex, _log, nameof(GetAllDesignationsPagingWithSearchAsync));
                return null;
            }
        }

        public async Task<DataResponse> CreateUpdateDesignationAsync(DesignationRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isSave = string.Equals(request.ActionName,
                    ConstantSupplier.SAVE_KEY, StringComparison.OrdinalIgnoreCase);

                if (isSave)
                {
                    bool dup = await _ctx.Designations!.AnyAsync(d => d.Name == request.Name);
                    if (dup) return Utilities.Warn(ConstantSupplier.DESIGNATION_DUPLICATE);

                    var d = new Designation
                    {
                        Id = Guid.NewGuid(),
                        Name = request.Name,
                        Code = request.Code,
                        Description = request.Description,
                        Grade = request.Grade,
                        CreatedBy = request.CreateUpdateBy,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = request.IsActive ?? true
                    };
                    await _ctx.Designations!.AddAsync(d);
                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.DESIGNATION_CREATE_SUCCESS, d.Id);
                }
                else
                {
                    if (!Guid.TryParse(request.Id, out var id))
                        return Utilities.Warn("Invalid designation id");

                    var d = await _ctx.Designations!.FirstOrDefaultAsync(x => x.Id == id);
                    if (d == null)
                        return Utilities.Warn(ConstantSupplier.DESIGNATION_NOT_FOUND,
                                              code: (int)HttpStatusCode.NotFound);

                    d.Name = request.Name;
                    d.Code = request.Code;
                    d.Description = request.Description;
                    d.Grade = request.Grade;
                    d.UpdatedBy = request.CreateUpdateBy;
                    d.UpdatedDate = DateTime.UtcNow;
                    d.IsActive = request.IsActive ?? true;

                    await _ctx.SaveChangesAsync();
                    await tx.CommitAsync();
                    return Utilities.Ok(ConstantSupplier.DESIGNATION_UPDATE_SUCCESS, d.Id);
                }
            }
            catch (DbUpdateException dbEx) when (Utilities.IsUniqueViolation(dbEx))
            {
                await tx.RollbackAsync();
                return Utilities.Warn(ConstantSupplier.DESIGNATION_DUPLICATE);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(CreateUpdateDesignationAsync));
            }
        }

        public async Task<DataResponse> DeleteDesignationAsync(string id, bool hardDelete = false)
        {
            try
            {
                if (!Guid.TryParse(id, out var g))
                    return Utilities.Warn("Invalid designation id");

                var d = await _ctx.Designations!.FirstOrDefaultAsync(x => x.Id == g);
                if (d == null)
                    return Utilities.Warn(ConstantSupplier.DESIGNATION_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                bool used = await _ctx.Employees!.AnyAsync(e => e.DesignationId == g);
                if (used)
                {
                    d.IsActive = false;
                    await _ctx.SaveChangesAsync();
                    return Utilities.Warn("Designation in use — inactivated instead of removed.", d.Id);
                }

                if (hardDelete) _ctx.Designations!.Remove(d);
                else d.IsActive = false;

                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.DESIGNATION_DELETE_SUCCESS, d.Id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteDesignationAsync)); }
        }

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
