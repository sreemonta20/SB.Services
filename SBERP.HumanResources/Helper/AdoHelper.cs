using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SBERP.HumanResources.Persistence;
using System.Data.Common;

namespace SBERP.HumanResources.Helper
{
    /// <summary>
    /// Thin ADO helpers over the EF connection, shared by the HR sub-modules:
    /// JSON-scalar SP reads, the RowsAffected/Message reader used by the
    /// transactional workflow SPs, and the PREFIX-0001 code generator.
    /// </summary>
    public static class AdoHelper
    {
        /// <summary>EXEC an SP that returns one JSON scalar (FOR JSON …, assigned to a variable).</summary>
        public static async Task<string?> ScalarSpAsync(HumanResourcesDBContext ctx, string sp,
            params (string name, object? value)[] ps)
        {
            using var cmd = await CreateCommandAsync(ctx, sp, ps);
            var result = await cmd.ExecuteScalarAsync();
            return result == null || result is DBNull ? null : result.ToString();
        }

        /// <summary>EXEC a workflow SP whose first row carries RowsAffected + Message.</summary>
        public static async Task<(int rows, string msg)> StatusSpAsync(HumanResourcesDBContext ctx, string sp,
            params (string name, object? value)[] ps)
        {
            using var cmd = await CreateCommandAsync(ctx, sp, ps);
            using var reader = await cmd.ExecuteReaderAsync();
            int rows = 0; string msg = ConstantSupplier.FAILED_MSG;
            if (await reader.ReadAsync())
            {
                rows = Convert.ToInt32(reader["RowsAffected"]);
                msg = reader["Message"]?.ToString() ?? msg;
            }
            return (rows, msg);
        }

        /// <summary>Same as <see cref="StatusSpAsync"/>, plus one extra column when the SP returns it.</summary>
        public static async Task<(int rows, string msg, string? extra)> StatusSpWithExtraAsync(HumanResourcesDBContext ctx, string sp, string? extraColumn,
            params (string name, object? value)[] ps)
        {
            using var cmd = await CreateCommandAsync(ctx, sp, ps);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return (0, ConstantSupplier.FAILED_MSG, null);

            string? extra = null;
            if (extraColumn != null && Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i).Equals(extraColumn, StringComparison.OrdinalIgnoreCase)))
                extra = await reader.IsDBNullAsync(reader.GetOrdinal(extraColumn)) ? null : reader[extraColumn].ToString();
            return (Convert.ToInt32(reader["RowsAffected"]), reader["Message"]?.ToString() ?? ConstantSupplier.FAILED_MSG, extra);
        }

        /// <summary>Next code for PREFIX-0001 style business keys from the codes already used.</summary>
        public static string NextCode(string prefix, IEnumerable<string?> existing)
        {
            int max = existing
                .Where(c => c != null && c.StartsWith(prefix + "-"))
                .Select(c => int.TryParse(c!.Substring(prefix.Length + 1), out var n) ? n : 0)
                .DefaultIfEmpty(0).Max();
            return $"{prefix}-{(max + 1):D4}";
        }

        private static async Task<DbCommand> CreateCommandAsync(HumanResourcesDBContext ctx, string sp,
            (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            // enlist in the ambient EF transaction when the caller opened one
            var tx = ctx.Database.CurrentTransaction?.GetDbTransaction();
            if (tx != null) cmd.Transaction = tx;
            cmd.CommandText = ps.Length == 0
                ? $"EXEC dbo.{sp}"
                : $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = $"@p{i}";
                p.Value = ps[i].value ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
            return cmd;
        }
    }
}
