using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.Security.Persistence;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBERP.Security.Migrations
{
    /// <inheritdoc />
    public partial class AddBiometricAttendanceMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUserMenus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "CssClass", "DropdownIcon", "Icon", "IsActive", "IsComponent", "IsHeader", "IsModule", "IsRouteLink", "IsSubModule", "Name", "ParentId", "Remark", "RouteLink", "RouteLinkClass", "SerialNo", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5e1d0000-0000-4000-8000-000000000200"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-fingerprint nav-icon", true, true, false, false, true, false, "Biometric Registration", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/biometric-registrations", "nav-link", 50, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000201"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-list-ol nav-icon", true, true, false, false, true, false, "Attendance Punch Log", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/punches", "nav-link", 51, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000202"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-tablet-alt nav-icon", true, true, false, false, true, false, "Attendance Device", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/devices", "nav-link", 52, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000203"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-wifi nav-icon", true, true, false, false, true, false, "Office Zone", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/zones", "nav-link", 53, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUserRoleMenus",
                columns: new[] { "Id", "AppUserMenuId", "AppUserRoleId", "CreatedBy", "CreatedDate", "IsActive", "IsCreate", "IsDelete", "IsUpdate", "IsView", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("5e1d1000-0000-4000-8000-000000000200"), new Guid("5e1d0000-0000-4000-8000-000000000200"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000201"), new Guid("5e1d0000-0000-4000-8000-000000000201"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000202"), new Guid("5e1d0000-0000-4000-8000-000000000202"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000203"), new Guid("5e1d0000-0000-4000-8000-000000000203"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            // SQL objects are applied by the newest migration (SqlScriptHelper.ApplyAll), see AddUserProfilePhoto.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUserRoleMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d1000-0000-4000-8000-000000000200"));

            migrationBuilder.DeleteData(
                table: "AppUserRoleMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d1000-0000-4000-8000-000000000201"));

            migrationBuilder.DeleteData(
                table: "AppUserRoleMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d1000-0000-4000-8000-000000000202"));

            migrationBuilder.DeleteData(
                table: "AppUserRoleMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d1000-0000-4000-8000-000000000203"));

            migrationBuilder.DeleteData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000200"));

            migrationBuilder.DeleteData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000201"));

            migrationBuilder.DeleteData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000202"));

            migrationBuilder.DeleteData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000203"));
        }
    }
}
