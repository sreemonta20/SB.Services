using System;
using Microsoft.EntityFrameworkCore.Migrations;
using SBERP.HumanResources.Persistence;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBERP.HumanResources.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationStages",
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
                    table.PrimaryKey("PK_ApplicationStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppraisalCycles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GoalDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RatingScaleMax = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppraisalCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppraisalCyclesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppraisalCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppraisalCyclesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppraisalCycleStatuses",
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
                    table.PrimaryKey("PK_AppraisalCycleStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRegularizationRequestsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegularizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegTypeId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceRegularizationRequestsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceUploadLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    TotalRows = table.Column<int>(type: "int", nullable: true),
                    SuccessRows = table.Column<int>(type: "int", nullable: true),
                    FailedRows = table.Column<int>(type: "int", nullable: true),
                    ErrorReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRolledBack = table.Column<bool>(type: "bit", nullable: true),
                    RolledBackDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceUploadLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BloodGroups",
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
                    table.PrimaryKey("PK_BloodGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHeadOffice = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_BranchesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalExperienceYears = table.Column<decimal>(type: "decimal(4,1)", precision: 18, scale: 2, nullable: true),
                    CurrentEmployer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentDesignation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NoticePeriodDays = table.Column<int>(type: "int", nullable: true),
                    HighestQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResumeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferredByEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidatesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlacklisted = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_CandidatesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClearanceDepartments",
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
                    table.PrimaryKey("PK_ClearanceDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClearanceStatuses",
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
                    table.PrimaryKey("PK_ClearanceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FinancialYearStartMonth = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompaniesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialYearStartMonth = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_CompaniesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Competencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetencyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetenciesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenciesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCentersLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCentersLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_DepartmentsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignationsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_DesignationsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmploymentStatus = table.Column<int>(type: "int", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfLeaving = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EmployeesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentStatuses",
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
                    table.PrimaryKey("PK_EmploymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
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
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExitInterviewsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExitInterviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OverallExperienceRating = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitInterviewsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
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
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoalsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppraisalCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoalStatuses",
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
                    table.PrimaryKey("PK_GoalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalendarCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayCalendarsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HolidayCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalendarCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCalendarsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayTypes",
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
                    table.PrimaryKey("PK_HolidayTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceSource = table.Column<int>(type: "int", nullable: false),
                    BiometricProvider = table.Column<int>(type: "int", nullable: true),
                    BiometricConnectionString = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BiometricSourceObject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OfficeStartTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OfficeEndTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GracePeriodMinutes = table.Column<int>(type: "int", nullable: true),
                    WeeklyOffDays = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AnnualLeaveDays = table.Column<int>(type: "int", nullable: true),
                    SickLeaveDays = table.Column<int>(type: "int", nullable: true),
                    CasualLeaveDays = table.Column<int>(type: "int", nullable: true),
                    AutoProcessTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AutoProcessEnabled = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsHardDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationOutbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AggregateType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewModes",
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
                    table.PrimaryKey("PK_InterviewModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewModeId = table.Column<int>(type: "int", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(3,1)", precision: 18, scale: 2, nullable: true),
                    Recommendation = table.Column<int>(type: "int", nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StageId = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_JobApplicationsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GradeLevel = table.Column<int>(type: "int", nullable: false),
                    Band = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    RefMinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RefMidSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RefMaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobGradesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeLevel = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGradesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobOffersLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobOfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    OfferedSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ConvertedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_JobOffersLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobPostingsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_JobPostingsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobPostingStatuses",
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
                    table.PrimaryKey("PK_JobPostingStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplicationsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalDays = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_LeaveApplicationsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplicationStatuses",
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
                    table.PrimaryKey("PK_LeaveApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeavePoliciesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeavePolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AnnualEntitlement = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: true),
                    EffectiveYear = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePoliciesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTransactionTypes",
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
                    table.PrimaryKey("PK_LeaveTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    AllowHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    IsCarryForward = table.Column<bool>(type: "bit", nullable: false),
                    MaxCarryForward = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: true),
                    AccrualPerMonth = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayslipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PayMonth = table.Column<int>(type: "int", nullable: false),
                    PayYear = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInstallments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoansLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoansLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanStatuses",
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
                    table.PrimaryKey("PK_LoanStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManpowerRequisitionsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManpowerRequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberOfVacancies = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ManpowerRequisitionsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatuses",
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
                    table.PrimaryKey("PK_MaritalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfferStatuses",
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
                    table.PrimaryKey("PK_OfferStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Onboardings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JobOfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ChecklistTotal = table.Column<int>(type: "int", nullable: false),
                    ChecklistDone = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Onboardings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobOfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_OnboardingsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingStatuses",
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
                    table.PrimaryKey("PK_OnboardingStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingTaskStatuses",
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
                    table.PrimaryKey("PK_OnboardingTaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeRequestsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OvertimeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hours = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_OvertimeRequestsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeStatuses",
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
                    table.PrimaryKey("PK_OvertimeStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComponentTypeId = table.Column<int>(type: "int", nullable: false),
                    CalcMethod = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsProrated = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayComponentsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComponentTypeId = table.Column<int>(type: "int", nullable: true),
                    CalcMethod = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayComponentsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayComponentTypes",
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
                    table.PrimaryKey("PK_PayComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PayMonth = table.Column<int>(type: "int", nullable: false),
                    PayYear = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkingDays = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TotalGross = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalNet = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EmployeeCount = table.Column<int>(type: "int", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRunsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayMonth = table.Column<int>(type: "int", nullable: true),
                    PayYear = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    TotalNet = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollRunsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollRunStatuses",
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
                    table.PrimaryKey("PK_PayrollRunStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BudgetedHeadcount = table.Column<int>(type: "int", nullable: true),
                    FilledHeadcount = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PositionsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PositionStatuses",
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
                    table.PrimaryKey("PK_PositionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatingItemTypes",
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
                    table.PrimaryKey("PK_RatingItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularizationStatuses",
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
                    table.PrimaryKey("PK_RegularizationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularizationTypes",
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
                    table.PrimaryKey("PK_RegularizationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionStatuses",
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
                    table.PrimaryKey("PK_RequisitionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppraisalCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStatuses",
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
                    table.PrimaryKey("PK_ReviewStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructuresLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalaryStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StructureCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructuresLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeparationClearancesLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationClearanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    ClearanceStatusId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeparationClearancesLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeparationsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeparationTypeId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    LastWorkingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeparationsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeparationStatuses",
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
                    table.PrimaryKey("PK_SeparationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeparationTypes",
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
                    table.PrimaryKey("PK_SeparationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftRostersLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftRosterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_ShiftRostersLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsNightShift = table.Column<bool>(type: "bit", nullable: false),
                    BreakMinutes = table.Column<int>(type: "int", nullable: false),
                    GraceInMinutes = table.Column<int>(type: "int", nullable: false),
                    GraceOutMinutes = table.Column<int>(type: "int", nullable: false),
                    HalfDayHours = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    FullDayHours = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    WeekOffDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
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
                    table.PrimaryKey("PK_ShiftsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsHeadOffice = table.Column<bool>(type: "bit", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    HolidayCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Branches_HolidayCalendar",
                        column: x => x.HolidayCalendarId,
                        principalTable: "HolidayCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HolidayDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HolidayCalendarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HolidayDate = table.Column<DateTime>(type: "date", nullable: false),
                    HolidayTypeId = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayDates_Calendar",
                        column: x => x.HolidayCalendarId,
                        principalTable: "HolidayCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayDates_Type",
                        column: x => x.HolidayTypeId,
                        principalTable: "HolidayTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewPanelMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewerEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPanelLead = table.Column<bool>(type: "bit", nullable: true),
                    IndividualRating = table.Column<decimal>(type: "decimal(3,1)", precision: 18, scale: 2, nullable: true),
                    IndividualNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewPanelMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Panel_Interview",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    AnnualEntitlement = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: false),
                    MinServiceMonths = table.Column<int>(type: "int", nullable: true),
                    MaxConsecutiveDays = table.Column<int>(type: "int", nullable: true),
                    EffectiveYear = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicies_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnboardingTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingTasks_Onboarding",
                        column: x => x.OnboardingId,
                        principalTable: "Onboardings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostCenterCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentCostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OwnerEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostCenters_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenters_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenters_Parent",
                        column: x => x.ParentCostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HeadEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Parent",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportsToPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    BudgetedHeadcount = table.Column<int>(type: "int", nullable: false),
                    FilledHeadcount = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    EmploymentType = table.Column<int>(type: "int", nullable: true),
                    WorkLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_CostCenter",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_Departments",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_Designations",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_JobGrade",
                        column: x => x.JobGradeId,
                        principalTable: "JobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_ReportsTo",
                        column: x => x.ReportsToPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_Status",
                        column: x => x.StatusId,
                        principalTable: "PositionStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OfficialEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    MaritalStatus = table.Column<int>(type: "int", nullable: true),
                    BloodGroup = table.Column<int>(type: "int", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassportExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AlternatePhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportingManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime", nullable: true),
                    ProbationEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateOfLeaving = table.Column<DateTime>(type: "datetime", nullable: true),
                    EmploymentType = table.Column<int>(type: "int", nullable: true),
                    EmploymentStatus = table.Column<int>(type: "int", nullable: true),
                    WorkLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SignatureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Company",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_CostCenter",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Department",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Designation",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_JobGrade",
                        column: x => x.JobGradeId,
                        principalTable: "JobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Position",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_ReportingManager",
                        column: x => x.ReportingManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManpowerRequisitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberOfVacancies = table.Column<int>(type: "int", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReplacement = table.Column<bool>(type: "bit", nullable: true),
                    ReplacingEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinExperienceYears = table.Column<int>(type: "int", nullable: true),
                    RequiredQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BudgetMin = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BudgetMax = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManpowerRequisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MRF_Positions",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRegularizationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegTypeId = table.Column<int>(type: "int", nullable: false),
                    RequestedCheckIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    RequestedCheckOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRegularizationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRegularizationRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    WorkingHours = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    UploadBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    Line1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Line2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAddresses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IbanNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoutingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBanks_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeCertifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    University = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    Grade = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEducations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEmergencyContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternatePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEmergencyContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEmergencyContacts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeExperiences_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTrainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTrainings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AppraisalCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_AppraisalCycles_AppraisalCycleId",
                        column: x => x.AppraisalCycleId,
                        principalTable: "AppraisalCycles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goals_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    TotalDays = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactDuringLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApplications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaveApplications_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Entitled = table.Column<decimal>(type: "decimal(6,1)", precision: 18, scale: 2, nullable: false),
                    CarriedForward = table.Column<decimal>(type: "decimal(6,1)", precision: 18, scale: 2, nullable: false),
                    Used = table.Column<decimal>(type: "decimal(6,1)", precision: 18, scale: 2, nullable: false),
                    Pending = table.Column<decimal>(type: "decimal(6,1)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaveBalances_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaveTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaveTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    Days = table.Column<decimal>(type: "decimal(6,1)", precision: 18, scale: 2, nullable: false),
                    LeaveApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveTransactions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoanCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InstallmentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalInstallments = table.Column<int>(type: "int", nullable: false),
                    PaidInstallments = table.Column<int>(type: "int", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OvertimeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ToTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayslipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayrollRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaidDays = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: false),
                    LopDays = table.Column<decimal>(type: "decimal(5,1)", precision: 18, scale: 2, nullable: false),
                    BasicEarned = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossEarnings = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payslips_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AppraisalCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SelfRating = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    ManagerRating = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    OverallRating = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    SelfComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeAcknowledgement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AppraisalCycles_AppraisalCycleId",
                        column: x => x.AppraisalCycleId,
                        principalTable: "AppraisalCycles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StructureCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryStructures_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Separations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoticeDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RequestedLastWorkingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastWorkingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    NoticePeriodDays = table.Column<int>(type: "int", nullable: true),
                    IsNoticeWaived = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExitInterviewDone = table.Column<bool>(type: "bit", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettlementRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRehireEligible = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Separations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Separations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShiftRosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftRosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftRosters_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShiftRosters_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostingCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ManpowerRequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentType = table.Column<int>(type: "int", nullable: true),
                    WorkLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRemote = table.Column<bool>(type: "bit", nullable: true),
                    SalaryRangeText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishChannels = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostings_MRF",
                        column: x => x.ManpowerRequisitionId,
                        principalTable: "ManpowerRequisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayslipLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayslipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComponentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComponentTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipLines_Payslips_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: false),
                    SelfScore = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    ManagerScore = table.Column<decimal>(type: "decimal(4,2)", precision: 18, scale: 2, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewRatings_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryStructureLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalaryStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalcMethod = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryStructureLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryStructureLines_PayComponents_PayComponentId",
                        column: x => x.PayComponentId,
                        principalTable: "PayComponents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalaryStructureLines_SalaryStructures_SalaryStructureId",
                        column: x => x.SalaryStructureId,
                        principalTable: "SalaryStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExitInterviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InterviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrimaryReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallExperienceRating = table.Column<int>(type: "int", nullable: true),
                    WouldRecommend = table.Column<bool>(type: "bit", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suggestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExitInterviews_Separations_SeparationId",
                        column: x => x.SeparationId,
                        principalTable: "Separations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SeparationClearances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeparationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    ClearanceStatusId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClearedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClearedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeparationClearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeparationClearances_Separations_SeparationId",
                        column: x => x.SeparationId,
                        principalTable: "Separations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JobPostingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScreeningScore = table.Column<decimal>(type: "decimal(5,2)", precision: 18, scale: 2, nullable: true),
                    ScreeningNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedRecruiterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Candidate",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobApplications_Posting",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JobApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DesignationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OfferedSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfferValidTill = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfferLetterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclineReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConvertedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobOffers_Application",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobOffers_Candidate",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApplicationStages",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "APPLIED", true, "Applied", 1 },
                    { 2, "SCREENING", true, "Screening", 2 },
                    { 3, "SHORTLISTED", true, "Shortlisted", 3 },
                    { 4, "INTERVIEW", true, "Interviewing", 4 },
                    { 5, "OFFER", true, "Offer", 5 },
                    { 6, "HIRED", true, "Hired", 6 },
                    { 7, "REJECTED", true, "Rejected", 7 },
                    { 8, "WITHDRAWN", true, "Withdrawn", 8 }
                });

            migrationBuilder.InsertData(
                table: "AppraisalCycleStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "ACTIVE", true, "Active", 2 },
                    { 3, "CLOSED", true, "Closed", 3 }
                });

            migrationBuilder.InsertData(
                table: "BloodGroups",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "A_POS", true, "A+", 1 },
                    { 2, "A_NEG", true, "A-", 2 },
                    { 3, "B_POS", true, "B+", 3 },
                    { 4, "B_NEG", true, "B-", 4 },
                    { 5, "AB_POS", true, "AB+", 5 },
                    { 6, "AB_NEG", true, "AB-", 6 },
                    { 7, "O_POS", true, "O+", 7 },
                    { 8, "O_NEG", true, "O-", 8 }
                });

            migrationBuilder.InsertData(
                table: "ClearanceDepartments",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "IT", true, "IT / Systems", 1 },
                    { 2, "FINANCE", true, "Finance / Accounts", 2 },
                    { 3, "HR", true, "Human Resources", 3 },
                    { 4, "ADMIN", true, "Admin / Facilities", 4 },
                    { 5, "MANAGER", true, "Reporting Manager", 5 }
                });

            migrationBuilder.InsertData(
                table: "ClearanceStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending", 1 },
                    { 2, "CLEARED", true, "Cleared", 2 },
                    { 3, "BLOCKED", true, "Blocked", 3 }
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "City", "CompanyCode", "Country", "CreatedBy", "CreatedDate", "CurrencyCode", "Email", "FinancialYearStartMonth", "IsActive", "LegalName", "LogoUrl", "Name", "Phone", "RegistrationNumber", "TaxNumber", "UpdatedBy", "UpdatedDate", "Website" },
                values: new object[] { new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), null, null, "SBERP", null, "SYSTEM", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, 1, true, "SBERP Default Company LLC", null, "SBERP Default Company", null, null, null, null, new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "Competencies",
                columns: new[] { "Id", "CompetencyCode", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "SortOrder", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0c0e0000-0000-4000-8000-000000000001"), "COMM", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Clarity, listening, and stakeholder updates", true, "Communication", 1, null, null },
                    { new Guid("0c0e0000-0000-4000-8000-000000000002"), "OWN", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Accountability and follow-through", true, "Ownership", 2, null, null },
                    { new Guid("0c0e0000-0000-4000-8000-000000000003"), "TEAM", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Collaboration and supporting peers", true, "Teamwork", 3, null, null },
                    { new Guid("0c0e0000-0000-4000-8000-000000000004"), "QUAL", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Accuracy and attention to detail", true, "Quality", 4, null, null },
                    { new Guid("0c0e0000-0000-4000-8000-000000000005"), "INNO", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Initiative and process improvement", true, "Innovation", 5, null, null },
                    { new Guid("0c0e0000-0000-4000-8000-000000000006"), "CUST", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Understands and serves internal and external customers", true, "Customer Focus", 6, null, null }
                });

            migrationBuilder.InsertData(
                table: "Designations",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate", "Description", "Grade", "IsActive", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("368c0e9b-e60d-40cf-912c-43744db1b19f"), "SWE", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 7, true, "Software Engineer", null, null },
                    { new Guid("4e2e6864-b92c-4224-8bef-20f7e72df648"), "CEO", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Chief Executive Officer", null, null },
                    { new Guid("51da7cd0-7d22-4c3b-a01e-21e1eafd27e9"), "SE", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 6, true, "Senior Engineer", null, null },
                    { new Guid("60bbb9db-3a9e-4560-bb0f-c93dbaf9c7db"), "HRM", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "HR Manager", null, null },
                    { new Guid("82b5f6d0-5403-48d0-8d1c-8ef510762631"), "ACC", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 6, true, "Accountant", null, null },
                    { new Guid("e619d48f-bd89-404f-a406-f466f1b62088"), "MGR", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "Manager", null, null },
                    { new Guid("f2597dae-b3c6-47bb-b2a1-5caa5dcfd770"), "INT", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 9, true, "Intern", null, null },
                    { new Guid("f46d487d-f446-4e3c-b06f-8263dc854c89"), "TL", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 5, true, "Team Lead", null, null }
                });

            migrationBuilder.InsertData(
                table: "EmploymentStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "ACTIVE", true, "Active", 1 },
                    { 2, "ON_LEAVE", true, "On Leave", 2 },
                    { 3, "SUSPENDED", true, "Suspended", 3 },
                    { 4, "RESIGNED", true, "Resigned", 4 },
                    { 5, "TERMINATED", true, "Terminated", 5 },
                    { 6, "RETIRED", true, "Retired", 6 }
                });

            migrationBuilder.InsertData(
                table: "EmploymentTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "FULL_TIME", true, "Full Time", 1 },
                    { 2, "PART_TIME", true, "Part Time", 2 },
                    { 3, "CONTRACT", true, "Contract", 3 },
                    { 4, "INTERN", true, "Intern", 4 },
                    { 5, "CONSULTANT", true, "Consultant", 5 },
                    { 6, "PROBATION", true, "Probation", 6 }
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "MALE", true, "Male", 1 },
                    { 2, "FEMALE", true, "Female", 2 },
                    { 3, "OTHER", true, "Other", 3 }
                });

            migrationBuilder.InsertData(
                table: "GoalStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "ACTIVE", true, "Active", 2 },
                    { 3, "COMPLETED", true, "Completed", 3 },
                    { 4, "CANCELLED", true, "Cancelled", 4 }
                });

            migrationBuilder.InsertData(
                table: "HRSettings",
                columns: new[] { "Id", "AnnualLeaveDays", "AttendanceSource", "AutoProcessEnabled", "AutoProcessTime", "BiometricConnectionString", "BiometricProvider", "BiometricSourceObject", "CasualLeaveDays", "CreatedBy", "CreatedDate", "GracePeriodMinutes", "IsActive", "IsHardDelete", "OfficeEndTime", "OfficeStartTime", "SickLeaveDays", "UpdatedBy", "UpdatedDate", "WeeklyOffDays" },
                values: new object[] { new Guid("feed0001-0000-4000-8000-000000000001"), 22, 1, false, "23:30", null, 0, null, 5, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 15, true, true, "17:30", "08:30", 10, null, null, "5,6" });

            migrationBuilder.InsertData(
                table: "HolidayCalendars",
                columns: new[] { "Id", "CalendarCode", "Country", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "UpdatedBy", "UpdatedDate", "Year" },
                values: new object[] { new Guid("0c0a1d00-0000-4000-8000-000000002026"), "HC-0001", "United Arab Emirates", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Islamic holiday dates are moon-sighting estimates — confirm against the official announcement.", true, "UAE Public Holidays 2026", null, null, 2026 });

            migrationBuilder.InsertData(
                table: "HolidayTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PUBLIC", true, "Public Holiday", 1 },
                    { 2, "RELIGIOUS", true, "Religious", 2 },
                    { 3, "NATIONAL", true, "National", 3 },
                    { 4, "COMPANY", true, "Company Holiday", 4 },
                    { 5, "OPTIONAL", true, "Optional / Restricted", 5 }
                });

            migrationBuilder.InsertData(
                table: "InterviewModes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "INPERSON", true, "In Person", 1 },
                    { 2, "VIDEO", true, "Video Call", 2 },
                    { 3, "PHONE", true, "Phone", 3 },
                    { 4, "TEST", true, "Assessment Test", 4 }
                });

            migrationBuilder.InsertData(
                table: "JobGrades",
                columns: new[] { "Id", "Band", "CreatedBy", "CreatedDate", "CurrencyCode", "Description", "GradeCode", "GradeLevel", "IsActive", "Name", "RefMaxSalary", "RefMidSalary", "RefMinSalary", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("06ad0001-0000-4000-8000-000000000001"), "Leadership", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0001", 1, true, "Executive", 100000m, 80000m, 60000m, null, null },
                    { new Guid("06ad0001-0000-4000-8000-000000000002"), "Management", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0002", 2, true, "Senior Manager", 60000m, 50000m, 40000m, null, null },
                    { new Guid("06ad0001-0000-4000-8000-000000000003"), "Management", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0003", 3, true, "Manager", 40000m, 34000m, 28000m, null, null },
                    { new Guid("06ad0001-0000-4000-8000-000000000004"), "Professional", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0004", 4, true, "Senior Specialist", 28000m, 23000m, 18000m, null, null },
                    { new Guid("06ad0001-0000-4000-8000-000000000005"), "Professional", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0005", 5, true, "Specialist", 18000m, 14000m, 10000m, null, null },
                    { new Guid("06ad0001-0000-4000-8000-000000000006"), "Support", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "AED", null, "GR-0006", 6, true, "Associate", 10000m, 7500m, 5000m, null, null }
                });

            migrationBuilder.InsertData(
                table: "JobPostingStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "PUBLISHED", true, "Published", 2 },
                    { 3, "PAUSED", true, "Paused", 3 },
                    { 4, "CLOSED", true, "Closed", 4 },
                    { 5, "CANCELLED", true, "Cancelled", 5 }
                });

            migrationBuilder.InsertData(
                table: "LeaveApplicationStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending", 1 },
                    { 2, "APPROVED", true, "Approved", 2 },
                    { 3, "REJECTED", true, "Rejected", 3 },
                    { 4, "CANCELLED", true, "Cancelled", 4 },
                    { 5, "WITHDRAWN", true, "Withdrawn", 5 }
                });

            migrationBuilder.InsertData(
                table: "LeaveTransactionTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "GRANT", true, "Annual Grant", 1 },
                    { 2, "ACCRUAL", true, "Monthly Accrual", 2 },
                    { 3, "CARRY_FORWARD", true, "Carry Forward", 3 },
                    { 4, "DEDUCTION", true, "Deduction (Approved Leave)", 4 },
                    { 5, "REVERSAL", true, "Reversal (Cancelled Leave)", 5 },
                    { 6, "ADJUSTMENT", true, "Manual Adjustment", 6 },
                    { 7, "ENCASHMENT", true, "Encashment", 7 }
                });

            migrationBuilder.InsertData(
                table: "LeaveTypes",
                columns: new[] { "Id", "AccrualPerMonth", "AllowHalfDay", "ColorHex", "CreatedBy", "CreatedDate", "Description", "IsActive", "IsCarryForward", "IsPaid", "LeaveTypeCode", "MaxCarryForward", "Name", "RequiresAttachment", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("01ea0000-0000-4000-8000-000000000001"), 2.5m, true, "#28a745", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "30 calendar days/year after 1 year of service (UAE)", true, true, true, "ANNUAL", 10m, "Annual Leave", false, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000002"), null, true, "#dc3545", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Medical certificate required", true, false, true, "SICK", null, "Sick Leave", true, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000003"), null, true, "#17a2b8", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Short personal absence", true, false, true, "CASUAL", null, "Casual Leave", false, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000004"), null, true, "#6c757d", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Leave without pay", true, false, false, "UNPAID", null, "Unpaid Leave", false, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000005"), null, false, "#e83e8c", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "60 days (UAE Labour Law)", true, false, true, "MATERNITY", null, "Maternity Leave", true, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000006"), null, false, "#fd7e14", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "5 working days (UAE Labour Law)", true, false, true, "PATERNITY", null, "Paternity Leave", false, null, null },
                    { new Guid("01ea0000-0000-4000-8000-000000000007"), null, false, "#6f42c1", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Once during service", true, false, false, "HAJJ", null, "Hajj Leave", false, null, null }
                });

            migrationBuilder.InsertData(
                table: "LoanStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "ACTIVE", true, "Active", 1 },
                    { 2, "CLOSED", true, "Closed", 2 },
                    { 3, "CANCELLED", true, "Cancelled", 3 }
                });

            migrationBuilder.InsertData(
                table: "MaritalStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "SINGLE", true, "Single", 1 },
                    { 2, "MARRIED", true, "Married", 2 },
                    { 3, "DIVORCED", true, "Divorced", 3 },
                    { 4, "WIDOWED", true, "Widowed", 4 },
                    { 5, "SEPARATED", true, "Separated", 5 }
                });

            migrationBuilder.InsertData(
                table: "OfferStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "ISSUED", true, "Issued", 2 },
                    { 3, "ACCEPTED", true, "Accepted", 3 },
                    { 4, "DECLINED", true, "Declined", 4 },
                    { 5, "EXPIRED", true, "Expired", 5 },
                    { 6, "WITHDRAWN", true, "Withdrawn", 6 }
                });

            migrationBuilder.InsertData(
                table: "OnboardingStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "INITIATED", true, "Initiated", 1 },
                    { 2, "INPROGRESS", true, "In Progress", 2 },
                    { 3, "COMPLETED", true, "Completed", 3 },
                    { 4, "CANCELLED", true, "Cancelled", 4 }
                });

            migrationBuilder.InsertData(
                table: "OnboardingTaskStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending", 1 },
                    { 2, "INPROGRESS", true, "In Progress", 2 },
                    { 3, "DONE", true, "Done", 3 },
                    { 4, "NA", true, "Not Applicable", 4 }
                });

            migrationBuilder.InsertData(
                table: "OvertimeStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending", 1 },
                    { 2, "APPROVED", true, "Approved", 2 },
                    { 3, "REJECTED", true, "Rejected", 3 },
                    { 4, "CANCELLED", true, "Cancelled", 4 }
                });

            migrationBuilder.InsertData(
                table: "PayComponentTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "EARNING", true, "Earning", 1 },
                    { 2, "DEDUCTION", true, "Deduction", 2 }
                });

            migrationBuilder.InsertData(
                table: "PayComponents",
                columns: new[] { "Id", "CalcMethod", "ComponentCode", "ComponentTypeId", "CreatedBy", "CreatedDate", "DefaultValue", "Description", "IsActive", "IsProrated", "IsTaxable", "Name", "SortOrder", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0fa10000-0000-4000-8000-000000000001"), 2, "HRA", 1, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 25m, "% of basic", true, true, false, "Housing Allowance", 1, null, null },
                    { new Guid("0fa10000-0000-4000-8000-000000000002"), 1, "TRANS", 1, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 800m, "Fixed monthly", true, true, false, "Transport Allowance", 2, null, null },
                    { new Guid("0fa10000-0000-4000-8000-000000000003"), 1, "PHONE", 1, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 200m, "Fixed monthly", true, true, false, "Telephone Allowance", 3, null, null },
                    { new Guid("0fa10000-0000-4000-8000-000000000004"), 2, "OTHER", 1, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 10m, "% of basic", true, true, false, "Other Allowance", 4, null, null },
                    { new Guid("0fa10000-0000-4000-8000-000000000005"), 2, "GPSSA", 2, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 11m, "Employee share, UAE/GCC nationals only", true, false, false, "Pension (GPSSA) — UAE nationals", 5, null, null },
                    { new Guid("0fa10000-0000-4000-8000-000000000006"), 1, "LOAN", 2, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 0m, "Driven by Loans module", true, false, false, "Loan / Advance Recovery", 6, null, null }
                });

            migrationBuilder.InsertData(
                table: "PayrollRunStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "PROCESSED", true, "Processed", 2 },
                    { 3, "APPROVED", true, "Approved", 3 },
                    { 4, "PAID", true, "Paid", 4 },
                    { 5, "CANCELLED", true, "Cancelled", 5 }
                });

            migrationBuilder.InsertData(
                table: "PositionStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "OPEN", true, "Open", 2 },
                    { 3, "FILLED", true, "Filled", 3 },
                    { 4, "ON_HOLD", true, "On Hold", 4 },
                    { 5, "CLOSED", true, "Closed", 5 }
                });

            migrationBuilder.InsertData(
                table: "RatingItemTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "GOAL", true, "Goal", 1 },
                    { 2, "COMPETENCY", true, "Competency", 2 }
                });

            migrationBuilder.InsertData(
                table: "RegularizationStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "PENDING", true, "Pending", 1 },
                    { 2, "APPROVED", true, "Approved", 2 },
                    { 3, "REJECTED", true, "Rejected", 3 },
                    { 4, "CANCELLED", true, "Cancelled", 4 }
                });

            migrationBuilder.InsertData(
                table: "RegularizationTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "MISSED_IN", true, "Missed Check-In", 1 },
                    { 2, "MISSED_OUT", true, "Missed Check-Out", 2 },
                    { 3, "WRONG_TIME", true, "Wrong Punch Time", 3 },
                    { 4, "FORGOT", true, "Forgot to Punch", 4 },
                    { 5, "ON_DUTY", true, "On Duty / Field Work", 5 }
                });

            migrationBuilder.InsertData(
                table: "RequisitionStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "PENDING", true, "Pending Approval", 2 },
                    { 3, "APPROVED", true, "Approved", 3 },
                    { 4, "REJECTED", true, "Rejected", 4 },
                    { 5, "ONHOLD", true, "On Hold", 5 },
                    { 6, "CLOSED", true, "Closed", 6 },
                    { 7, "FILLED", true, "Filled", 7 }
                });

            migrationBuilder.InsertData(
                table: "ReviewStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "NOT_STARTED", true, "Not Started", 1 },
                    { 2, "SELF_ASSESSMENT", true, "Self Assessment", 2 },
                    { 3, "MANAGER_REVIEW", true, "Manager Review", 3 },
                    { 4, "COMPLETED", true, "Completed", 4 },
                    { 5, "ACKNOWLEDGED", true, "Acknowledged", 5 }
                });

            migrationBuilder.InsertData(
                table: "SeparationStatuses",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "DRAFT", true, "Draft", 1 },
                    { 2, "PENDING", true, "Pending Approval", 2 },
                    { 3, "APPROVED", true, "Approved", 3 },
                    { 4, "IN_CLEARANCE", true, "In Clearance", 4 },
                    { 5, "SETTLED", true, "Settled", 5 },
                    { 6, "COMPLETED", true, "Completed", 6 },
                    { 7, "CANCELLED", true, "Cancelled", 7 }
                });

            migrationBuilder.InsertData(
                table: "SeparationTypes",
                columns: new[] { "Id", "Code", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "RESIGNATION", true, "Resignation", 1 },
                    { 2, "TERMINATION", true, "Termination", 2 },
                    { 3, "RETIREMENT", true, "Retirement", 3 },
                    { 4, "END_OF_CONTRACT", true, "End of Contract", 4 },
                    { 5, "DEATH", true, "Death", 5 }
                });

            migrationBuilder.InsertData(
                table: "Shifts",
                columns: new[] { "Id", "BreakMinutes", "CreatedBy", "CreatedDate", "Description", "EndTime", "FullDayHours", "GraceInMinutes", "GraceOutMinutes", "HalfDayHours", "IsActive", "IsNightShift", "Name", "ShiftCode", "StartTime", "UpdatedBy", "UpdatedDate", "WeekOffDays" },
                values: new object[,]
                {
                    { new Guid("05f10000-0000-4000-8000-000000000001"), 60, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "UAE standard office hours, Fri–Sat weekend", new TimeSpan(0, 17, 30, 0, 0), 8m, 15, 10, 4m, true, false, "General Shift", "SH-0001", new TimeSpan(0, 8, 30, 0, 0), null, null, "5,6" },
                    { new Guid("05f10000-0000-4000-8000-000000000002"), 0, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Reduced hours during Ramadan (UAE Labour Law)", new TimeSpan(0, 15, 0, 0, 0), 6m, 15, 10, 3m, true, false, "Ramadan Shift", "SH-0002", new TimeSpan(0, 9, 0, 0, 0), null, null, "5,6" },
                    { new Guid("05f10000-0000-4000-8000-000000000003"), 30, "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Operations night cover", new TimeSpan(0, 6, 0, 0, 0), 7.5m, 15, 10, 4m, true, true, "Night Shift", "SH-0003", new TimeSpan(0, 22, 0, 0, 0), null, null, "5" }
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Address", "BranchCode", "City", "CompanyId", "Country", "CreatedBy", "CreatedDate", "Email", "HolidayCalendarId", "IsActive", "IsHeadOffice", "Name", "Phone", "TimeZone", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), null, "HO", null, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), null, "SYSTEM", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), true, true, "Head Office", null, "Asia/Dubai", null, new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "HolidayDates",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "HolidayCalendarId", "HolidayDate", "HolidayTypeId", "IsActive", "IsRecurring", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0d0a1d00-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, true, "New Year's Day", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000002"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Fitr (Day 1)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000003"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Fitr (Day 2)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000004"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Fitr (Day 3)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000005"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Arafat Day", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000006"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Adha (Day 1)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000007"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Adha (Day 2)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000008"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Eid Al Adha (Day 3)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000009"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Islamic New Year", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000010"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "Prophet Muhammad's Birthday", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000011"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, true, "Commemoration Day", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000012"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, true, "UAE National Day (Day 1)", null, null },
                    { new Guid("0d0a1d00-0000-4000-8000-000000000013"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new Guid("0c0a1d00-0000-4000-8000-000000002026"), new DateTime(2026, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, true, "UAE National Day (Day 2)", null, null }
                });

            migrationBuilder.InsertData(
                table: "CostCenters",
                columns: new[] { "Id", "BranchId", "CompanyId", "CostCenterCode", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "OwnerEmployeeId", "ParentCostCenterId", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("0cc00001-0000-4000-8000-000000000001"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "CC-0001", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Executive office, HR and Finance", true, "Corporate & Administration", null, null, null, null });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "BranchId", "CompanyId", "CreatedBy", "CreatedDate", "DepartmentCode", "Description", "HeadEmployeeId", "IsActive", "Name", "ParentDepartmentId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1f8fd7ee-c9c4-484b-aeb9-3f81f2c04609"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "ENG", "Software engineering and development", null, true, "Engineering", null, null, null },
                    { new Guid("28260da1-0bb1-4842-a3ec-786f859dc5ca"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "ADMIN", "Administrative and executive office", null, true, "Administration", null, null, null },
                    { new Guid("2b3c7610-f979-48f0-881c-44b9f3c93555"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "HR", "People operations and talent management", null, true, "Human Resources", null, null, null },
                    { new Guid("5232ea50-7b93-4559-8eb6-dc5f3bd78a09"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "OPS", "Day-to-day business operations", null, true, "Operations", null, null, null },
                    { new Guid("729197ee-0a7e-4910-81a1-be9060a51ae7"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "FIN", "Accounting, payroll and treasury", null, true, "Finance", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "CostCenters",
                columns: new[] { "Id", "BranchId", "CompanyId", "CostCenterCode", "CreatedBy", "CreatedDate", "Description", "IsActive", "Name", "OwnerEmployeeId", "ParentCostCenterId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0cc00001-0000-4000-8000-000000000002"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "CC-0002", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Product and software engineering", true, "Engineering", null, new Guid("0cc00001-0000-4000-8000-000000000001"), null, null },
                    { new Guid("0cc00001-0000-4000-8000-000000000003"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), "CC-0003", "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Business operations", true, "Operations", null, new Guid("0cc00001-0000-4000-8000-000000000001"), null, null }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "BranchId", "BudgetedHeadcount", "CompanyId", "CostCenterId", "CreatedBy", "CreatedDate", "DepartmentId", "Description", "DesignationId", "EffectiveFrom", "EffectiveTo", "EmploymentType", "FilledHeadcount", "Grade", "IsActive", "JobGradeId", "MaxSalary", "MinSalary", "PositionCode", "ReportsToPositionId", "SalaryCurrency", "StatusId", "Title", "UpdatedBy", "UpdatedDate", "WorkLocation" },
                values: new object[,]
                {
                    { new Guid("0b050000-0000-4000-8000-000000000001"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 1, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("28260da1-0bb1-4842-a3ec-786f859dc5ca"), null, new Guid("4e2e6864-b92c-4224-8bef-20f7e72df648"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000001"), null, null, "POS-0001", null, "AED", 2, "Chief Executive Officer", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000002"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 1, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("2b3c7610-f979-48f0-881c-44b9f3c93555"), null, new Guid("60bbb9db-3a9e-4560-bb0f-c93dbaf9c7db"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000003"), null, null, "POS-0002", new Guid("0b050000-0000-4000-8000-000000000001"), "AED", 2, "HR Manager", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000003"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 1, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000002"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("1f8fd7ee-c9c4-484b-aeb9-3f81f2c04609"), null, new Guid("e619d48f-bd89-404f-a406-f466f1b62088"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000003"), null, null, "POS-0003", new Guid("0b050000-0000-4000-8000-000000000001"), "AED", 2, "Engineering Manager", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000006"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 2, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("729197ee-0a7e-4910-81a1-be9060a51ae7"), null, new Guid("82b5f6d0-5403-48d0-8d1c-8ef510762631"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000005"), null, null, "POS-0006", new Guid("0b050000-0000-4000-8000-000000000001"), "AED", 2, "Accountant", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000004"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 3, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000002"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("1f8fd7ee-c9c4-484b-aeb9-3f81f2c04609"), null, new Guid("51da7cd0-7d22-4c3b-a01e-21e1eafd27e9"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000004"), null, null, "POS-0004", new Guid("0b050000-0000-4000-8000-000000000003"), "AED", 2, "Senior Software Engineer", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000005"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 5, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000002"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("1f8fd7ee-c9c4-484b-aeb9-3f81f2c04609"), null, new Guid("368c0e9b-e60d-40cf-912c-43744db1b19f"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000005"), null, null, "POS-0005", new Guid("0b050000-0000-4000-8000-000000000003"), "AED", 2, "Software Engineer", null, null, "Dubai" },
                    { new Guid("0b050000-0000-4000-8000-000000000007"), new Guid("7aa06b50-b394-41b9-b17b-a96cd0e18b8b"), 1, new Guid("eeb27f7f-f7db-479b-923f-3be0ad7a243a"), new Guid("0cc00001-0000-4000-8000-000000000001"), "C047D662-9F0E-4358-B323-15EC3081312C", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("2b3c7610-f979-48f0-881c-44b9f3c93555"), null, new Guid("f2597dae-b3c6-47bb-b2a1-5caa5dcfd770"), new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, 1, 0, null, true, new Guid("06ad0001-0000-4000-8000-000000000006"), null, null, "POS-0007", new Guid("0b050000-0000-4000-8000-000000000002"), "AED", 2, "HR Intern", null, null, "Dubai" }
                });

            migrationBuilder.CreateIndex(
                name: "UX_AppraisalCycles_Code",
                table: "AppraisalCycles",
                column: "CycleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reg_Emp_Date",
                table: "AttendanceRegularizationRequests",
                columns: new[] { "EmployeeId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "UX_Reg_Code",
                table: "AttendanceRegularizationRequests",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Attendance_Employee_Date",
                table: "Attendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_BloodGroups_Code",
                table: "BloodGroups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyId",
                table: "Branches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_HolidayCalendarId",
                table: "Branches",
                column: "HolidayCalendarId");

            migrationBuilder.CreateIndex(
                name: "UX_Branches_Code",
                table: "Branches",
                column: "BranchCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_Email",
                table: "Candidates",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "UX_Candidates_Code",
                table: "Candidates",
                column: "CandidateCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Companies_Code",
                table: "Companies",
                column: "CompanyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Competencies_Code",
                table: "Competencies",
                column: "CompetencyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_BranchId",
                table: "CostCenters",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_CompanyId",
                table: "CostCenters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_ParentCostCenterId",
                table: "CostCenters",
                column: "ParentCostCenterId");

            migrationBuilder.CreateIndex(
                name: "UX_CostCenters_Code",
                table: "CostCenters",
                column: "CostCenterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BranchId",
                table: "Departments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "UX_Departments_Code",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Designations_Name",
                table: "Designations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAddresses_EmployeeId",
                table: "EmployeeAddresses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBanks_EmployeeId",
                table: "EmployeeBanks",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCertifications_EmployeeId",
                table: "EmployeeCertifications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeId",
                table: "EmployeeDocuments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEducations_EmployeeId",
                table: "EmployeeEducations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEmergencyContacts_EmployeeId",
                table: "EmployeeEmergencyContacts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeExperiences_EmployeeId",
                table: "EmployeeExperiences",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchId",
                table: "Employees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CostCenterId",
                table: "Employees",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DesignationId",
                table: "Employees",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobGradeId",
                table: "Employees",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ReportingManagerId",
                table: "Employees",
                column: "ReportingManagerId");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Employees_OfficialEmail",
                table: "Employees",
                column: "OfficialEmail",
                unique: true,
                filter: "[OfficialEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_EmployeeId",
                table: "EmployeeSkills",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTrainings_EmployeeId",
                table: "EmployeeTrainings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_EmploymentStatuses_Code",
                table: "EmploymentStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_EmploymentTypes_Code",
                table: "EmploymentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExitInterviews_SeparationId",
                table: "ExitInterviews",
                column: "SeparationId");

            migrationBuilder.CreateIndex(
                name: "UX_Genders_Code",
                table: "Genders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_Cycle_Emp",
                table: "Goals",
                columns: new[] { "AppraisalCycleId", "EmployeeId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_EmployeeId",
                table: "Goals",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_Goals_Code",
                table: "Goals",
                column: "GoalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_HolidayCalendars_Code",
                table: "HolidayCalendars",
                column: "CalendarCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HolidayDates_Calendar_Date",
                table: "HolidayDates",
                columns: new[] { "HolidayCalendarId", "HolidayDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayDates_HolidayTypeId",
                table: "HolidayDates",
                column: "HolidayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutbox_Unprocessed",
                table: "IntegrationOutbox",
                columns: new[] { "ProcessedDate", "OccurredOn" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPanelMembers_InterviewId",
                table: "InterviewPanelMembers",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CandidateId",
                table: "JobApplications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobPostingId",
                table: "JobApplications",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "UX_JobApplications_Code",
                table: "JobApplications",
                column: "ApplicationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_JobGrades_Code",
                table: "JobGrades",
                column: "GradeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobOffers_CandidateId",
                table: "JobOffers",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffers_JobApplicationId",
                table: "JobOffers",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "UX_JobOffers_Code",
                table: "JobOffers",
                column: "OfferCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_ManpowerRequisitionId",
                table: "JobPostings",
                column: "ManpowerRequisitionId");

            migrationBuilder.CreateIndex(
                name: "UX_JobPostings_Code",
                table: "JobPostings",
                column: "PostingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_LeaveTypeId",
                table: "LeaveApplications",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApps_Emp_Year",
                table: "LeaveApplications",
                columns: new[] { "EmployeeId", "Year", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "UX_LeaveApplications_Code",
                table: "LeaveApplications",
                column: "ApplicationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_LeaveTypeId",
                table: "LeaveBalances",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_LeaveBalances_Emp_Type_Year",
                table: "LeaveBalances",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicies_LeaveTypeId",
                table: "LeavePolicies",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_LeavePolicies_Code",
                table: "LeavePolicies",
                column: "PolicyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTxn_Emp_Type_Year",
                table: "LeaveTransactions",
                columns: new[] { "EmployeeId", "LeaveTypeId", "Year" });

            migrationBuilder.CreateIndex(
                name: "UX_LeaveTypes_Code",
                table: "LeaveTypes",
                column: "LeaveTypeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_Emp",
                table: "Loans",
                columns: new[] { "EmployeeId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "UX_Loans_Code",
                table: "Loans",
                column: "LoanCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManpowerRequisitions_PositionId",
                table: "ManpowerRequisitions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "UX_ManpowerRequisitions_Code",
                table: "ManpowerRequisitions",
                column: "RequisitionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_MaritalStatuses_Code",
                table: "MaritalStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Onboardings_Code",
                table: "Onboardings",
                column: "OnboardingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Onboardings_Offer",
                table: "Onboardings",
                column: "JobOfferId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingTasks_OnboardingId",
                table: "OnboardingTasks",
                column: "OnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_Overtime_Emp_Date",
                table: "OvertimeRequests",
                columns: new[] { "EmployeeId", "OvertimeDate" });

            migrationBuilder.CreateIndex(
                name: "UX_Overtime_Code",
                table: "OvertimeRequests",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_PayComponents_Code",
                table: "PayComponents",
                column: "ComponentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_PayrollRuns_Code",
                table: "PayrollRuns",
                column: "RunCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayslipLines_PayslipId",
                table: "PayslipLines",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_Emp",
                table: "Payslips",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_Run",
                table: "Payslips",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_BranchId",
                table: "Positions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CostCenterId",
                table: "Positions",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DepartmentId",
                table: "Positions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_DesignationId",
                table: "Positions",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_JobGradeId",
                table: "Positions",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ReportsToPositionId",
                table: "Positions",
                column: "ReportsToPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_StatusId",
                table: "Positions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UX_Positions_Code",
                table: "Positions",
                column: "PositionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRatings_Review",
                table: "ReviewRatings",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AppraisalCycleId",
                table: "Reviews",
                column: "AppraisalCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EmployeeId",
                table: "Reviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_Reviews_Code",
                table: "Reviews",
                column: "ReviewCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructureLines_PayComponentId",
                table: "SalaryStructureLines",
                column: "PayComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructureLines_SalaryStructureId",
                table: "SalaryStructureLines",
                column: "SalaryStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryStructures_Emp",
                table: "SalaryStructures",
                columns: new[] { "EmployeeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "UX_SalaryStructures_Code",
                table: "SalaryStructures",
                column: "StructureCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Clearances_Sep_Dept",
                table: "SeparationClearances",
                columns: new[] { "SeparationId", "DepartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Separations_EmployeeId",
                table: "Separations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_Separations_Code",
                table: "Separations",
                column: "SeparationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRosters_Emp_Eff",
                table: "ShiftRosters",
                columns: new[] { "EmployeeId", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRosters_ShiftId",
                table: "ShiftRosters",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "UX_Shifts_Code",
                table: "Shifts",
                column: "ShiftCode",
                unique: true);

            // SQL objects are applied by the newest migration (SqlScriptHelper.ApplyAll), see AddEmployeePhoto.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SQL objects go with their tables (triggers) or are left behind harmlessly; Down is for development only.
            migrationBuilder.DropTable(
                name: "ApplicationStages");

            migrationBuilder.DropTable(
                name: "AppraisalCyclesLog");

            migrationBuilder.DropTable(
                name: "AppraisalCycleStatuses");

            migrationBuilder.DropTable(
                name: "AttendanceRegularizationRequests");

            migrationBuilder.DropTable(
                name: "AttendanceRegularizationRequestsLog");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "AttendanceUploadLogs");

            migrationBuilder.DropTable(
                name: "BloodGroups");

            migrationBuilder.DropTable(
                name: "BranchesLog");

            migrationBuilder.DropTable(
                name: "CandidatesLog");

            migrationBuilder.DropTable(
                name: "ClearanceDepartments");

            migrationBuilder.DropTable(
                name: "ClearanceStatuses");

            migrationBuilder.DropTable(
                name: "CompaniesLog");

            migrationBuilder.DropTable(
                name: "Competencies");

            migrationBuilder.DropTable(
                name: "CompetenciesLog");

            migrationBuilder.DropTable(
                name: "CostCentersLog");

            migrationBuilder.DropTable(
                name: "DepartmentsLog");

            migrationBuilder.DropTable(
                name: "DesignationsLog");

            migrationBuilder.DropTable(
                name: "EmployeeAddresses");

            migrationBuilder.DropTable(
                name: "EmployeeBanks");

            migrationBuilder.DropTable(
                name: "EmployeeCertifications");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "EmployeeEducations");

            migrationBuilder.DropTable(
                name: "EmployeeEmergencyContacts");

            migrationBuilder.DropTable(
                name: "EmployeeExperiences");

            migrationBuilder.DropTable(
                name: "EmployeeSkills");

            migrationBuilder.DropTable(
                name: "EmployeesLog");

            migrationBuilder.DropTable(
                name: "EmployeeTrainings");

            migrationBuilder.DropTable(
                name: "EmploymentStatuses");

            migrationBuilder.DropTable(
                name: "EmploymentTypes");

            migrationBuilder.DropTable(
                name: "ExitInterviews");

            migrationBuilder.DropTable(
                name: "ExitInterviewsLog");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "GoalsLog");

            migrationBuilder.DropTable(
                name: "GoalStatuses");

            migrationBuilder.DropTable(
                name: "HolidayCalendarsLog");

            migrationBuilder.DropTable(
                name: "HolidayDates");

            migrationBuilder.DropTable(
                name: "HRSettings");

            migrationBuilder.DropTable(
                name: "IntegrationOutbox");

            migrationBuilder.DropTable(
                name: "InterviewModes");

            migrationBuilder.DropTable(
                name: "InterviewPanelMembers");

            migrationBuilder.DropTable(
                name: "JobApplicationsLog");

            migrationBuilder.DropTable(
                name: "JobGradesLog");

            migrationBuilder.DropTable(
                name: "JobOffers");

            migrationBuilder.DropTable(
                name: "JobOffersLog");

            migrationBuilder.DropTable(
                name: "JobPostingsLog");

            migrationBuilder.DropTable(
                name: "JobPostingStatuses");

            migrationBuilder.DropTable(
                name: "LeaveApplications");

            migrationBuilder.DropTable(
                name: "LeaveApplicationsLog");

            migrationBuilder.DropTable(
                name: "LeaveApplicationStatuses");

            migrationBuilder.DropTable(
                name: "LeaveBalances");

            migrationBuilder.DropTable(
                name: "LeavePolicies");

            migrationBuilder.DropTable(
                name: "LeavePoliciesLog");

            migrationBuilder.DropTable(
                name: "LeaveTransactions");

            migrationBuilder.DropTable(
                name: "LeaveTransactionTypes");

            migrationBuilder.DropTable(
                name: "LeaveTypesLog");

            migrationBuilder.DropTable(
                name: "LoanInstallments");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "LoansLog");

            migrationBuilder.DropTable(
                name: "LoanStatuses");

            migrationBuilder.DropTable(
                name: "ManpowerRequisitionsLog");

            migrationBuilder.DropTable(
                name: "MaritalStatuses");

            migrationBuilder.DropTable(
                name: "OfferStatuses");

            migrationBuilder.DropTable(
                name: "OnboardingsLog");

            migrationBuilder.DropTable(
                name: "OnboardingStatuses");

            migrationBuilder.DropTable(
                name: "OnboardingTasks");

            migrationBuilder.DropTable(
                name: "OnboardingTaskStatuses");

            migrationBuilder.DropTable(
                name: "OvertimeRequests");

            migrationBuilder.DropTable(
                name: "OvertimeRequestsLog");

            migrationBuilder.DropTable(
                name: "OvertimeStatuses");

            migrationBuilder.DropTable(
                name: "PayComponentsLog");

            migrationBuilder.DropTable(
                name: "PayComponentTypes");

            migrationBuilder.DropTable(
                name: "PayrollRuns");

            migrationBuilder.DropTable(
                name: "PayrollRunsLog");

            migrationBuilder.DropTable(
                name: "PayrollRunStatuses");

            migrationBuilder.DropTable(
                name: "PayslipLines");

            migrationBuilder.DropTable(
                name: "PositionsLog");

            migrationBuilder.DropTable(
                name: "RatingItemTypes");

            migrationBuilder.DropTable(
                name: "RegularizationStatuses");

            migrationBuilder.DropTable(
                name: "RegularizationTypes");

            migrationBuilder.DropTable(
                name: "RequisitionStatuses");

            migrationBuilder.DropTable(
                name: "ReviewRatings");

            migrationBuilder.DropTable(
                name: "ReviewsLog");

            migrationBuilder.DropTable(
                name: "ReviewStatuses");

            migrationBuilder.DropTable(
                name: "SalaryStructureLines");

            migrationBuilder.DropTable(
                name: "SalaryStructuresLog");

            migrationBuilder.DropTable(
                name: "SeparationClearances");

            migrationBuilder.DropTable(
                name: "SeparationClearancesLog");

            migrationBuilder.DropTable(
                name: "SeparationsLog");

            migrationBuilder.DropTable(
                name: "SeparationStatuses");

            migrationBuilder.DropTable(
                name: "SeparationTypes");

            migrationBuilder.DropTable(
                name: "ShiftRosters");

            migrationBuilder.DropTable(
                name: "ShiftRostersLog");

            migrationBuilder.DropTable(
                name: "ShiftsLog");

            migrationBuilder.DropTable(
                name: "HolidayTypes");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "LeaveTypes");

            migrationBuilder.DropTable(
                name: "Onboardings");

            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "PayComponents");

            migrationBuilder.DropTable(
                name: "SalaryStructures");

            migrationBuilder.DropTable(
                name: "Separations");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DropTable(
                name: "AppraisalCycles");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ManpowerRequisitions");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "JobGrades");

            migrationBuilder.DropTable(
                name: "PositionStatuses");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "HolidayCalendars");
        }
    }
}
