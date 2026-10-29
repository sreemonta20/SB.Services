using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.Security.Persistence;

#nullable disable

namespace SBERP.Security.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileJobTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AppUserProfilesLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AppUserProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUserProfiles",
                keyColumn: "Id",
                keyValue: new Guid("c047d662-9f0e-4358-b323-15ec3081312c"),
                column: "JobTitle",
                value: "Chief Executive Officer");

            migrationBuilder.UpdateData(
                table: "AppUserProfiles",
                keyColumn: "Id",
                keyValue: new Guid("efedc118-3459-4c2e-9158-ad69196a59e0"),
                column: "JobTitle",
                value: "Software Engineer");

            // SQL objects (functions, stored procedures, triggers) from Persistence/Sql/SecurityDB.sql,
            // applied after every table exists. Keep this call ONLY in the newest migration: when a
            // migration is added, move the call there (see SqlScriptHelper).
            SqlScriptHelper.ApplyAll(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AppUserProfilesLog");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AppUserProfiles");
        }
    }
}
