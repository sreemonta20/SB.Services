using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.Security.Persistence;

#nullable disable

namespace SBERP.Security.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProfilePhotoId",
                table: "AppUserProfilesLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoLocation",
                table: "AppUserProfilesLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfilePhotoId",
                table: "AppUserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoLocation",
                table: "AppUserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // SqlScriptHelper.ApplyAll moved to 20261028200556_AddUserProfileJobTitle (newest migration).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhotoId",
                table: "AppUserProfilesLog");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoLocation",
                table: "AppUserProfilesLog");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoId",
                table: "AppUserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoLocation",
                table: "AppUserProfiles");
        }
    }
}
