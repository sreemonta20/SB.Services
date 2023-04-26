using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SB.Security.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfos",
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
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserInfos",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FullName", "LastLoginAttemptAt", "LoginFailedAttemptsCount", "Password", "SaltKey", "UpdatedBy", "UpdatedDate", "UserName", "UserRole" },
                values: new object[] { new Guid("3b3ac0ec-ebaa-4e3d-8bec-8910814b6559"), null, new DateTime(2023, 4, 26, 15, 47, 15, 634, DateTimeKind.Utc).AddTicks(1330), "sbhowmikcse08@gmail.com", "Sreemonta Bhowmik", new DateTime(2023, 4, 26, 19, 47, 15, 634, DateTimeKind.Local).AddTicks(1197), 0, "$2b$10$dqPNaHnCGjUcvxXHTRXmDeNwNRQ0YI8kT9376noZw8i8tDj8KKoEa", "$2b$10$dqPNaHnCGjUcvxXHTRXmDe", null, new DateTime(2023, 4, 26, 15, 47, 15, 634, DateTimeKind.Utc).AddTicks(1334), "sree", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
