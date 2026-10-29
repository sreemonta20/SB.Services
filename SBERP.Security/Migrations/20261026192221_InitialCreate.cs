using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.Security.Persistence;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBERP.Security.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsHeader = table.Column<bool>(type: "bit", nullable: true),
                    IsModule = table.Column<bool>(type: "bit", nullable: true),
                    IsSubModule = table.Column<bool>(type: "bit", nullable: false),
                    IsComponent = table.Column<bool>(type: "bit", nullable: true),
                    CssClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsRouteLink = table.Column<bool>(type: "bit", nullable: true),
                    RouteLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RouteLinkClass = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DropdownIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserMenusLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsHeader = table.Column<bool>(type: "bit", nullable: true),
                    IsModule = table.Column<bool>(type: "bit", nullable: true),
                    IsSubModule = table.Column<bool>(type: "bit", nullable: true),
                    IsComponent = table.Column<bool>(type: "bit", nullable: true),
                    CssClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsRouteLink = table.Column<bool>(type: "bit", nullable: true),
                    RouteLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RouteLinkClass = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DropdownIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserMenusLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProfilesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AppUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfilesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoleMenusLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserRoleMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsView = table.Column<bool>(type: "bit", nullable: true),
                    IsCreate = table.Column<bool>(type: "bit", nullable: true),
                    IsUpdate = table.Column<bool>(type: "bit", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoleMenusLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRolesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRolesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsersLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SaltKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsersLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailOutbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ToName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginDeactivations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastWorkingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeactivateOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginDeactivations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AppUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserProfiles_AppUserRole",
                        column: x => x.AppUserRoleId,
                        principalTable: "AppUserRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoleMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsView = table.Column<bool>(type: "bit", nullable: true),
                    IsCreate = table.Column<bool>(type: "bit", nullable: true),
                    IsUpdate = table.Column<bool>(type: "bit", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoleMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserRoleMenus_AppUserMenu",
                        column: x => x.AppUserMenuId,
                        principalTable: "AppUserMenus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppUserRoleMenus_AppUserRole",
                        column: x => x.AppUserRoleId,
                        principalTable: "AppUserRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SaltKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LockoutEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUser_AppUserProfiles",
                        column: x => x.AppUserProfileId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RouteLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AppUserProfile",
                        column: x => x.RecipientProfileId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    FamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    SessionExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RevokedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserRefreshTokens_AppUsers",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppUserMenus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "CssClass", "DropdownIcon", "Icon", "IsActive", "IsComponent", "IsHeader", "IsModule", "IsRouteLink", "IsSubModule", "Name", "ParentId", "Remark", "RouteLink", "RouteLinkClass", "SerialNo", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("05fbca7e-8d98-4c4b-9529-4dfb29db65fd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-cogs nav-icon", true, true, false, false, true, false, "HR Settings", new Guid("5e1d0000-0000-4000-8000-000000000100"), "Navigation Item", "/business/humanresources/settings", "nav-link", 23, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("082295e5-cd29-4693-8cf9-8082d4f8e791"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-industry nav-icon", true, true, false, false, true, false, "Company", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/humanresources/company", "nav-link", 13, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("0e5e1b9e-840d-4f96-abd1-fdb3efb8a55d"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-user-lock nav-icon", true, true, false, false, true, false, "User Role Menu", new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "Navigation Item", "/business/security/appuserrolemenu", "nav-link", 7, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("171253a9-0beb-4722-8fc4-a4c18895b980"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-user-tie nav-icon", true, true, false, false, true, false, "Employee", new Guid("5e1d0000-0000-4000-8000-000000000100"), "Navigation Item", "/business/humanresources/employee", "nav-link", 22, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-users nav-icon", true, false, false, true, false, false, "Human Resources", new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), "Navigation Item", "", "nav-link", 11, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-header", null, "", true, false, true, false, false, false, "Settings", null, "Header", "", "", 9, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "nav-icon fas fa-cog", true, false, false, true, true, false, "App Settings", new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), "Navigation Item", "/business/appsettings", "nav-link", 10, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-building nav-icon", true, false, false, false, false, true, "Organization Setup", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 12, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000002"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-chair nav-icon", true, true, false, false, true, false, "Position", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/orgsetup/position", "nav-link", 17, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000003"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-coins nav-icon", true, true, false, false, true, false, "Cost Center", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/orgsetup/cost-center", "nav-link", 18, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000004"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-layer-group nav-icon", true, true, false, false, true, false, "Job Grade", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/orgsetup/job-grade", "nav-link", 19, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000005"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-calendar-alt nav-icon", true, true, false, false, true, false, "Holiday Calendar", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/orgsetup/holiday-calendar", "nav-link", 20, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000006"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-user-plus nav-icon", true, false, false, false, false, true, "Recruitment", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 24, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000007"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-file-signature nav-icon", true, true, false, false, true, false, "Requisition", new Guid("5e1d0000-0000-4000-8000-000000000006"), "Navigation Item", "/business/recruitment/requisition", "nav-link", 25, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000008"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-bullhorn nav-icon", true, true, false, false, true, false, "Job Posting", new Guid("5e1d0000-0000-4000-8000-000000000006"), "Navigation Item", "/business/recruitment/job-posting", "nav-link", 26, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000009"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-stream nav-icon", true, true, false, false, true, false, "Applicant Pipeline", new Guid("5e1d0000-0000-4000-8000-000000000006"), "Navigation Item", "/business/recruitment/pipeline", "nav-link", 27, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000010"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-handshake nav-icon", true, false, false, false, false, true, "Onboarding", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 28, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000011"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-clipboard-check nav-icon", true, true, false, false, true, false, "Onboarding", new Guid("5e1d0000-0000-4000-8000-000000000010"), "Navigation Item", "/business/onboarding/list", "nav-link", 29, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000012"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-clock nav-icon", true, false, false, false, false, true, "Time & Attendance", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 30, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000013"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-business-time nav-icon", true, true, false, false, true, false, "Shift", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/shift", "nav-link", 31, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000014"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-hourglass-half nav-icon", true, true, false, false, true, false, "Overtime", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/overtime", "nav-link", 32, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000015"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-calendar-check nav-icon", true, true, false, false, true, false, "Attendance Summary", new Guid("5e1d0000-0000-4000-8000-000000000012"), "Navigation Item", "/business/attendance/summary", "nav-link", 33, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000016"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-plane-departure nav-icon", true, false, false, false, false, true, "Leave", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 34, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000017"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-paper-plane nav-icon", true, true, false, false, true, false, "Leave Application", new Guid("5e1d0000-0000-4000-8000-000000000016"), "Navigation Item", "/business/leave/applications", "nav-link", 35, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000018"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-tags nav-icon", true, true, false, false, true, false, "Leave Type", new Guid("5e1d0000-0000-4000-8000-000000000016"), "Navigation Item", "/business/leave/types", "nav-link", 36, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000019"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-gavel nav-icon", true, true, false, false, true, false, "Leave Policy", new Guid("5e1d0000-0000-4000-8000-000000000016"), "Navigation Item", "/business/leave/policies", "nav-link", 37, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000020"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-money-check-alt nav-icon", true, false, false, false, false, true, "Payroll", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 38, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000021"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-play-circle nav-icon", true, true, false, false, true, false, "Payroll Run", new Guid("5e1d0000-0000-4000-8000-000000000020"), "Navigation Item", "/business/payroll/runs", "nav-link", 39, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000022"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-sitemap nav-icon", true, true, false, false, true, false, "Salary Structure", new Guid("5e1d0000-0000-4000-8000-000000000020"), "Navigation Item", "/business/payroll/salary-structures", "nav-link", 40, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000023"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-puzzle-piece nav-icon", true, true, false, false, true, false, "Pay Component", new Guid("5e1d0000-0000-4000-8000-000000000020"), "Navigation Item", "/business/payroll/components", "nav-link", 41, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000024"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-hand-holding-usd nav-icon", true, true, false, false, true, false, "Loan / Advance", new Guid("5e1d0000-0000-4000-8000-000000000020"), "Navigation Item", "/business/payroll/loans", "nav-link", 42, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000025"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-chart-line nav-icon", true, false, false, false, false, true, "Performance", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 43, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000026"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-sync-alt nav-icon", true, true, false, false, true, false, "Appraisal Cycle", new Guid("5e1d0000-0000-4000-8000-000000000025"), "Navigation Item", "/business/performance/cycles", "nav-link", 44, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000027"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-bullseye nav-icon", true, true, false, false, true, false, "Goal", new Guid("5e1d0000-0000-4000-8000-000000000025"), "Navigation Item", "/business/performance/goals", "nav-link", 45, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000028"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-star-half-alt nav-icon", true, true, false, false, true, false, "Review", new Guid("5e1d0000-0000-4000-8000-000000000025"), "Navigation Item", "/business/performance/reviews", "nav-link", 46, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000029"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-brain nav-icon", true, true, false, false, true, false, "Competency", new Guid("5e1d0000-0000-4000-8000-000000000025"), "Navigation Item", "/business/performance/competencies", "nav-link", 47, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000030"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-sign-out-alt nav-icon", true, false, false, false, false, true, "Separation", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 48, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000031"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-door-open nav-icon", true, true, false, false, true, false, "Separation", new Guid("5e1d0000-0000-4000-8000-000000000030"), "Navigation Item", "/business/separation", "nav-link", 49, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d0000-0000-4000-8000-000000000100"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-id-card nav-icon", true, false, false, false, false, true, "Core HR", new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), "Navigation Item", "", "nav-link", 21, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-briefcase nav-icon", true, true, false, false, true, false, "User Role", new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "Navigation Item", "/business/security/appuserrole", "nav-link", 5, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("88664718-2161-44ce-b940-39c952e7f56c"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-code-branch nav-icon", true, true, false, false, true, false, "Branch", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/humanresources/branch", "nav-link", 14, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("89614ef6-4931-458f-8611-de1ea89a2c2d"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-briefcase nav-icon", true, true, false, false, true, false, "Designation", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/humanresources/designation", "nav-link", 16, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("904b0d8a-cc96-4930-bb1c-65838af2b2ef"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-user-cog nav-icon", true, true, false, false, true, false, "User Menu", new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "Navigation Item", "/business/security/appusermenu", "nav-link", 6, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("96695ca5-6010-474c-bfd1-f9746fabd81a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-user-circle nav-icon", true, true, false, false, true, false, "User Profile", new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "Navigation Item", "/business/security/appuserprofile", "nav-link", 8, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0116982-f0b2-4c4e-ab05-0a59b0b2a0b8"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "fas fa-sitemap nav-icon", true, true, false, false, true, false, "Department", new Guid("5e1d0000-0000-4000-8000-000000000001"), "Navigation Item", "/business/humanresources/department", "nav-link", 15, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c15215c8-32ca-4182-9510-b57419708a80"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", "fas fa-angle-left right", "fas fa-shield-alt nav-icon", true, false, false, true, false, false, "Security", new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), "Navigation Item", "", "nav-link", 4, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-header", null, "", true, false, true, false, false, false, "Home", null, "Header", "", "", 1, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-item", null, "nav-icon fas fa-tachometer-alt", true, false, false, true, true, false, "Dashboard", new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), "Navigation Item", "/business/home", "nav-link", 2, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc), "nav-header", null, "", true, false, true, false, false, false, "Business", null, "Header", "", "", 3, null, new DateTime(2024, 1, 1, 10, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActive", "RoleName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "1B15CE5A-56B3-4EB9-8286-6E27F770B0DA", new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "User", true, "User", null, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Admin", true, "Admin", null, new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUserProfiles",
                columns: new[] { "Id", "Address", "AppUserRoleId", "CreatedBy", "CreatedDate", "Email", "EmployeeId", "FullName", "IsActive", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("c047d662-9f0e-4358-b323-15ec3081312c"), "Dubai", new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), null, new DateTime(2024, 1, 1, 10, 5, 0, 0, DateTimeKind.Utc), "sbhowmikcse08@gmail.com", null, "Sreemonta Bhowmik", true, null, new DateTime(2024, 1, 1, 10, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("efedc118-3459-4c2e-9158-ad69196a59e0"), "Dubai", new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 5, 0, 0, DateTimeKind.Utc), "rohine2008@gmail.com", null, "Anannya Rohine", true, null, new DateTime(2024, 1, 1, 10, 5, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUserRoleMenus",
                columns: new[] { "Id", "AppUserMenuId", "AppUserRoleId", "CreatedBy", "CreatedDate", "IsActive", "IsCreate", "IsDelete", "IsUpdate", "IsView", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("00865190-7ecb-4c61-b2bf-ce4dcdf83092"), new Guid("082295e5-cd29-4693-8cf9-8082d4f8e791"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("0ba66610-aa06-4fcb-a350-b9f9018e5fd9"), new Guid("05fbca7e-8d98-4c4b-9529-4dfb29db65fd"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("1122ebc9-40b0-43d1-bd60-f3b0f7666ea3"), new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("1d3bee1c-6126-4113-8599-c64c7f1d47f6"), new Guid("db0085b7-695d-4751-a190-6c52e3bb44f1"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("228de240-db62-4e40-8ef0-367c0f71286d"), new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), false, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5b40c437-8827-413b-92a7-239b42186a05"), new Guid("171253a9-0beb-4722-8fc4-a4c18895b980"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5bca0329-727c-46c1-9c05-7f54292b0b17"), new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000001"), new Guid("5e1d0000-0000-4000-8000-000000000001"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000002"), new Guid("5e1d0000-0000-4000-8000-000000000002"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000003"), new Guid("5e1d0000-0000-4000-8000-000000000003"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000004"), new Guid("5e1d0000-0000-4000-8000-000000000004"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000005"), new Guid("5e1d0000-0000-4000-8000-000000000005"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000006"), new Guid("5e1d0000-0000-4000-8000-000000000006"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000007"), new Guid("5e1d0000-0000-4000-8000-000000000007"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000008"), new Guid("5e1d0000-0000-4000-8000-000000000008"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000009"), new Guid("5e1d0000-0000-4000-8000-000000000009"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000010"), new Guid("5e1d0000-0000-4000-8000-000000000010"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000011"), new Guid("5e1d0000-0000-4000-8000-000000000011"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000012"), new Guid("5e1d0000-0000-4000-8000-000000000012"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000013"), new Guid("5e1d0000-0000-4000-8000-000000000013"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000014"), new Guid("5e1d0000-0000-4000-8000-000000000014"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000015"), new Guid("5e1d0000-0000-4000-8000-000000000015"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000016"), new Guid("5e1d0000-0000-4000-8000-000000000016"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000017"), new Guid("5e1d0000-0000-4000-8000-000000000016"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000018"), new Guid("5e1d0000-0000-4000-8000-000000000017"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000019"), new Guid("5e1d0000-0000-4000-8000-000000000017"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000020"), new Guid("5e1d0000-0000-4000-8000-000000000018"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000021"), new Guid("5e1d0000-0000-4000-8000-000000000019"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000022"), new Guid("5e1d0000-0000-4000-8000-000000000020"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000023"), new Guid("5e1d0000-0000-4000-8000-000000000021"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000024"), new Guid("5e1d0000-0000-4000-8000-000000000022"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000025"), new Guid("5e1d0000-0000-4000-8000-000000000023"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000026"), new Guid("5e1d0000-0000-4000-8000-000000000024"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000027"), new Guid("5e1d0000-0000-4000-8000-000000000025"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000028"), new Guid("5e1d0000-0000-4000-8000-000000000025"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000029"), new Guid("5e1d0000-0000-4000-8000-000000000026"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000030"), new Guid("5e1d0000-0000-4000-8000-000000000027"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000031"), new Guid("5e1d0000-0000-4000-8000-000000000027"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000032"), new Guid("5e1d0000-0000-4000-8000-000000000028"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000033"), new Guid("5e1d0000-0000-4000-8000-000000000028"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000034"), new Guid("5e1d0000-0000-4000-8000-000000000029"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000035"), new Guid("5e1d0000-0000-4000-8000-000000000030"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000036"), new Guid("5e1d0000-0000-4000-8000-000000000031"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000100"), new Guid("5e1d0000-0000-4000-8000-000000000100"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e1d1000-0000-4000-8000-000000000101"), new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("698ece21-fd28-44ba-95f1-1ffef963c5b5"), new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6ee5cff9-9cef-450d-85a7-b9b1fc3b1431"), new Guid("0e5e1b9e-840d-4f96-abd1-fdb3efb8a55d"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, false, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("87f5e973-b247-4fa9-8c48-60f3fd6f2021"), new Guid("c15215c8-32ca-4182-9510-b57419708a80"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8dd17156-46b2-4199-ad17-b32785209468"), new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("90503e10-3fc1-4dc2-a39b-2f2a9aa198f1"), new Guid("60aadc18-6b91-4cee-ace7-97700b685c98"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9119fbda-76e6-4098-9ada-b0e8b027fc24"), new Guid("52f916cc-6c4d-4b4f-b884-4e89f1489b8d"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), false, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("99c2d21a-2197-4334-a86c-ae9416dbfb67"), new Guid("96695ca5-6010-474c-bfd1-f9746fabd81a"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b136043f-372b-49d5-9a26-ad6331b89724"), new Guid("88664718-2161-44ce-b940-39c952e7f56c"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b5aaf803-3ba5-4003-804a-c8bb8973a481"), new Guid("f0f0183b-7f60-4eb4-97d8-d2c15a4ae62c"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bb58e8fa-60cc-4505-bf83-21495ed9f8e3"), new Guid("904b0d8a-cc96-4930-bb1c-65838af2b2ef"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbacb3f-e5b8-4ad7-a88f-894af652a62f"), new Guid("a0116982-f0b2-4c4e-ab05-0a59b0b2a0b8"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c90c4eb9-a475-4c07-ac6c-ea4c22b93eb6"), new Guid("89614ef6-4931-458f-8611-de1ea89a2c2d"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cb2f5feb-0c2e-4a7f-9483-0e61fc83b140"), new Guid("2ac99ae0-c481-4a9c-a129-4cb3024b642e"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, false, false, false, false, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dc00dc24-6d35-47ee-8ced-5f119721628e"), new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, false, false, false, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e7e675a8-f9aa-4def-b952-eee1b18c708a"), new Guid("52d7e13b-ef24-4f17-937b-d6e8005a6658"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("eb3008bc-7ef6-42cf-a6af-50e720e47b8e"), new Guid("c15215c8-32ca-4182-9510-b57419708a80"), new Guid("10a9e9e7-cb24-4816-9b94-9db275a40edd"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, null, null, null, null, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f6f1bfa4-1aa9-45e8-876d-cf1c4d959c38"), new Guid("e8038aef-f00b-4d01-a5d3-99da9cc1a56b"), new Guid("1b15ce5a-56b3-4eb9-8286-6e27f770b0da"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc), true, true, true, true, true, null, new DateTime(2024, 1, 1, 10, 20, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AppUserProfileId", "CreatedBy", "CreatedDate", "IsActive", "LockoutEnd", "Password", "SaltKey", "UpdatedBy", "UpdatedDate", "UserName" },
                values: new object[,]
                {
                    { new Guid("5fd67aaf-183e-48f8-bb53-15b7628a3e0a"), new Guid("c047d662-9f0e-4358-b323-15ec3081312c"), null, new DateTime(2024, 1, 1, 10, 10, 0, 0, DateTimeKind.Utc), true, null, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2024, 1, 1, 10, 10, 0, 0, DateTimeKind.Utc), "sree" },
                    { new Guid("a9e00f47-89ec-46a3-a5e8-31ebd52ac121"), new Guid("efedc118-3459-4c2e-9158-ad69196a59e0"), null, new DateTime(2024, 1, 1, 10, 10, 0, 0, DateTimeKind.Utc), true, null, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2024, 1, 1, 10, 10, 0, 0, DateTimeKind.Utc), "rohine" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserProfiles_AppUserRoleId",
                table: "AppUserProfiles",
                column: "AppUserRoleId");

            migrationBuilder.CreateIndex(
                name: "UX_AppUserProfiles_EmployeeId",
                table: "AppUserProfiles",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRefreshTokens_AppUserId_RevokedDate",
                table: "AppUserRefreshTokens",
                columns: new[] { "AppUserId", "RevokedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRefreshTokens_FamilyId",
                table: "AppUserRefreshTokens",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "UX_AppUserRefreshTokens_TokenHash",
                table: "AppUserRefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoleMenus_AppUserMenuId",
                table: "AppUserRoleMenus",
                column: "AppUserMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoleMenus_AppUserRoleId",
                table: "AppUserRoleMenus",
                column: "AppUserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_AppUserProfileId",
                table: "AppUsers",
                column: "AppUserProfileId",
                unique: true,
                filter: "[AppUserProfileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_AppUsers_UserName",
                table: "AppUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailOutbox_Unprocessed",
                table: "EmailOutbox",
                columns: new[] { "ProcessedDate", "CreatedDate" },
                filter: "[ProcessedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoginDeactivations_Due",
                table: "LoginDeactivations",
                column: "DeactivateOn",
                filter: "[ProcessedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_LoginDeactivations_Separation",
                table: "LoginDeactivations",
                column: "SeparationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Recipient_Unread",
                table: "Notifications",
                columns: new[] { "RecipientProfileId", "IsRead", "CreatedDate" });

            // SQL objects are applied by the newest migration (SqlScriptHelper.ApplyAll), see AddUserProfilePhoto.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SQL objects go with their tables (triggers) or are left behind harmlessly; Down is for development only.
            migrationBuilder.DropTable(
                name: "AppUserMenusLog");

            migrationBuilder.DropTable(
                name: "AppUserProfilesLog");

            migrationBuilder.DropTable(
                name: "AppUserRefreshTokens");

            migrationBuilder.DropTable(
                name: "AppUserRoleMenus");

            migrationBuilder.DropTable(
                name: "AppUserRoleMenusLog");

            migrationBuilder.DropTable(
                name: "AppUserRolesLog");

            migrationBuilder.DropTable(
                name: "AppUsersLog");

            migrationBuilder.DropTable(
                name: "EmailOutbox");

            migrationBuilder.DropTable(
                name: "LoginDeactivations");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "AppUserMenus");

            migrationBuilder.DropTable(
                name: "AppUserProfiles");

            migrationBuilder.DropTable(
                name: "AppUserRoles");
        }
    }
}
