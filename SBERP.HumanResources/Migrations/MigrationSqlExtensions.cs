using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.HumanResources.Persistence;

#nullable disable

namespace SBERP.HumanResources.Migrations
{
    /// <summary>
    /// After running `Add-Migration InitialCreate` EF will write the table-create
    /// code. Then paste the three migrationBuilder.Sql(...) lines below into
    /// that file's Up() so the SQL objects are created alongside the tables.
    ///
    /// Functions.sql, StoredProcedures.sql, Triggers.sql MUST be marked as
    /// "Embedded Resource" in the csproj for LoadEmbeddedSql to find them.
    /// </summary>
    public static class MigrationSqlExtensions
    {
        public static void ApplyHRSqlObjects(this MigrationBuilder mb)
        {
            mb.Sql(SqlScriptHelper.LoadEmbeddedSql("Functions.sql"));
            mb.Sql(SqlScriptHelper.LoadEmbeddedSql("StoredProcedures.sql"));
            mb.Sql(SqlScriptHelper.LoadEmbeddedSql("Triggers.sql"));
        }
    }
}
