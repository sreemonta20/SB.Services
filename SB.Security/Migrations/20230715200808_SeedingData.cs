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
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaltKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_UserInfo_UserRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_UserRoleMenu_UserMenu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "UserMenu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleMenu_UserRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "CssClass", "DropdownIcon", "Icon", "IsActive", "IsHeader", "Name", "ParentId", "Remark", "RouteLink", "RouteLinkClass", "SerialNo", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9543), "nav-header", null, "", true, true, "Settings", null, "Header", "", "", 6, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9544) },
                    { new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9547), "nav-item", null, "nav-icon fas fa-cog", true, false, "App Settings", null, "Navigation Item", "/business/appsettings", "nav-link active", 7, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9547) },
                    { new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9476), "nav-item", null, "far fa-circle nav-icon", true, false, "User", new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "Navigation Item", "/business/security", "nav-link", 5, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9476) },
                    { new Guid("c15215c8-32ca-4182-9510-b57419708a80"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9472), "nav-item", "fas fa-angle-left right", "nav-icon fas fa-cog", true, false, "Security", null, "Navigation Item", "", "nav-link active", 4, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9472) },
                    { new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9460), "nav-header", null, "", true, true, "Home", null, "Header", "", "", 1, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9461) },
                    { new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9465), "nav-item", null, "nav-icon fas fa-tachometer-alt", true, false, "Dashboard", null, "Navigation Item", "/business/home", "nav-link active", 2, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9465) },
                    { new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9468), "nav-header", null, "", true, true, "Business", null, "Header", "", "", 3, null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9469) }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "RoleName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9334), "User", true, "User", null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9334) },
                    { new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9324), "Admin", true, "Admin", null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9330) }
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FullName", "IsActive", "LastLoginAttemptAt", "LoginFailedAttemptsCount", "Password", "RoleId", "SaltKey", "UpdatedBy", "UpdatedDate", "UserName" },
                values: new object[,]
                {
                    { new Guid("c047d662-9f0e-4358-b323-15ec3081312c"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9679), "sbhowmikcse08@gmail.com", "Sreemonta Bhowmik", true, new DateTime(2023, 7, 16, 0, 8, 7, 967, DateTimeKind.Local).AddTicks(9668), 0, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9680), "sree" },
                    { new Guid("efedc118-3459-4c2e-9158-ad69196a59e0"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9685), "rohine2008@gmail.com", "Anannya Rohine", true, new DateTime(2023, 7, 16, 0, 8, 7, 967, DateTimeKind.Local).AddTicks(9684), 0, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9685), "rohine" }
                });

            migrationBuilder.InsertData(
                table: "UserRoleMenu",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "MenuId", "RoleId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("2d6dffc0-3c90-4a2a-8755-6a07fd6b1aee"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9630), true, new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9630) },
                    { new Guid("39d64f8a-a4c7-4cdb-8be0-785f93c7f01a"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9609), true, new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9609) },
                    { new Guid("4379abf7-a1c3-4f39-a60b-043cd57be1e1"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9612), true, new Guid("c15215c8-32ca-4182-9510-b57419708a80"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9613) },
                    { new Guid("921c8de1-7a7a-409d-816d-30c3768bbc34"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9640), true, new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9641) },
                    { new Guid("a270a25f-e808-4071-9dff-285e98c41f73"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9644), true, new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9644) },
                    { new Guid("efd3a617-bce0-49e1-a57b-876d7fa8513e"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9633), true, new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9634) },
                    { new Guid("f45cef7d-b805-4f2c-8725-b3eb4a089dde"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9637), true, new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9637) },
                    { new Guid("fb5af66d-ba07-4f88-acd5-e91c38483633"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9605), true, new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9606) },
                    { new Guid("fcb6cbdf-bf86-4ec3-b0c2-45dcab597878"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9600), true, new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2023, 7, 15, 20, 8, 7, 967, DateTimeKind.Utc).AddTicks(9601) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_RoleId",
                table: "UserInfo",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMenu_MenuId",
                table: "UserRoleMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMenu_RoleId",
                table: "UserRoleMenu",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRoleMenu");

            migrationBuilder.DropTable(
                name: "UserMenu");

            migrationBuilder.DropTable(
                name: "UserRole");
        }
    }
}
