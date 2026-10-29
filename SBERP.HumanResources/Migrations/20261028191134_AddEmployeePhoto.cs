using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.HumanResources.Persistence;

#nullable disable

namespace SBERP.HumanResources.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "EmployeesLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoLocation",
                table: "EmployeesLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoLocation",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // SqlScriptHelper.ApplyAll moved to 20261028200651_RefreshSqlObjectsDesignationName (newest migration).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "EmployeesLog");

            migrationBuilder.DropColumn(
                name: "PhotoLocation",
                table: "EmployeesLog");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhotoLocation",
                table: "Employees");
        }
    }
}
