using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SB.Security.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaltKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLoginAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginFailedAttemptsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMenu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHeader = table.Column<bool>(type: "bit", nullable: true),
                    CssClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteLinkClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DropdownIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMenu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMenu", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FullName", "IsActive", "LastLoginAttemptAt", "LoginFailedAttemptsCount", "Password", "SaltKey", "UpdatedBy", "UpdatedDate", "UserName", "UserRole" },
                values: new object[] { new Guid("c68023aa-8d1c-47ab-90e2-3ad749f4cb1c"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2141), "sbhowmikcse08@gmail.com", "Sreemonta Bhowmik", true, new DateTime(2023, 7, 13, 23, 4, 43, 776, DateTimeKind.Local).AddTicks(2099), 0, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2142), "sree", "Admin" });

            migrationBuilder.InsertData(
                table: "UserMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "CssClass", "DropdownIcon", "Icon", "IsActive", "IsHeader", "Name", "ParentId", "Remark", "RouteLink", "RouteLinkClass", "SerialNo", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0ba16451-3045-4770-8e05-f894666acaf4"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2258), "nav-item", null, "nav-icon fas fa-tachometer-alt", true, false, "Dashboard", null, "Navigation Item", "/business/home", "nav-link active", 2, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2258) },
                    { new Guid("6f36b193-383f-491e-a443-887a4774b999"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2278), "nav-header", null, "", true, true, "Settings", null, "Header", "", "", 6, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2278) },
                    { new Guid("9228b41c-8db5-4624-9427-be645234759f"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2254), "nav-header", null, "", true, true, "Home", null, "Header", "", "", 1, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2255) },
                    { new Guid("9ee64115-c836-48c6-824a-0420fa6473f8"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2260), "nav-header", null, "", true, true, "Business", null, "Header", "", "", 3, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2260) },
                    { new Guid("a2228b17-c6ab-4da0-a071-65a099f03d56"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2276), "nav-item", null, "far fa-circle nav-icon", true, false, "User", new Guid("7630c8a8-0319-4e9f-8020-e2feaf0cde5c"), "Navigation Item", "/business/security", "nav-link", 5, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2276) },
                    { new Guid("d4e001f6-9dc8-46f2-bfd9-f63ba5f1c1ab"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2273), "nav-item", "fas fa-angle-left right", "nav-icon fas fa-cog", true, false, "Security", new Guid("054da5d9-8034-40d4-995f-9e11a344d764"), "Navigation Item", "", "nav-link active", 4, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2273) },
                    { new Guid("fa9b5c11-39e9-43f7-9431-86acb209e552"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2280), "nav-item", null, "nav-icon fas fa-cog", true, false, "App Settings", null, "Navigation Item", "/business/appsettings", "nav-link active", 7, null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2280) }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "RoleName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("ae992bbb-ecfa-428e-9e7c-760b29ba1311"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2235), "User", true, "User", null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2235) },
                    { new Guid("ee4338ad-5e80-40d6-88eb-085cddea22c0"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2232), "Admin", true, "Admin", null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2233) }
                });

            migrationBuilder.InsertData(
                table: "UserRoleMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "MenuId", "RoleId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4f5dc663-fa73-45e5-850d-4a1479f7911b"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2344), true, new Guid("e99c408a-0c57-4d60-b000-c70d0745156b"), new Guid("10fb98f4-c1d5-4644-9183-b380021fc283"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2344) },
                    { new Guid("9d437d3e-7bdc-4541-a35f-f9853796a8bb"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2338), true, new Guid("a4241050-178f-45f1-a92d-e3ad54672c09"), new Guid("7d45960e-a4a5-411c-8084-66c477b16379"), null, new DateTime(2023, 7, 13, 19, 4, 43, 776, DateTimeKind.Utc).AddTicks(2338) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserMenu");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserRoleMenu");
        }
    }
}
