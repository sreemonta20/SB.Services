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
    /// <summary>Shared ADO helper for the payroll services — JSON scalar reads
    /// plus the RowsAffected/Message reader for the run/loan SPs.</summary>
    internal static class PayrollAdo
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

        public static async Task<(int rows, string msg, string? extra)> StatusSpAsync(HumanResourcesDBContext ctx, string sp, string? extraCol, params (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            using var reader = await cmd.ExecuteReaderAsync();
            int rows = 0; string msg = ConstantSupplier.FAILED_MSG; string? extra = null;
            if (await reader.ReadAsync())
            {
                rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                msg = reader.GetString(reader.GetOrdinal("Message"));
                if (extraCol != null && HasColumn(reader, extraCol) && !await reader.IsDBNullAsync(reader.GetOrdinal(extraCol)))
                    extra = reader.GetValue(reader.GetOrdinal(extraCol))?.ToString();
            }
            await reader.CloseAsync();
            return (rows, msg, extra);
        }

        private static bool HasColumn(DbDataReader r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++) if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        public static void AddParam(DbCommand cmd, string name, object? value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p); }
    }

    public class PayComponentService : IPayComponentService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public PayComponentService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<PayComponentResponse>?> GetAllPayComponentsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<PayComponentResponse>(_ctx, ConstantSupplier.SP_GET_ALL_PAY_COMPONENTS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllPayComponentsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetPayComponentsLookupAsync()
        {
            try
            {
                var items = await _ctx.PayComponents!.Where(c => c.IsActive == true)
                    .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
                    .Select(c => new PayComponentResponse
                    {
                        Id = c.Id, ComponentCode = c.ComponentCode, Name = c.Name,
                        ComponentTypeId = c.ComponentTypeId, CalcMethod = c.CalcMethod,
                        DefaultValue = c.DefaultValue, IsTaxable = c.IsTaxable, IsProrated = c.IsProrated, IsActive = c.IsActive
                    }).ToListAsync();
                return Utilities.Ok(ConstantSupplier.PAY_COMPONENT_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetPayComponentsLookupAsync)); }
        }

        public async Task<DataResponse> CreateUpdatePayComponentAsync(PayComponentRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                bool dup = await _ctx.PayComponents!.AnyAsync(c => c.ComponentCode == request.ComponentCode && (!isUpdate || c.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Component code already exists.");

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid component id");
                    var c = await _ctx.PayComponents!.FirstOrDefaultAsync(x => x.Id == id);
                    if (c == null) return Utilities.Warn("Component not found", code: (int)HttpStatusCode.NotFound);
                    c.ComponentCode = request.ComponentCode; c.Name = request.Name; c.ComponentTypeId = request.ComponentTypeId;
                    c.CalcMethod = request.CalcMethod; c.DefaultValue = request.DefaultValue; c.IsTaxable = request.IsTaxable;
                    c.IsProrated = request.IsProrated; c.SortOrder = request.SortOrder; c.Description = request.Description;
                    c.UpdatedBy = request.CreateUpdateBy; c.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    await _ctx.PayComponents!.AddAsync(new PayComponent
                    {
                        Id = Guid.NewGuid(), ComponentCode = request.ComponentCode, Name = request.Name,
                        ComponentTypeId = request.ComponentTypeId, CalcMethod = request.CalcMethod, DefaultValue = request.DefaultValue,
                        IsTaxable = request.IsTaxable, IsProrated = request.IsProrated, SortOrder = request.SortOrder,
                        Description = request.Description, CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdatePayComponentAsync)); }
        }

        public async Task<DataResponse> DeletePayComponentAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid component id");
                var c = await _ctx.PayComponents!.FirstOrDefaultAsync(x => x.Id == g);
                if (c == null) return Utilities.Warn("Component not found", code: (int)HttpStatusCode.NotFound);
                bool inUse = await _ctx.SalaryStructureLines!.AnyAsync(l => l.PayComponentId == g);
                if (inUse) return Utilities.Warn(ConstantSupplier.PAY_COMPONENT_IN_USE);
                c.IsActive = false; c.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeletePayComponentAsync)); }
        }
    }

    public class SalaryStructureService : ISalaryStructureService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public SalaryStructureService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<SalaryStructureResponse>?> GetAllSalaryStructuresPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<SalaryStructureResponse>(_ctx, ConstantSupplier.SP_GET_ALL_SALARY_STRUCTURES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllSalaryStructuresPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> GetSalaryStructureDetailAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid structure id");
                var json = await PayrollAdo.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_SALARY_STRUCTURE_DETAIL, ("@Id", g));
                if (string.IsNullOrWhiteSpace(json)) return Utilities.Warn("Structure not found", code: (int)HttpStatusCode.NotFound);
                var obj = JsonConvert.DeserializeObject<object>(json!);
                return Utilities.Ok(ConstantSupplier.SALARY_STRUCTURE_FETCH, obj);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetSalaryStructureDetailAsync)); }
        }

        public async Task<DataResponse> CreateUpdateSalaryStructureAsync(SalaryStructureRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                Guid structureId;
                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out structureId)) return Utilities.Warn("Invalid structure id");
                    var s = await _ctx.SalaryStructures!.FirstOrDefaultAsync(x => x.Id == structureId);
                    if (s == null) return Utilities.Warn("Structure not found", code: (int)HttpStatusCode.NotFound);

                    // lock if a payslip already references this employee for an overlapping processed run
                    bool locked = await _ctx.Payslips!.AnyAsync(p => p.EmployeeId == empId);
                    // (soft guard — a stricter period check can be added; keeping edit open pre-run)

                    s.BasicSalary = request.BasicSalary; s.Currency = request.Currency;
                    s.EffectiveFrom = request.EffectiveFrom; s.EffectiveTo = request.EffectiveTo; s.Remarks = request.Remarks;
                    s.UpdatedBy = request.CreateUpdateBy; s.UpdatedDate = DateTime.UtcNow;

                    var old = _ctx.SalaryStructureLines!.Where(l => l.SalaryStructureId == structureId);
                    _ctx.SalaryStructureLines!.RemoveRange(old);
                }
                else
                {
                    // close any current open structure for this employee
                    var open = await _ctx.SalaryStructures!
                        .Where(s => s.EmployeeId == empId && s.IsActive == true && s.EffectiveTo == null)
                        .ToListAsync();
                    foreach (var o in open) { o.EffectiveTo = request.EffectiveFrom.AddDays(-1); o.UpdatedBy = request.CreateUpdateBy; o.UpdatedDate = DateTime.UtcNow; }

                    var max = await _ctx.SalaryStructures!
                        .Where(s => s.StructureCode!.StartsWith("SAL-"))
                        .Select(s => s.StructureCode!.Substring(4))
                        .ToListAsync();
                    int next = max.Select(x => int.TryParse(x, out var n) ? n : 0).DefaultIfEmpty(0).Max() + 1;

                    structureId = Guid.NewGuid();
                    await _ctx.SalaryStructures!.AddAsync(new SalaryStructure
                    {
                        Id = structureId, StructureCode = $"SAL-{next:D4}", EmployeeId = empId,
                        BasicSalary = request.BasicSalary, Currency = request.Currency,
                        EffectiveFrom = request.EffectiveFrom, EffectiveTo = request.EffectiveTo, Remarks = request.Remarks,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }

                if (request.Lines != null)
                {
                    foreach (var line in request.Lines)
                    {
                        if (!Guid.TryParse(line.PayComponentId, out var pc)) continue;
                        await _ctx.SalaryStructureLines!.AddAsync(new SalaryStructureLine
                        {
                            Id = Guid.NewGuid(), SalaryStructureId = structureId, PayComponentId = pc,
                            CalcMethod = line.CalcMethod, Value = line.Value,
                            CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                        });
                    }
                }

                await _ctx.SaveChangesAsync(); await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateSalaryStructureAsync)); }
        }

        public async Task<DataResponse> DeleteSalaryStructureAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid structure id");
                var s = await _ctx.SalaryStructures!.FirstOrDefaultAsync(x => x.Id == g);
                if (s == null) return Utilities.Warn("Structure not found", code: (int)HttpStatusCode.NotFound);
                s.IsActive = false; s.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteSalaryStructureAsync)); }
        }
    }
}
