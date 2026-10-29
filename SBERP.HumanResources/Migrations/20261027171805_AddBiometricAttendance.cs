using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.HumanResources.Persistence;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBERP.HumanResources.Migrations
{
    /// <inheritdoc />
    public partial class AddBiometricAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "SEQ_BiometricRegistrationNo");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceDayCutoffTime",
                table: "HRSettings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuplicatePunchMinutes",
                table: "HRSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinMinutesBetweenInOut",
                table: "HRSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MobileAttendanceEnabled",
                table: "HRSettings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireOfficeNetwork",
                table: "HRSettings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "HRSettings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Protocol = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ApiKeyHash = table.Column<string>(type: "varchar(64)", nullable: true),
                    LastSeenDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastPunchDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDevicesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Protocol = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceDevicesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowedIpRanges = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AllowedWifiBssids = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 18, scale: 2, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 18, scale: 2, nullable: true),
                    RadiusMeters = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceZonesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceZoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowedIpRanges = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowedWifiBssids = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceZonesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiometricRegistrationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiometricRegistrationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiometricTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiometricTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBiometricRegistrationsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BiometricTypeId = table.Column<int>(type: "int", nullable: true),
                    MethodId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    DeviceInstallationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceUserPin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeBiometricRegistrationsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PunchSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PunchStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBiometricRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BiometricTypeId = table.Column<int>(type: "int", nullable: false),
                    MethodId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    DeviceInstallationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DevicePlatform = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeviceModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PublicKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KeyAlgorithm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceUserPin = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TemplateFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TemplateFormat = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TemplateSha256 = table.Column<string>(type: "varchar(64)", nullable: true),
                    TemplateSizeBytes = table.Column<int>(type: "int", nullable: true),
                    CapturedAngles = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConsentGiven = table.Column<bool>(type: "bit", nullable: false),
                    ConsentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUsedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBiometricRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiometricRegistrations_Device",
                        column: x => x.AttendanceDeviceId,
                        principalTable: "AttendanceDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BiometricRegistrations_Employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BiometricRegistrations_Status",
                        column: x => x.StatusId,
                        principalTable: "BiometricRegistrationStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BiometricRegistrations_Type",
                        column: x => x.BiometricTypeId,
                        principalTable: "BiometricTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttendancePunches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PunchTimeUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    PunchLocalTime = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "date", nullable: false),
                    PunchTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RejectReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BiometricTypeId = table.Column<int>(type: "int", nullable: true),
                    BiometricRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttendanceDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceUserPin = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ClientRequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientTime = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 18, scale: 2, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 18, scale: 2, nullable: true),
                    WifiBssid = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    WifiSsid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsWithinOffice = table.Column<bool>(type: "bit", nullable: true),
                    LocationCheck = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendancePunches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendancePunches_Employee",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendancePunches_Source",
                        column: x => x.SourceId,
                        principalTable: "PunchSources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttendancePunches_Status",
                        column: x => x.StatusId,
                        principalTable: "PunchStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AttendanceZones",
                columns: new[] { "Id", "AllowedIpRanges", "AllowedWifiBssids", "BranchId", "CreatedBy", "CreatedDate", "IsActive", "Latitude", "Longitude", "Name", "RadiusMeters", "Remarks", "UpdatedBy", "UpdatedDate", "ZoneCode" },
                values: new object[] { new Guid("0a7e0001-0000-4000-8000-000000000001"), "127.0.0.1/32,::1/128,10.0.0.0/8,172.16.0.0/12,192.168.0.0/16", null, null, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Office LAN (default)", null, "Seeded: private network ranges. Narrow to the office subnet / public IP before go-live.", null, null, "ZONE-LAN" });

            migrationBuilder.InsertData(
                table: "BiometricRegistrationStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending Verification", 1 },
                    { 2, "ACTIVE", true, "Active", 2 },
                    { 3, "REVOKED", true, "Revoked", 3 }
                });

            migrationBuilder.InsertData(
                table: "BiometricTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "FINGERPRINT", true, "Fingerprint", 1 },
                    { 2, "FACE", true, "Face", 2 }
                });

            migrationBuilder.UpdateData(
                table: "HRSettings",
                keyColumn: "Id",
                keyValue: new Guid("feed0001-0000-4000-8000-000000000001"),
                columns: new[] { "AttendanceDayCutoffTime", "DuplicatePunchMinutes", "MinMinutesBetweenInOut", "MobileAttendanceEnabled", "RequireOfficeNetwork", "TimeZoneId" },
                values: new object[] { "04:00", 1, 30, true, true, "Asia/Dubai" });

            migrationBuilder.InsertData(
                table: "PunchSources",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "MOBILE", true, "Mobile App", 1 },
                    { 2, "TERMINAL", true, "Door Terminal", 2 },
                    { 3, "MANUAL", true, "Manual", 3 }
                });

            migrationBuilder.InsertData(
                table: "PunchStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "ACCEPTED", true, "Accepted", 1 },
                    { 2, "DUPLICATE", true, "Duplicate", 2 },
                    { 3, "REJECTED", true, "Rejected", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "UX_AttendanceDevices_Code",
                table: "AttendanceDevices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AttendanceDevices_Serial",
                table: "AttendanceDevices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendancePunches_Emp_Date",
                table: "AttendancePunches",
                columns: new[] { "EmployeeId", "AttendanceDate", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendancePunches_StatusId",
                table: "AttendancePunches",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendancePunches_Time",
                table: "AttendancePunches",
                column: "PunchTimeUtc");

            migrationBuilder.CreateIndex(
                name: "UX_AttendancePunches_Request",
                table: "AttendancePunches",
                columns: new[] { "SourceId", "ClientRequestId" },
                unique: true,
                filter: "[ClientRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_AttendanceZones_Code",
                table: "AttendanceZones",
                column: "ZoneCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiometricRegistrations_Emp_Status",
                table: "EmployeeBiometricRegistrations",
                columns: new[] { "EmployeeId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_BiometricRegistrations_Pin",
                table: "EmployeeBiometricRegistrations",
                columns: new[] { "DeviceUserPin", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBiometricRegistrations_AttendanceDeviceId",
                table: "EmployeeBiometricRegistrations",
                column: "AttendanceDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBiometricRegistrations_BiometricTypeId",
                table: "EmployeeBiometricRegistrations",
                column: "BiometricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBiometricRegistrations_StatusId",
                table: "EmployeeBiometricRegistrations",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UX_BiometricRegistrations_No",
                table: "EmployeeBiometricRegistrations",
                column: "RegistrationNo",
                unique: true);

            // SQL objects are applied by the newest migration (SqlScriptHelper.ApplyAll), see AddEmployeePhoto.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceDevicesLog");

            migrationBuilder.DropTable(
                name: "AttendancePunches");

            migrationBuilder.DropTable(
                name: "AttendanceZones");

            migrationBuilder.DropTable(
                name: "AttendanceZonesLog");

            migrationBuilder.DropTable(
                name: "EmployeeBiometricRegistrations");

            migrationBuilder.DropTable(
                name: "EmployeeBiometricRegistrationsLog");

            migrationBuilder.DropTable(
                name: "PunchSources");

            migrationBuilder.DropTable(
                name: "PunchStatuses");

            migrationBuilder.DropTable(
                name: "AttendanceDevices");

            migrationBuilder.DropTable(
                name: "BiometricRegistrationStatuses");

            migrationBuilder.DropTable(
                name: "BiometricTypes");

            migrationBuilder.DropColumn(
                name: "AttendanceDayCutoffTime",
                table: "HRSettings");

            migrationBuilder.DropColumn(
                name: "DuplicatePunchMinutes",
                table: "HRSettings");

            migrationBuilder.DropColumn(
                name: "MinMinutesBetweenInOut",
                table: "HRSettings");

            migrationBuilder.DropColumn(
                name: "MobileAttendanceEnabled",
                table: "HRSettings");

            migrationBuilder.DropColumn(
                name: "RequireOfficeNetwork",
                table: "HRSettings");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "HRSettings");

            migrationBuilder.DropSequence(
                name: "SEQ_BiometricRegistrationNo");
        }
    }
}
