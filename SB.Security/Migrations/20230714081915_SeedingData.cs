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
                values: new object[] { new Guid("52bc5ac3-8a09-4605-864d-58ab29a6761b"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6470), "sbhowmikcse08@gmail.com", "Sreemonta Bhowmik", true, new DateTime(2023, 7, 14, 12, 19, 14, 987, DateTimeKind.Local).AddTicks(6265), 0, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6470), "sree", "Admin" });

            migrationBuilder.InsertData(
                table: "UserMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "CssClass", "DropdownIcon", "Icon", "IsActive", "IsHeader", "Name", "ParentId", "Remark", "RouteLink", "RouteLinkClass", "SerialNo", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1e5313ef-0224-4dc9-9b69-e5fe31397943"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6693), "nav-item", "fas fa-angle-left right", "nav-icon fas fa-cog", true, false, "Security", new Guid("5352e0cf-e7d7-4449-9ff7-232c09806937"), "Navigation Item", "", "nav-link active", 4, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6694) },
                    { new Guid("20e6e353-1d7b-4382-8528-2749dee0d067"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6678), "nav-header", null, "", true, true, "Business", null, "Header", "", "", 3, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6678) },
                    { new Guid("243de74f-beb1-4366-9ca8-41e414882570"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6698), "nav-header", null, "", true, true, "Settings", null, "Header", "", "", 6, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6699) },
                    { new Guid("8e6a3c9e-b316-4b87-a37b-a34cef6e0c1d"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6696), "nav-item", null, "far fa-circle nav-icon", true, false, "User", new Guid("a9b05c8b-84aa-4edf-871a-bff7110a174f"), "Navigation Item", "/business/security", "nav-link", 5, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6697) },
                    { new Guid("b86a96b9-9a26-4848-8381-a270a1cd2eda"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6675), "nav-item", null, "nav-icon fas fa-tachometer-alt", true, false, "Dashboard", null, "Navigation Item", "/business/home", "nav-link active", 2, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6676) },
                    { new Guid("d58d22e7-bc6b-4823-a81d-41c1869239e9"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6701), "nav-item", null, "nav-icon fas fa-cog", true, false, "App Settings", null, "Navigation Item", "/business/appsettings", "nav-link active", 7, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6701) },
                    { new Guid("dc639f43-be0d-4b00-b664-b8a159ed3d19"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6672), "nav-header", null, "", true, true, "Home", null, "Header", "", "", 1, null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6672) }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "RoleName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2a6152df-89c5-4b5c-8b88-be3686ab5b8d"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6654), "Admin", true, "Admin", null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6655) },
                    { new Guid("85008dde-a159-4b81-ab52-00859404109d"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6657), "User", true, "User", null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6657) }
                });

            migrationBuilder.InsertData(
                table: "UserRoleMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "MenuId", "RoleId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("63ffa85c-bff6-4807-8525-b96404437008"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6722), true, new Guid("ad987118-9c71-4a84-9df1-b975314e0cdb"), new Guid("4ba27142-a0c9-4331-bf62-4d21eb9ae4fd"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6723) },
                    { new Guid("9ed95872-e35a-4c60-831d-0308705beedd"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6727), true, new Guid("cc6ce2af-e055-4d3e-9df6-b1d045eb89cd"), new Guid("cc270c13-54d3-4653-9327-41cfe2b81a85"), null, new DateTime(2023, 7, 14, 8, 19, 14, 987, DateTimeKind.Utc).AddTicks(6728) }
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
