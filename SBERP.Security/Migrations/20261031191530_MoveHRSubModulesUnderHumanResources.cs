using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBERP.Security.Migrations
{
    /// <inheritdoc />
    public partial class MoveHRSubModulesUnderHumanResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("05fbca7e-8d98-4c4b-9529-4dfb29db65fd"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/corehr/settings", 18 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("082295e5-cd29-4693-8cf9-8082d4f8e791"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000100"), "/business/humanresources/corehr/company" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("171253a9-0beb-4722-8fc4-a4c18895b980"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/corehr/employee", 17 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000001"),
                column: "SerialNo",
                value: 19);

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000002"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/orgsetup/position", 20 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000003"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/orgsetup/cost-center", 21 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000004"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/orgsetup/job-grade", 22 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000005"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/orgsetup/holiday-calendar", 23 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000007"),
                column: "RouteLink",
                value: "/business/humanresources/recruitment/requisition");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000008"),
                column: "RouteLink",
                value: "/business/humanresources/recruitment/job-posting");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000009"),
                column: "RouteLink",
                value: "/business/humanresources/recruitment/pipeline");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000011"),
                column: "RouteLink",
                value: "/business/humanresources/onboarding/list");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000013"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/shift");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000014"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/overtime");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000015"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/summary");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000017"),
                column: "RouteLink",
                value: "/business/humanresources/leave/applications");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000018"),
                column: "RouteLink",
                value: "/business/humanresources/leave/types");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000019"),
                column: "RouteLink",
                value: "/business/humanresources/leave/policies");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000021"),
                column: "RouteLink",
                value: "/business/humanresources/payroll/runs");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000022"),
                column: "RouteLink",
                value: "/business/humanresources/payroll/salary-structures");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000023"),
                column: "RouteLink",
                value: "/business/humanresources/payroll/components");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000024"),
                column: "RouteLink",
                value: "/business/humanresources/payroll/loans");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000026"),
                column: "RouteLink",
                value: "/business/humanresources/performance/cycles");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000027"),
                column: "RouteLink",
                value: "/business/humanresources/performance/goals");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000028"),
                column: "RouteLink",
                value: "/business/humanresources/performance/reviews");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000029"),
                column: "RouteLink",
                value: "/business/humanresources/performance/competencies");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000031"),
                column: "RouteLink",
                value: "/business/humanresources/separation");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000100"),
                column: "SerialNo",
                value: 12);

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000200"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/biometric-registrations");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000201"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/punches");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000202"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/devices");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000203"),
                column: "RouteLink",
                value: "/business/humanresources/attendance/zones");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("88664718-2161-44ce-b940-39c952e7f56c"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000100"), "/business/humanresources/corehr/branch" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("89614ef6-4931-458f-8611-de1ea89a2c2d"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000100"), "/business/humanresources/corehr/designation" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("a0116982-f0b2-4c4e-ab05-0a59b0b2a0b8"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000100"), "/business/humanresources/corehr/department" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("05fbca7e-8d98-4c4b-9529-4dfb29db65fd"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/settings", 23 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("082295e5-cd29-4693-8cf9-8082d4f8e791"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000001"), "/business/humanresources/company" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("171253a9-0beb-4722-8fc4-a4c18895b980"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/humanresources/employee", 22 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000001"),
                column: "SerialNo",
                value: 12);

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000002"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/orgsetup/position", 17 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000003"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/orgsetup/cost-center", 18 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000004"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/orgsetup/job-grade", 19 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000005"),
                columns: new[] { "RouteLink", "SerialNo" },
                values: new object[] { "/business/orgsetup/holiday-calendar", 20 });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000007"),
                column: "RouteLink",
                value: "/business/recruitment/requisition");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000008"),
                column: "RouteLink",
                value: "/business/recruitment/job-posting");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000009"),
                column: "RouteLink",
                value: "/business/recruitment/pipeline");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000011"),
                column: "RouteLink",
                value: "/business/onboarding/list");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000013"),
                column: "RouteLink",
                value: "/business/attendance/shift");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000014"),
                column: "RouteLink",
                value: "/business/attendance/overtime");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000015"),
                column: "RouteLink",
                value: "/business/attendance/summary");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000017"),
                column: "RouteLink",
                value: "/business/leave/applications");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000018"),
                column: "RouteLink",
                value: "/business/leave/types");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000019"),
                column: "RouteLink",
                value: "/business/leave/policies");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000021"),
                column: "RouteLink",
                value: "/business/payroll/runs");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000022"),
                column: "RouteLink",
                value: "/business/payroll/salary-structures");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000023"),
                column: "RouteLink",
                value: "/business/payroll/components");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000024"),
                column: "RouteLink",
                value: "/business/payroll/loans");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000026"),
                column: "RouteLink",
                value: "/business/performance/cycles");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000027"),
                column: "RouteLink",
                value: "/business/performance/goals");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000028"),
                column: "RouteLink",
                value: "/business/performance/reviews");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000029"),
                column: "RouteLink",
                value: "/business/performance/competencies");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000031"),
                column: "RouteLink",
                value: "/business/separation");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000100"),
                column: "SerialNo",
                value: 21);

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000200"),
                column: "RouteLink",
                value: "/business/attendance/biometric-registrations");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000201"),
                column: "RouteLink",
                value: "/business/attendance/punches");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000202"),
                column: "RouteLink",
                value: "/business/attendance/devices");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("5e1d0000-0000-4000-8000-000000000203"),
                column: "RouteLink",
                value: "/business/attendance/zones");

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("88664718-2161-44ce-b940-39c952e7f56c"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000001"), "/business/humanresources/branch" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("89614ef6-4931-458f-8611-de1ea89a2c2d"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000001"), "/business/humanresources/designation" });

            migrationBuilder.UpdateData(
                table: "AppUserMenus",
                keyColumn: "Id",
                keyValue: new Guid("a0116982-f0b2-4c4e-ab05-0a59b0b2a0b8"),
                columns: new[] { "ParentId", "RouteLink" },
                values: new object[] { new Guid("5e1d0000-0000-4000-8000-000000000001"), "/business/humanresources/department" });
        }
    }
}
