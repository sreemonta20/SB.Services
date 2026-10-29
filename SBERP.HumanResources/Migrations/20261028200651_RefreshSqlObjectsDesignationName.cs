using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.HumanResources.Persistence;

#nullable disable

namespace SBERP.HumanResources.Migrations
{
    /// <inheritdoc />
    public partial class RefreshSqlObjectsDesignationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No table change: re-applies the SQL objects so SP_OnboardEmployeeFromOffer sends DesignationName
            // in EmployeeOnboardedEvent (Security stores it as the new login's role name / JobTitle).
            // SQL objects (functions, stored procedures, triggers) from Persistence/Sql/HumanResourcesDB.sql,
            // applied after every table exists. Keep this call ONLY in the newest migration: when a
            // migration is added, move the call there (see SqlScriptHelper).
            SqlScriptHelper.ApplyAll(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
