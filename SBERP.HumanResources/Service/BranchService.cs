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
    public class BranchService : IBranchService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;

        public BranchService(HumanResourcesDBContext ctx, IHRLogService log)
        {
            _ctx = ctx;
            _log = log;
        }

        public async Task<DataResponse> GetAllBranchesAsync()
        {
            try
            {
                var list = await _ctx.Branches!
                    .AsNoTracking()
                    .Where(b => b.IsActive == true)
                    .OrderBy(b => b.Name)
                    .Select(b => new BranchResponse
                    {
                        Id = b.Id,
                        CompanyId = b.CompanyId,
                        BranchCode = b.BranchCode,
                        Name = b.Name,
                        IsHeadOffice = b.IsHeadOffice,
                        IsActive = b.IsActive
                    })
                    .ToListAsync();

                if (Utilities.IsNull(list))
                    return Utilities.Warn(ConstantSupplier.BRANCH_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);

                return Utilities.Ok(ConstantSupplier.BRANCH_FETCH_SUCCESS, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllBranchesAsync)); }
        }

        public async Task<DataResponse> GetAllBranchesByCompanyIdAsync(string companyId)
        {
            if (!Guid.TryParse(companyId, out var cid)) return Utilities.Warn("Invalid company id.");

            var list = await _ctx.Branches!
                .AsNoTracking()
                .Where(b => b.IsActive == true && b.CompanyId == cid)
                .OrderBy(b => b.Name)
                .Select(b => new BranchResponse { Id = b.Id, CompanyId = b.CompanyId, BranchCode = b.BranchCode, Name = b.Name, IsHeadOffice = b.IsHeadOffice })
                .ToListAsync();

            return Utilities.Ok("Branches fetched successfully.", list);
        }

        public async Task<PagingResult<BranchResponse>?> GetAllBranchesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            var sql = "EXEC dbo.SP_GetAllBranchesPagingWithSearch @SearchTerm={0}, @SortColumnName={1}, @SortColumnDirection={2}, @PageNumber={3}, @PageSize={4}";
            var conn = _ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql.Replace("{0}", "@p0").Replace("{1}", "@p1").Replace("{2}", "@p2").Replace("{3}", "@p3").Replace("{4}", "@p4");
            AddParam(cmd, "@p0", filter.SearchTerm);
            AddParam(cmd, "@p1", filter.SortColumnName);
            AddParam(cmd, "@p2", filter.SortColumnDirection);
            AddParam(cmd, "@p3", filter.PageNumber);
            AddParam(cmd, "@p4", filter.PageSize);

            var raw = (string?)await cmd.ExecuteScalarAsync();
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var jo = JObject.Parse(raw);
            var items = jo["Items"]?.ToString();

            return new PagingResult<BranchResponse>
            {
                RowCount = jo.Value<int>("RowCount"),
                CurrentPage = jo.Value<int>("CurrentPage"),
                PageSize = jo.Value<int>("PageSize"),
                PageCount = jo.Value<int>("PageCount"),
                Items = string.IsNullOrWhiteSpace(items) ? new List<BranchResponse>()
                        : JsonConvert.DeserializeObject<List<BranchResponse>>(items)
            };
        }

        public async Task<DataResponse> GetBranchByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid)) return Utilities.Warn("Invalid branch id.");

            var b = await _ctx.Branches!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == gid);
            if (b == null) return Utilities.Warn("Branch not found.", code: (int)HttpStatusCode.NotFound);

            var resp = new BranchResponse
            {
                Id = b.Id,
                CompanyId = b.CompanyId,
                BranchCode = b.BranchCode,
                Name = b.Name,
                Address = b.Address,
                City = b.City,
                Country = b.Country,
                Phone = b.Phone,
                Email = b.Email,
                IsHeadOffice = b.IsHeadOffice,
                IsActive = b.IsActive,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate
            };
            return Utilities.Ok("Branch fetched successfully.", resp);
        }

        public async Task<DataResponse> CreateUpdateBranchAsync(BranchRequest request)
        {
            if (!Guid.TryParse(request.CompanyId, out var companyId)) return Utilities.Warn("A valid CompanyId is required.");

            var companyExists = await _ctx.Companies!.AnyAsync(c => c.Id == companyId && c.IsActive == true);
            if (!companyExists) return Utilities.Warn("Selected company does not exist or is inactive.");

            var gid = Guid.Empty;
            var isUpdate = !string.IsNullOrWhiteSpace(request.Id) && Guid.TryParse(request.Id, out gid);
            var now = DateTime.UtcNow;

            if (isUpdate)
            {
                var entity = await _ctx.Branches!.FirstOrDefaultAsync(x => x.Id == gid);
                if (entity == null) return Utilities.Warn("Branch not found.", code: (int)HttpStatusCode.NotFound);

                entity.CompanyId = companyId;
                entity.BranchCode = request.BranchCode;
                entity.Name = request.Name;
                entity.Address = request.Address;
                entity.City = request.City;
                entity.Country = request.Country;
                entity.Phone = request.Phone;
                entity.Email = request.Email;
                entity.IsHeadOffice = request.IsHeadOffice;
                entity.UpdatedBy = request.CreateUpdateBy;
                entity.UpdatedDate = now;
                entity.IsActive = request.IsActive ?? entity.IsActive;

                await _ctx.SaveChangesAsync();
                return Utilities.Ok("Branch updated successfully.", entity.Id);
            }
            else
            {
                var entity = new Branch
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    BranchCode = request.BranchCode,
                    Name = request.Name,
                    Address = request.Address,
                    City = request.City,
                    Country = request.Country,
                    Phone = request.Phone,
                    Email = request.Email,
                    IsHeadOffice = request.IsHeadOffice,
                    CreatedBy = request.CreateUpdateBy,
                    CreatedDate = now,
                    UpdatedDate = now,
                    IsActive = true
                };
                _ctx.Branches!.Add(entity);
                await _ctx.SaveChangesAsync();
                return Utilities.Ok("Branch created successfully.", entity.Id);
            }
        }

        public async Task<DataResponse> DeleteBranchAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid)) return Utilities.Warn("Invalid branch id.");

            var entity = await _ctx.Branches!.FirstOrDefaultAsync(x => x.Id == gid);
            if (entity == null) return Utilities.Warn("Branch not found.", code: (int)HttpStatusCode.NotFound);

            var hasDependents = await _ctx.Departments!.AnyAsync(d => d.BranchId == gid && d.IsActive == true)
                || await _ctx.Employees!.AnyAsync(e => e.BranchId == gid && e.IsActive == true);
            if (hasDependents) return Utilities.Warn("Cannot remove a branch that still has active departments or employees.");

            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            return Utilities.Ok("Branch removed successfully.", entity.Id);
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