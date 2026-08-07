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
    /// Same shape as DepartmentService: EF Core for CRUD (so triggers fire),
    /// the JSON-paging SP for list views. Uses the SAME HumanResourcesDBContext
    /// as DepartmentService/EmployeeService — Company/Branch are Module 0 of
    /// this microservice, not a separate database.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;

        public CompanyService(HumanResourcesDBContext ctx, IHRLogService log, IOptions<AppSettings> opts)
        {
            _ctx = ctx;
            _log = log;
            _settings = opts.Value;
        }

        public async Task<DataResponse> GetAllCompaniesAsync()
        {
            try
            {
                var list = await _ctx.Companies!
                    .AsNoTracking()
                    .Where(c => c.IsActive == true)
                    .OrderBy(c => c.Name)
                    .Select(c => new CompanyResponse
                    {
                        Id = c.Id,
                        CompanyCode = c.CompanyCode,
                        Name = c.Name,
                        LegalName = c.LegalName,
                        CurrencyCode = c.CurrencyCode,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();

                if (Utilities.IsNull(list))
                    return Utilities.Warn(ConstantSupplier.COMPANY_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);

                return Utilities.Ok(ConstantSupplier.COMPANY_FETCH_SUCCESS, list);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCompaniesAsync)); }
        }

        public async Task<PagingResult<CompanyResponse>?> GetAllCompaniesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            var sql = "EXEC dbo.SP_GetAllCompaniesPagingWithSearch @SearchTerm={0}, @SortColumnName={1}, @SortColumnDirection={2}, @PageNumber={3}, @PageSize={4}";
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

            return new PagingResult<CompanyResponse>
            {
                RowCount = jo.Value<int>("RowCount"),
                CurrentPage = jo.Value<int>("CurrentPage"),
                PageSize = jo.Value<int>("PageSize"),
                PageCount = jo.Value<int>("PageCount"),
                Items = string.IsNullOrWhiteSpace(items) ? new List<CompanyResponse>()
                        : JsonConvert.DeserializeObject<List<CompanyResponse>>(items)
            };
        }

        public async Task<DataResponse> GetCompanyByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid)) return Utilities.Warn("Invalid company id.");

            var c = await _ctx.Companies!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == gid);
            if (c == null) return Utilities.Warn("Company not found.", code: (int)HttpStatusCode.NotFound);

            var branchCount = await _ctx.Branches!.CountAsync(b => b.CompanyId == gid && b.IsActive == true);

            var resp = new CompanyResponse
            {
                Id = c.Id,
                CompanyCode = c.CompanyCode,
                Name = c.Name,
                LegalName = c.LegalName,
                RegistrationNumber = c.RegistrationNumber,
                TaxNumber = c.TaxNumber,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                Phone = c.Phone,
                Email = c.Email,
                Website = c.Website,
                LogoUrl = c.LogoUrl,
                CurrencyCode = c.CurrencyCode,
                FinancialYearStartMonth = c.FinancialYearStartMonth,
                BranchCount = branchCount,
                IsActive = c.IsActive,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            };
            return Utilities.Ok("Company fetched successfully.", resp);
        }

        public async Task<DataResponse> CreateUpdateCompanyAsync(CompanyRequest request)
        {
            var gid = Guid.Empty;
            var isUpdate = !string.IsNullOrWhiteSpace(request.Id) && Guid.TryParse(request.Id, out gid);
            var now = DateTime.UtcNow;

            if (isUpdate)
            {
                var entity = await _ctx.Companies!.FirstOrDefaultAsync(x => x.Id == gid);
                if (entity == null) return Utilities.Warn("Company not found.", code: (int)HttpStatusCode.NotFound);

                entity.CompanyCode = request.CompanyCode;
                entity.Name = request.Name;
                entity.LegalName = request.LegalName;
                entity.RegistrationNumber = request.RegistrationNumber;
                entity.TaxNumber = request.TaxNumber;
                entity.Address = request.Address;
                entity.City = request.City;
                entity.Country = request.Country;
                entity.Phone = request.Phone;
                entity.Email = request.Email;
                entity.Website = request.Website;
                entity.LogoUrl = request.LogoUrl;
                entity.CurrencyCode = request.CurrencyCode;
                entity.FinancialYearStartMonth = request.FinancialYearStartMonth;
                entity.UpdatedBy = request.CreateUpdateBy;
                entity.UpdatedDate = now;
                entity.IsActive = request.IsActive ?? entity.IsActive;

                await _ctx.SaveChangesAsync();
                return Utilities.Ok("Company updated successfully.", entity.Id);
            }
            else
            {
                var entity = new Company
                {
                    Id = Guid.NewGuid(),
                    CompanyCode = request.CompanyCode,
                    Name = request.Name,
                    LegalName = request.LegalName,
                    RegistrationNumber = request.RegistrationNumber,
                    TaxNumber = request.TaxNumber,
                    Address = request.Address,
                    City = request.City,
                    Country = request.Country,
                    Phone = request.Phone,
                    Email = request.Email,
                    Website = request.Website,
                    LogoUrl = request.LogoUrl,
                    CurrencyCode = request.CurrencyCode,
                    FinancialYearStartMonth = request.FinancialYearStartMonth,
                    CreatedBy = request.CreateUpdateBy,
                    CreatedDate = now,
                    UpdatedDate = now,
                    IsActive = true
                };
                _ctx.Companies!.Add(entity);
                await _ctx.SaveChangesAsync();
                return Utilities.Ok("Company created successfully.", entity.Id);
            }
        }

        public async Task<DataResponse> DeleteCompanyAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid)) return Utilities.Warn("Invalid company id.");

            var entity = await _ctx.Companies!.FirstOrDefaultAsync(x => x.Id == gid);
            if (entity == null) return Utilities.Warn("Company not found.", code: (int)HttpStatusCode.NotFound);

            var hasDependents = await _ctx.Branches!.AnyAsync(b => b.CompanyId == gid && b.IsActive == true)
                || await _ctx.Departments!.AnyAsync(d => d.CompanyId == gid && d.IsActive == true)
                || await _ctx.Employees!.AnyAsync(e => e.CompanyId == gid && e.IsActive == true);
            if (hasDependents) return Utilities.Warn("Cannot remove a company that still has active branches, departments, or employees.");

            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            return Utilities.Ok("Company removed successfully.", entity.Id);
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
