using Microsoft.EntityFrameworkCore;
using SBERP.HumanResources.Models.Base;

namespace SBERP.HumanResources.Persistence
{
    /// <summary>
    /// EF Core context for HumanResourcesDB.
    ///
    /// <para>Menu and role-permission concerns are NOT owned by this service —
    /// SBERP.Security holds <c>AppUserMenus</c> and <c>AppUserRoleMenus</c>,
    /// and the frontend asks Security for the navigation for any role. HR
    /// routes simply appear as menu rows inside SecurityDB.</para>
    /// </summary>
    public class HumanResourcesDBContext : DbContext
    {
        public HumanResourcesDBContext() { }

        public HumanResourcesDBContext(DbContextOptions<HumanResourcesDBContext> options)
            : base(options) { }

        // === Org structure ===
        public virtual DbSet<Company>? Companies { get; set; }
        public virtual DbSet<CompanyLog>? CompaniesLog { get; set; }
        public virtual DbSet<Branch>? Branches { get; set; }
        public virtual DbSet<BranchLog>? BranchesLog { get; set; }
        public virtual DbSet<Department>?     Departments     { get; set; }
        public virtual DbSet<DepartmentLog>?  DepartmentsLog  { get; set; }
        public virtual DbSet<Designation>?    Designations    { get; set; }
        public virtual DbSet<DesignationLog>? DesignationsLog { get; set; }

        // === Employee + profile ===
        public virtual DbSet<Employee>?    Employees    { get; set; }
        public virtual DbSet<EmployeeLog>? EmployeesLog { get; set; }
        public virtual DbSet<EmployeeAddress>?          EmployeeAddresses         { get; set; }
        public virtual DbSet<EmployeeEducation>?        EmployeeEducations        { get; set; }
        public virtual DbSet<EmployeeExperience>?       EmployeeExperiences       { get; set; }
        public virtual DbSet<EmployeeSkill>?            EmployeeSkills            { get; set; }
        public virtual DbSet<EmployeeTraining>?         EmployeeTrainings         { get; set; }
        public virtual DbSet<EmployeeCertification>?    EmployeeCertifications    { get; set; }
        public virtual DbSet<EmployeeDocument>?         EmployeeDocuments         { get; set; }
        public virtual DbSet<EmployeeEmergencyContact>? EmployeeEmergencyContacts { get; set; }
        public virtual DbSet<EmployeeBank>?             EmployeeBanks             { get; set; }

        // === Settings ===
        public virtual DbSet<HRSettings>? HRSettings { get; set; }

        // === Employee reference / lookup tables ===
        // Ids mirror the C# enums in SBERP.HumanResources.Enum — keep in sync.
        public virtual DbSet<GenderLookup>? Genders { get; set; }
        public virtual DbSet<MaritalStatusLookup>? MaritalStatuses { get; set; }
        public virtual DbSet<BloodGroupLookup>? BloodGroups { get; set; }
        public virtual DbSet<EmploymentTypeLookup>? EmploymentTypes { get; set; }
        public virtual DbSet<EmploymentStatusLookup>? EmploymentStatuses { get; set; }

        // === Attendance ===
        public virtual DbSet<Attendance>?          Attendances          { get; set; }
        public virtual DbSet<AttendanceUploadLog>? AttendanceUploadLogs { get; set; }

        /// <summary>
        /// Money / hours / scores default to decimal(18,2) unless a slice maps a
        /// column explicitly (same store type EF used before, minus the warnings).
        /// </summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder cb)
        {
            cb.Properties<decimal>().HavePrecision(18, 2);
            cb.Properties<decimal?>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // ------------------------------------------------------------
            // Company
            // ------------------------------------------------------------
            mb.Entity<Company>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CompanyCode).HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.LegalName).HasMaxLength(300);
                e.Property(x => x.RegistrationNumber).HasMaxLength(100);
                e.Property(x => x.TaxNumber).HasMaxLength(100);
                e.Property(x => x.Address).HasMaxLength(500);
                e.Property(x => x.City).HasMaxLength(100);
                e.Property(x => x.Country).HasMaxLength(100);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.Property(x => x.Email).HasMaxLength(200);
                e.Property(x => x.Website).HasMaxLength(200);
                e.Property(x => x.CurrencyCode).HasMaxLength(10);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.CompanyCode).IsUnique().HasDatabaseName("UX_Companies_Code");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertCompanies");
                    t.HasTrigger("TRG_UpdateCompanies");
                    t.HasTrigger("TRG_DeleteCompanies");
                });
            });

            mb.Entity<CompanyLog>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            // ------------------------------------------------------------
            // Branch
            // ------------------------------------------------------------
            mb.Entity<Branch>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.BranchCode).HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Address).HasMaxLength(500);
                e.Property(x => x.City).HasMaxLength(100);
                e.Property(x => x.Country).HasMaxLength(100);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.Property(x => x.Email).HasMaxLength(200);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.BranchCode).IsUnique().HasDatabaseName("UX_Branches_Code");
                e.HasOne(x => x.Company)
                    .WithMany(c => c!.Branches)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branches_Company");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertBranches");
                    t.HasTrigger("TRG_UpdateBranches");
                    t.HasTrigger("TRG_DeleteBranches");
                });
            });

            mb.Entity<BranchLog>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            // ------------------------------------------------------------
            // Department
            // ------------------------------------------------------------
            mb.Entity<Department>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.DepartmentCode).HasMaxLength(20).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.DepartmentCode).IsUnique().HasDatabaseName("UX_Departments_Code");
                e.HasOne(x => x.Company)
                    .WithMany(c => c!.Departments)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Departments_Company");
                e.HasOne(x => x.Branch)
                    .WithMany(b => b!.Departments)
                    .HasForeignKey(x => x.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Departments_Branch");
                e.HasOne(x => x.ParentDepartment)
                    .WithMany(p => p!.ChildDepartments)
                    .HasForeignKey(x => x.ParentDepartmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Departments_Parent");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertDepartments");
                    t.HasTrigger("TRG_UpdateDepartments");
                    t.HasTrigger("TRG_DeleteDepartments");
                });
            });

            mb.Entity<DepartmentLog>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            // ------------------------------------------------------------
            // Designation
            // ------------------------------------------------------------
            mb.Entity<Designation>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Code).HasMaxLength(20);
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Designations_Name");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertDesignations");
                    t.HasTrigger("TRG_UpdateDesignations");
                    t.HasTrigger("TRG_DeleteDesignations");
                });
            });

            mb.Entity<DesignationLog>(e => e.HasKey(x => x.Id));

            // ------------------------------------------------------------
            // Employee (root)
            // ------------------------------------------------------------
            mb.Entity<Employee>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.EmployeeCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.OfficialEmail).HasMaxLength(200);
                e.Property(x => x.PersonalEmail).HasMaxLength(200);
                e.Property(x => x.FirstName).HasMaxLength(100);
                e.Property(x => x.MiddleName).HasMaxLength(100);
                e.Property(x => x.LastName).HasMaxLength(100);
                e.Property(x => x.FullName).HasMaxLength(300);
                e.Property(x => x.Nationality).HasMaxLength(100);
                e.Property(x => x.Religion).HasMaxLength(100);
                e.Property(x => x.NationalId).HasMaxLength(100);
                e.Property(x => x.PassportNumber).HasMaxLength(50);
                e.Property(x => x.MobileNumber).HasMaxLength(30);
                e.Property(x => x.AlternatePhoneNumber).HasMaxLength(30);
                e.Property(x => x.WorkLocation).HasMaxLength(200);
                e.Property(x => x.SalaryCurrency).HasMaxLength(10);
                e.Property(x => x.PhotoUrl).HasMaxLength(500);
                e.Property(x => x.PhotoLocation).HasMaxLength(500);
                e.Property(x => x.SignatureUrl).HasMaxLength(500);
                e.Property(x => x.DateOfBirth).HasColumnType("datetime");
                e.Property(x => x.PassportExpiryDate).HasColumnType("datetime");
                e.Property(x => x.DateOfJoining).HasColumnType("datetime");
                e.Property(x => x.ProbationEndDate).HasColumnType("datetime");
                e.Property(x => x.ConfirmationDate).HasColumnType("datetime");
                e.Property(x => x.DateOfLeaving).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");

                e.HasIndex(x => x.EmployeeCode).IsUnique().HasDatabaseName("UX_Employees_EmployeeCode");
                e.HasIndex(x => x.OfficialEmail).IsUnique()
                    .HasFilter("[OfficialEmail] IS NOT NULL")
                    .HasDatabaseName("UX_Employees_OfficialEmail");

                e.HasOne(x => x.Company)
                    .WithMany(c => c!.Employees)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employees_Company");

                e.HasOne(x => x.Branch)
                    .WithMany(b => b!.Employees)
                    .HasForeignKey(x => x.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employees_Branch");

                e.HasOne(x => x.Department)
                    .WithMany(d => d!.Employees)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employees_Department");

                e.HasOne(x => x.Designation)
                    .WithMany(d => d!.Employees)
                    .HasForeignKey(x => x.DesignationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employees_Designation");

                e.HasOne(x => x.ReportingManager)
                    .WithMany(m => m!.DirectReports)
                    .HasForeignKey(x => x.ReportingManagerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employees_ReportingManager");

                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertEmployees");
                    t.HasTrigger("TRG_UpdateEmployees");
                    t.HasTrigger("TRG_DeleteEmployees");
                });
            });

            mb.Entity<EmployeeLog>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.PhotoLocation).HasMaxLength(500);
            });

            // ------------------------------------------------------------
            // Employee sub-entities — cascade delete with the parent
            // ------------------------------------------------------------
            mb.Entity<EmployeeAddress>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Addresses)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeEducation>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Educations)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeExperience>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Experiences)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeSkill>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Skills)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeTraining>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Trainings)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeCertification>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Certifications)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeDocument>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.Documents)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeEmergencyContact>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany(p => p!.EmergencyContacts)
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            });
            mb.Entity<EmployeeBank>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.EmployeeId).IsUnique();   // 1-to-1
                e.HasOne(x => x.Employee).WithOne(p => p!.BankInfo)
                    .HasForeignKey<EmployeeBank>(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ------------------------------------------------------------
            // HRSettings — single row controlling attendance source etc.
            // ------------------------------------------------------------
            mb.Entity<HRSettings>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.BiometricConnectionString).HasMaxLength(1000);
                e.Property(x => x.BiometricSourceObject).HasMaxLength(200);
                e.Property(x => x.OfficeStartTime).HasMaxLength(10);
                e.Property(x => x.OfficeEndTime).HasMaxLength(10);
                e.Property(x => x.AutoProcessTime).HasMaxLength(10);
                e.Property(x => x.WeeklyOffDays).HasMaxLength(30);
                e.Property(x => x.TimeZoneId).HasMaxLength(64);
                e.Property(x => x.AttendanceDayCutoffTime).HasMaxLength(10);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            // ------------------------------------------------------------
            // Attendance
            // ------------------------------------------------------------
            mb.Entity<Attendance>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.AttendanceDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.HasIndex(x => new { x.EmployeeId, x.AttendanceDate })
                    .IsUnique()
                    .HasDatabaseName("UX_Attendance_Employee_Date");
                e.HasOne(x => x.Employee).WithMany()
                    .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<AttendanceUploadLog>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FileName).HasMaxLength(255);
                e.Property(x => x.FilePath).HasMaxLength(500);
                e.Property(x => x.UploadedDate).HasColumnType("datetime");
                e.Property(x => x.RolledBackDate).HasColumnType("datetime");
            });

            // ------------------------------------------------------------
            // Lookup tables — identical shape. No audit triggers: reference
            // data, not transactional. Id is assigned (ValueGeneratedNever)
            // so it matches the C# enum values exactly.
            // ------------------------------------------------------------
            mb.Entity<GenderLookup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Genders_Code");
            });
            mb.Entity<MaritalStatusLookup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_MaritalStatuses_Code");
            });
            mb.Entity<BloodGroupLookup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_BloodGroups_Code");
            });
            mb.Entity<EmploymentTypeLookup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_EmploymentTypes_Code");
            });
            mb.Entity<EmploymentStatusLookup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_EmploymentStatuses_Code");
            });

            // ============================================================
            // SEED DATA — deterministic
            // Menu seeds live in SBERP.Security, not here.
            // ============================================================
            var seedTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var seedCompanyId = new Guid("EEB27F7F-F7DB-479B-923F-3BE0AD7A243A");
            var seedBranchId = new Guid("7AA06B50-B394-41B9-B17B-A96CD0E18B8B");
            const string seedCreator = "C047D662-9F0E-4358-B323-15EC3081312C";

            mb.Entity<Company>().HasData(new Company
            {
                Id = seedCompanyId,
                CompanyCode = "SBERP",
                Name = "SBERP Default Company",
                LegalName = "SBERP Default Company LLC",
                CurrencyCode = "AED",
                FinancialYearStartMonth = 1,
                CreatedBy = "SYSTEM",
                CreatedDate = seedTime,
                UpdatedDate = seedTime,
                IsActive = true
            });

            mb.Entity<Branch>().HasData(new Branch
            {
                Id = seedBranchId,
                CompanyId = seedCompanyId,
                BranchCode = "HO",
                Name = "Head Office",
                IsHeadOffice = true,
                TimeZone = "Asia/Dubai",
                HolidayCalendarId = SeedCalendarId,
                CreatedBy = "SYSTEM",
                CreatedDate = seedTime,
                UpdatedDate = seedTime,
                IsActive = true
            });

            // --- Departments ---
            // CompanyId/BranchId REQUIRED here — Department.CompanyId is a non-nullable
            // Guid with a real FK to Companies. Without it, EF writes Guid.Empty for every
            // row and Update-Database fails on the FK_Departments_Company constraint the
            // moment it tries to apply this seed.
            mb.Entity<Department>().HasData(
                new Department { Id = new Guid("28260DA1-0BB1-4842-A3EC-786F859DC5CA"), DepartmentCode = "ADMIN", Name = "Administration", Description = "Administrative and executive office", CompanyId = seedCompanyId, BranchId = seedBranchId, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("2B3C7610-F979-48F0-881C-44B9F3C93555"), DepartmentCode = "HR", Name = "Human Resources", Description = "People operations and talent management", CompanyId = seedCompanyId, BranchId = seedBranchId, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("1F8FD7EE-C9C4-484B-AEB9-3F81F2C04609"), DepartmentCode = "ENG", Name = "Engineering", Description = "Software engineering and development", CompanyId = seedCompanyId, BranchId = seedBranchId, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("729197EE-0A7E-4910-81A1-BE9060A51AE7"), DepartmentCode = "FIN", Name = "Finance", Description = "Accounting, payroll and treasury", CompanyId = seedCompanyId, BranchId = seedBranchId, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("5232EA50-7B93-4559-8EB6-DC5F3BD78A09"), DepartmentCode = "OPS", Name = "Operations", Description = "Day-to-day business operations", CompanyId = seedCompanyId, BranchId = seedBranchId, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true }
            );

            // --- Designations ---
            mb.Entity<Designation>().HasData(
                new Designation { Id = new Guid("4E2E6864-B92C-4224-8BEF-20F7E72DF648"), Name = "Chief Executive Officer", Code = "CEO", Grade = 1, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("E619D48F-BD89-404F-A406-F466F1B62088"), Name = "Manager",                  Code = "MGR", Grade = 4, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("F46D487D-F446-4E3C-B06F-8263DC854C89"), Name = "Team Lead",                Code = "TL",  Grade = 5, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("51DA7CD0-7D22-4C3B-A01E-21E1EAFD27E9"), Name = "Senior Engineer",          Code = "SE",  Grade = 6, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("368C0E9B-E60D-40CF-912C-43744DB1B19F"), Name = "Software Engineer",        Code = "SWE", Grade = 7, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("60BBB9DB-3A9E-4560-BB0F-C93DBAF9C7DB"), Name = "HR Manager",               Code = "HRM", Grade = 4, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("82B5F6D0-5403-48D0-8D1C-8EF510762631"), Name = "Accountant",               Code = "ACC", Grade = 6, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("F2597DAE-B3C6-47BB-B2A1-5CAA5DCFD770"), Name = "Intern",                   Code = "INT", Grade = 9, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true }
            );

            // --- HRSettings (single row) ---
            mb.Entity<HRSettings>().HasData(
                new HRSettings
                {
                    Id = new Guid("FEED0001-0000-4000-8000-000000000001"),
                    AttendanceSource = 1,          // ManualExcelUpload
                    BiometricProvider = 0,
                    OfficeStartTime = "08:30",
                    OfficeEndTime = "17:30",
                    GracePeriodMinutes = 15,
                    WeeklyOffDays = "5,6",         // Fri-Sat (UAE)
                    AnnualLeaveDays = 22,
                    SickLeaveDays = 10,
                    CasualLeaveDays = 5,
                    AutoProcessTime = "23:30",
                    AutoProcessEnabled = false,
                    TimeZoneId = "Asia/Dubai",
                    AttendanceDayCutoffTime = "04:00",
                    MinMinutesBetweenInOut = 30,
                    DuplicatePunchMinutes = 1,
                    MobileAttendanceEnabled = true,
                    RequireOfficeNetwork = true,
                    CreatedBy = seedCreator,
                    CreatedDate = seedTime,
                    IsActive = true,
                    IsHardDelete = true
                }
            );

            // --- Genders (Id == EnumGender) ---
            mb.Entity<GenderLookup>().HasData(
                new GenderLookup { Id = 1, Code = "MALE", Name = "Male", SortOrder = 1, IsActive = true },
                new GenderLookup { Id = 2, Code = "FEMALE", Name = "Female", SortOrder = 2, IsActive = true },
                new GenderLookup { Id = 3, Code = "OTHER", Name = "Other", SortOrder = 3, IsActive = true }
            );

            // --- Marital statuses (Id == EnumMaritalStatus) ---
            mb.Entity<MaritalStatusLookup>().HasData(
                new MaritalStatusLookup { Id = 1, Code = "SINGLE", Name = "Single", SortOrder = 1, IsActive = true },
                new MaritalStatusLookup { Id = 2, Code = "MARRIED", Name = "Married", SortOrder = 2, IsActive = true },
                new MaritalStatusLookup { Id = 3, Code = "DIVORCED", Name = "Divorced", SortOrder = 3, IsActive = true },
                new MaritalStatusLookup { Id = 4, Code = "WIDOWED", Name = "Widowed", SortOrder = 4, IsActive = true },
                new MaritalStatusLookup { Id = 5, Code = "SEPARATED", Name = "Separated", SortOrder = 5, IsActive = true }
            );

            // --- Blood groups (Id == EnumBloodGroup) ---
            mb.Entity<BloodGroupLookup>().HasData(
                new BloodGroupLookup { Id = 1, Code = "A_POS", Name = "A+", SortOrder = 1, IsActive = true },
                new BloodGroupLookup { Id = 2, Code = "A_NEG", Name = "A-", SortOrder = 2, IsActive = true },
                new BloodGroupLookup { Id = 3, Code = "B_POS", Name = "B+", SortOrder = 3, IsActive = true },
                new BloodGroupLookup { Id = 4, Code = "B_NEG", Name = "B-", SortOrder = 4, IsActive = true },
                new BloodGroupLookup { Id = 5, Code = "AB_POS", Name = "AB+", SortOrder = 5, IsActive = true },
                new BloodGroupLookup { Id = 6, Code = "AB_NEG", Name = "AB-", SortOrder = 6, IsActive = true },
                new BloodGroupLookup { Id = 7, Code = "O_POS", Name = "O+", SortOrder = 7, IsActive = true },
                new BloodGroupLookup { Id = 8, Code = "O_NEG", Name = "O-", SortOrder = 8, IsActive = true }
            );

            // --- Employment types (Id == EnumEmploymentType) ---
            mb.Entity<EmploymentTypeLookup>().HasData(
                new EmploymentTypeLookup { Id = 1, Code = "FULL_TIME", Name = "Full Time", SortOrder = 1, IsActive = true },
                new EmploymentTypeLookup { Id = 2, Code = "PART_TIME", Name = "Part Time", SortOrder = 2, IsActive = true },
                new EmploymentTypeLookup { Id = 3, Code = "CONTRACT", Name = "Contract", SortOrder = 3, IsActive = true },
                new EmploymentTypeLookup { Id = 4, Code = "INTERN", Name = "Intern", SortOrder = 4, IsActive = true },
                new EmploymentTypeLookup { Id = 5, Code = "CONSULTANT", Name = "Consultant", SortOrder = 5, IsActive = true },
                new EmploymentTypeLookup { Id = 6, Code = "PROBATION", Name = "Probation", SortOrder = 6, IsActive = true }
            );

            // --- Employment statuses (Id == EnumEmploymentStatus) ---
            mb.Entity<EmploymentStatusLookup>().HasData(
                new EmploymentStatusLookup { Id = 1, Code = "ACTIVE", Name = "Active", SortOrder = 1, IsActive = true },
                new EmploymentStatusLookup { Id = 2, Code = "ON_LEAVE", Name = "On Leave", SortOrder = 2, IsActive = true },
                new EmploymentStatusLookup { Id = 3, Code = "SUSPENDED", Name = "Suspended", SortOrder = 3, IsActive = true },
                new EmploymentStatusLookup { Id = 4, Code = "RESIGNED", Name = "Resigned", SortOrder = 4, IsActive = true },
                new EmploymentStatusLookup { Id = 5, Code = "TERMINATED", Name = "Terminated", SortOrder = 5, IsActive = true },
                new EmploymentStatusLookup { Id = 6, Code = "RETIRED", Name = "Retired", SortOrder = 6, IsActive = true }
            );

            // ============================================================
            // HR sub-module slices — each lives in its own partial file
            // (HumanResourcesDBContext.<Module>.cs). Order matters only where
            // a later slice references an entity configured by an earlier one.
            // ============================================================
            ConfigureOrgSetup(mb);       // Module 0 — CostCenter / JobGrade / Holiday / Position
            ConfigureRecruitment(mb);    // Module 1
            ConfigureOnboarding(mb);     // Module 2
            ConfigureAttendance(mb);     // Module 4
            ConfigureLeave(mb);          // Module 5
            ConfigurePayroll(mb);        // Module 6
            ConfigurePerformance(mb);    // Module 7
            ConfigureSeparation(mb);     // Module 8
            ConfigureBiometricAttendance(mb);   // Module 4 — mobile app / door terminal punches
            SeedSubModuleData(mb);       // lookups + reference data for Modules 1-8

            // Every lookup table's Id mirrors a C# enum value — never identity.
            foreach (var et in mb.Model.GetEntityTypes()
                         .Where(t => typeof(BaseLookup).IsAssignableFrom(t.ClrType)))
            {
                mb.Entity(et.ClrType).Property(nameof(BaseLookup.Id)).ValueGeneratedNever();
            }
        }


        #region Attendance (was HumanResourcesDBContext.Attendance.cs)
        // Attendance workflow slice of HumanResourcesDBContext. The existing
        // Attendance / AttendanceUploadLog DbSets stay where they are; this adds the
        // shift/roster/overtime/regularization tables. Call ConfigureAttendance(mb)
        // at the end of OnModelCreating.
        public virtual DbSet<OvertimeStatusLookup>?       OvertimeStatuses        { get; set; }
        public virtual DbSet<RegularizationStatusLookup>? RegularizationStatuses  { get; set; }
        public virtual DbSet<RegularizationTypeLookup>?   RegularizationTypes     { get; set; }

        public virtual DbSet<Shift>?       Shifts        { get; set; }
        public virtual DbSet<ShiftLog>?    ShiftsLog     { get; set; }
        public virtual DbSet<ShiftRoster>? ShiftRosters  { get; set; }
        public virtual DbSet<ShiftRosterLog>? ShiftRostersLog { get; set; }

        public virtual DbSet<OvertimeRequest>?    OvertimeRequests    { get; set; }
        public virtual DbSet<OvertimeRequestLog>? OvertimeRequestsLog { get; set; }

        public virtual DbSet<AttendanceRegularizationRequest>?    AttendanceRegularizationRequests    { get; set; }
        public virtual DbSet<AttendanceRegularizationRequestLog>? AttendanceRegularizationRequestsLog { get; set; }

        private void ConfigureAttendance(ModelBuilder mb)
        {
            mb.Entity<OvertimeStatusLookup>(e => { e.ToTable("OvertimeStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<RegularizationStatusLookup>(e => { e.ToTable("RegularizationStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<RegularizationTypeLookup>(e => { e.ToTable("RegularizationTypes"); e.HasKey(x => x.Id); });

            mb.Entity<Shift>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ShiftCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.HasIndex(x => x.ShiftCode).IsUnique().HasDatabaseName("UX_Shifts_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertShifts"); t.HasTrigger("TRG_UpdateShifts"); t.HasTrigger("TRG_DeleteShifts"); });
            });
            mb.Entity<ShiftLog>(e => e.HasKey(x => x.Id));

            mb.Entity<ShiftRoster>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => new { x.EmployeeId, x.EffectiveFrom, x.EffectiveTo }).HasDatabaseName("IX_ShiftRosters_Emp_Eff");
                e.ToTable(t => { t.HasTrigger("TRG_InsertShiftRosters"); t.HasTrigger("TRG_UpdateShiftRosters"); });
            });
            mb.Entity<ShiftRosterLog>(e => e.HasKey(x => x.Id));

            mb.Entity<OvertimeRequest>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.RequestCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.RequestCode).IsUnique().HasDatabaseName("UX_Overtime_Code");
                e.HasIndex(x => new { x.EmployeeId, x.OvertimeDate }).HasDatabaseName("IX_Overtime_Emp_Date");
                e.ToTable(t => { t.HasTrigger("TRG_InsertOvertimeRequests"); t.HasTrigger("TRG_UpdateOvertimeRequests"); });
            });
            mb.Entity<OvertimeRequestLog>(e => e.HasKey(x => x.Id));

            mb.Entity<AttendanceRegularizationRequest>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.RequestCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.RequestCode).IsUnique().HasDatabaseName("UX_Reg_Code");
                e.HasIndex(x => new { x.EmployeeId, x.AttendanceDate }).HasDatabaseName("IX_Reg_Emp_Date");
                e.ToTable(t => { t.HasTrigger("TRG_InsertRegularization"); t.HasTrigger("TRG_UpdateRegularization"); });
            });
            mb.Entity<AttendanceRegularizationRequestLog>(e => e.HasKey(x => x.Id));
        }
        #endregion

        #region BiometricAttendance
        // Mobile-app and door-terminal attendance. Punches arrive in AttendancePunches (raw,
        // append-only); AttendanceProcessor turns the accepted ones into the daily dbo.Attendances
        // row (SourceType 2 = Biometric): earliest punch of the work day = check-in, latest = check-out.
        public virtual DbSet<BiometricTypeLookup>?               BiometricTypes                  { get; set; }
        public virtual DbSet<BiometricRegistrationStatusLookup>? BiometricRegistrationStatuses   { get; set; }
        public virtual DbSet<PunchSourceLookup>?                 PunchSources                    { get; set; }
        public virtual DbSet<PunchStatusLookup>?                 PunchStatuses                   { get; set; }
        public virtual DbSet<AttendanceDevice>?                  AttendanceDevices               { get; set; }
        public virtual DbSet<AttendanceDeviceLog>?               AttendanceDevicesLog            { get; set; }
        public virtual DbSet<AttendanceZone>?                    AttendanceZones                 { get; set; }
        public virtual DbSet<AttendanceZoneLog>?                 AttendanceZonesLog              { get; set; }
        public virtual DbSet<EmployeeBiometricRegistration>?     EmployeeBiometricRegistrations  { get; set; }
        public virtual DbSet<EmployeeBiometricRegistrationLog>?  EmployeeBiometricRegistrationsLog { get; set; }
        public virtual DbSet<AttendancePunch>?                   AttendancePunches               { get; set; }

        /// <summary>Source of the number in EmployeeBiometricRegistrations.RegistrationNo.</summary>
        internal const string BiometricRegistrationSequence = "SEQ_BiometricRegistrationNo";

        private void ConfigureBiometricAttendance(ModelBuilder mb)
        {
            mb.HasSequence<long>(BiometricRegistrationSequence).StartsAt(1).IncrementsBy(1);

            mb.Entity<BiometricTypeLookup>(e => { e.ToTable("BiometricTypes"); e.HasKey(x => x.Id); });
            mb.Entity<BiometricRegistrationStatusLookup>(e => { e.ToTable("BiometricRegistrationStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<PunchSourceLookup>(e => { e.ToTable("PunchSources"); e.HasKey(x => x.Id); });
            mb.Entity<PunchStatusLookup>(e => { e.ToTable("PunchStatuses"); e.HasKey(x => x.Id); });

            mb.Entity<AttendanceDevice>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.DeviceCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Vendor).HasMaxLength(50);
                e.Property(x => x.Model).HasMaxLength(50);
                e.Property(x => x.SerialNumber).HasMaxLength(50).IsRequired();
                e.Property(x => x.Location).HasMaxLength(200);
                e.Property(x => x.IpAddress).HasMaxLength(64);
                e.Property(x => x.ApiKeyHash).HasColumnType("varchar(64)");
                e.Property(x => x.Remarks).HasMaxLength(500);
                e.Property(x => x.LastSeenDate).HasColumnType("datetime");
                e.Property(x => x.LastPunchDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.DeviceCode).IsUnique().HasDatabaseName("UX_AttendanceDevices_Code");
                e.HasIndex(x => x.SerialNumber).IsUnique().HasDatabaseName("UX_AttendanceDevices_Serial");
                e.ToTable(t => { t.HasTrigger("TRG_InsertAttendanceDevices"); t.HasTrigger("TRG_UpdateAttendanceDevices"); });
            });
            mb.Entity<AttendanceDeviceLog>(e => { e.ToTable("AttendanceDevicesLog"); e.HasKey(x => x.Id); });

            mb.Entity<AttendanceZone>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ZoneCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.AllowedIpRanges).HasMaxLength(1000);
                e.Property(x => x.AllowedWifiBssids).HasMaxLength(1000);
                e.Property(x => x.Remarks).HasMaxLength(500);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.ZoneCode).IsUnique().HasDatabaseName("UX_AttendanceZones_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertAttendanceZones"); t.HasTrigger("TRG_UpdateAttendanceZones"); });
            });
            mb.Entity<AttendanceZoneLog>(e => { e.ToTable("AttendanceZonesLog"); e.HasKey(x => x.Id); });

            mb.Entity<EmployeeBiometricRegistration>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.RegistrationNo).HasMaxLength(60).IsRequired();
                e.Property(x => x.DeviceInstallationId).HasMaxLength(100);
                e.Property(x => x.DevicePlatform).HasMaxLength(20);
                e.Property(x => x.DeviceModel).HasMaxLength(100);
                e.Property(x => x.AppVersion).HasMaxLength(20);
                e.Property(x => x.PublicKey).HasMaxLength(1000);
                e.Property(x => x.KeyAlgorithm).HasMaxLength(20);
                e.Property(x => x.DeviceUserPin).HasMaxLength(30);
                e.Property(x => x.TemplateFileName).HasMaxLength(100);
                e.Property(x => x.TemplateFormat).HasMaxLength(30);
                e.Property(x => x.TemplateSha256).HasColumnType("varchar(64)");
                e.Property(x => x.CapturedAngles).HasMaxLength(50);
                e.Property(x => x.RevokedReason).HasMaxLength(200);
                e.Property(x => x.ConsentDate).HasColumnType("datetime");
                e.Property(x => x.VerifiedDate).HasColumnType("datetime");
                e.Property(x => x.LastUsedDate).HasColumnType("datetime");
                e.Property(x => x.RevokedDate).HasColumnType("datetime");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_BiometricRegistrations_Employee");
                e.HasOne<BiometricTypeLookup>().WithMany().HasForeignKey(x => x.BiometricTypeId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_BiometricRegistrations_Type");
                e.HasOne<BiometricRegistrationStatusLookup>().WithMany().HasForeignKey(x => x.StatusId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_BiometricRegistrations_Status");
                e.HasOne<AttendanceDevice>().WithMany().HasForeignKey(x => x.AttendanceDeviceId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_BiometricRegistrations_Device");
                e.HasIndex(x => x.RegistrationNo).IsUnique().HasDatabaseName("UX_BiometricRegistrations_No");
                e.HasIndex(x => new { x.EmployeeId, x.StatusId }).HasDatabaseName("IX_BiometricRegistrations_Emp_Status");
                e.HasIndex(x => new { x.DeviceUserPin, x.StatusId }).HasDatabaseName("IX_BiometricRegistrations_Pin");
                e.ToTable(t => { t.HasTrigger("TRG_InsertEmployeeBiometricRegistrations"); t.HasTrigger("TRG_UpdateEmployeeBiometricRegistrations"); });
            });
            mb.Entity<EmployeeBiometricRegistrationLog>(e => { e.ToTable("EmployeeBiometricRegistrationsLog"); e.HasKey(x => x.Id); });

            mb.Entity<AttendancePunch>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.PunchTimeUtc).HasColumnType("datetime2(0)");
                e.Property(x => x.PunchLocalTime).HasColumnType("datetime2(0)");
                e.Property(x => x.AttendanceDate).HasColumnType("date");
                e.Property(x => x.ClientTime).HasColumnType("datetime2(0)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.RejectReason).HasMaxLength(200);
                e.Property(x => x.DeviceUserPin).HasMaxLength(30);
                e.Property(x => x.ClientRequestId).HasMaxLength(100);
                e.Property(x => x.ClientIp).HasMaxLength(64);
                e.Property(x => x.WifiBssid).HasMaxLength(64);
                e.Property(x => x.WifiSsid).HasMaxLength(100);
                e.Property(x => x.LocationCheck).HasMaxLength(100);
                e.Property(x => x.RawData).HasMaxLength(500);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_AttendancePunches_Employee");
                e.HasOne<PunchSourceLookup>().WithMany().HasForeignKey(x => x.SourceId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_AttendancePunches_Source");
                e.HasOne<PunchStatusLookup>().WithMany().HasForeignKey(x => x.StatusId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_AttendancePunches_Status");
                e.HasIndex(x => new { x.EmployeeId, x.AttendanceDate, x.StatusId }).HasDatabaseName("IX_AttendancePunches_Emp_Date");
                e.HasIndex(x => x.PunchTimeUtc).HasDatabaseName("IX_AttendancePunches_Time");
                // the same app request / terminal record is stored once
                e.HasIndex(x => new { x.SourceId, x.ClientRequestId }).IsUnique()
                    .HasFilter("[ClientRequestId] IS NOT NULL").HasDatabaseName("UX_AttendancePunches_Request");
            });

            mb.Entity<BiometricTypeLookup>().HasData(
                L<BiometricTypeLookup>(1, "FINGERPRINT", "Fingerprint"), L<BiometricTypeLookup>(2, "FACE", "Face"));
            mb.Entity<BiometricRegistrationStatusLookup>().HasData(
                L<BiometricRegistrationStatusLookup>(1, "PENDING", "Pending Verification"),
                L<BiometricRegistrationStatusLookup>(2, "ACTIVE", "Active"),
                L<BiometricRegistrationStatusLookup>(3, "REVOKED", "Revoked"));
            mb.Entity<PunchSourceLookup>().HasData(
                L<PunchSourceLookup>(1, "MOBILE", "Mobile App"), L<PunchSourceLookup>(2, "TERMINAL", "Door Terminal"),
                L<PunchSourceLookup>(3, "MANUAL", "Manual"));
            mb.Entity<PunchStatusLookup>().HasData(
                L<PunchStatusLookup>(1, "ACCEPTED", "Accepted"), L<PunchStatusLookup>(2, "DUPLICATE", "Duplicate"),
                L<PunchStatusLookup>(3, "REJECTED", "Rejected"));

            // Office network for local development and a typical office LAN: the private address
            // ranges plus this machine. Replace with the real office ranges / Wi-Fi BSSIDs per branch.
            mb.Entity<AttendanceZone>().HasData(new AttendanceZone
            {
                Id = new Guid("0A7E0001-0000-4000-8000-000000000001"), ZoneCode = "ZONE-LAN", Name = "Office LAN (default)",
                AllowedIpRanges = "127.0.0.1/32,::1/128,10.0.0.0/8,172.16.0.0/12,192.168.0.0/16",
                Remarks = "Seeded: private network ranges. Narrow to the office subnet / public IP before go-live.",
                CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            });
        }
        #endregion

        #region Leave (was HumanResourcesDBContext.Leave.cs)
        // Leave slice of HumanResourcesDBContext. Call ConfigureLeave(mb) at the end
        // of OnModelCreating. LeaveTransactions are an immutable ledger (no triggers).
        public virtual DbSet<LeaveApplicationStatusLookup>? LeaveApplicationStatuses { get; set; }
        public virtual DbSet<LeaveTransactionTypeLookup>?   LeaveTransactionTypes    { get; set; }

        public virtual DbSet<LeaveType>?       LeaveTypes        { get; set; }
        public virtual DbSet<LeaveTypeLog>?    LeaveTypesLog     { get; set; }
        public virtual DbSet<LeavePolicy>?     LeavePolicies     { get; set; }
        public virtual DbSet<LeavePolicyLog>?  LeavePoliciesLog  { get; set; }
        public virtual DbSet<LeaveBalance>?    LeaveBalances     { get; set; }
        public virtual DbSet<LeaveApplication>?    LeaveApplications    { get; set; }
        public virtual DbSet<LeaveApplicationLog>? LeaveApplicationsLog { get; set; }
        public virtual DbSet<LeaveTransaction>?    LeaveTransactions    { get; set; }

        private void ConfigureLeave(ModelBuilder mb)
        {
            mb.Entity<LeaveApplicationStatusLookup>(e => { e.ToTable("LeaveApplicationStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<LeaveTransactionTypeLookup>(e => { e.ToTable("LeaveTransactionTypes"); e.HasKey(x => x.Id); });

            mb.Entity<LeaveType>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.LeaveTypeCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.HasIndex(x => x.LeaveTypeCode).IsUnique().HasDatabaseName("UX_LeaveTypes_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertLeaveTypes"); t.HasTrigger("TRG_UpdateLeaveTypes"); });
            });
            mb.Entity<LeaveTypeLog>(e => e.HasKey(x => x.Id));

            mb.Entity<LeavePolicy>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.PolicyCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.LeaveType).WithMany().HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.PolicyCode).IsUnique().HasDatabaseName("UX_LeavePolicies_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertLeavePolicies"); t.HasTrigger("TRG_UpdateLeavePolicies"); });
            });
            mb.Entity<LeavePolicyLog>(e => e.HasKey(x => x.Id));

            mb.Entity<LeaveBalance>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.LeaveType).WithMany().HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique().HasDatabaseName("UX_LeaveBalances_Emp_Type_Year");
                // no triggers — maintained by SPs
            });

            mb.Entity<LeaveApplication>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ApplicationCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.LeaveType).WithMany().HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.ApplicationCode).IsUnique().HasDatabaseName("UX_LeaveApplications_Code");
                e.HasIndex(x => new { x.EmployeeId, x.Year, x.StatusId }).HasDatabaseName("IX_LeaveApps_Emp_Year");
                e.ToTable(t => { t.HasTrigger("TRG_InsertLeaveApplications"); t.HasTrigger("TRG_UpdateLeaveApplications"); });
            });
            mb.Entity<LeaveApplicationLog>(e => e.HasKey(x => x.Id));

            mb.Entity<LeaveTransaction>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne<Employee>().WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).HasDatabaseName("IX_LeaveTxn_Emp_Type_Year");
            });
        }
        #endregion

        #region Onboarding (was HumanResourcesDBContext.Onboarding.cs)
        // Onboarding slice of HumanResourcesDBContext. Add 'partial' to the core
        // context class, declare `partial void ConfigureOnboarding(ModelBuilder mb);`
        // and call it at the end of OnModelCreating (next to ConfigureRecruitment).
        // IntegrationOutbox has NO triggers — it's plumbing, not an audited entity.
        public virtual DbSet<OnboardingStatusLookup>?     OnboardingStatuses     { get; set; }
        public virtual DbSet<OnboardingTaskStatusLookup>? OnboardingTaskStatuses { get; set; }

        public virtual DbSet<Onboarding>?       Onboardings    { get; set; }
        public virtual DbSet<OnboardingLog>?    OnboardingsLog { get; set; }
        public virtual DbSet<OnboardingTask>?   OnboardingTasks { get; set; }
        public virtual DbSet<IntegrationOutbox>? IntegrationOutbox { get; set; }

        private void ConfigureOnboarding(ModelBuilder mb)
        {
            mb.Entity<OnboardingStatusLookup>(e => { e.ToTable("OnboardingStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<OnboardingTaskStatusLookup>(e => { e.ToTable("OnboardingTaskStatuses"); e.HasKey(x => x.Id); });

            mb.Entity<Onboarding>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.OnboardingCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.OnboardingCode).IsUnique().HasDatabaseName("UX_Onboardings_Code");
                e.HasIndex(x => x.JobOfferId).IsUnique().HasDatabaseName("UX_Onboardings_Offer");
                e.HasMany(x => x.Tasks).WithOne().HasForeignKey(t => t.OnboardingId)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_OnboardingTasks_Onboarding");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertOnboardings");
                    t.HasTrigger("TRG_UpdateOnboardings");
                });
            });
            mb.Entity<OnboardingLog>(e => e.HasKey(x => x.Id));

            mb.Entity<OnboardingTask>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
            });

            mb.Entity<IntegrationOutbox>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.EventType).HasMaxLength(200).IsRequired();
                e.Property(x => x.RoutingKey).HasMaxLength(200).IsRequired();
                e.Property(x => x.OccurredOn).HasColumnType("datetime");
                e.Property(x => x.ProcessedDate).HasColumnType("datetime");
                e.HasIndex(x => new { x.ProcessedDate, x.OccurredOn })
                    .HasDatabaseName("IX_IntegrationOutbox_Unprocessed");
            });
        }
        #endregion

        #region OrgSetup (was HumanResourcesDBContext.OrgSetup.cs)
        // Organization Setup (Module 0 foundation) slice. Company/Branch are mapped
        // in the main OnModelCreating (hr2); this slice adds Cost Center, Job Grade,
        // Holiday Calendar (+dates) and the Employee/Branch links into them.
        // Position itself is configured in the Recruitment slice (shared entity) —
        // the extra org-structure FKs on it are added here.
        public virtual DbSet<PositionStatusLookup>? PositionStatuses { get; set; }
        public virtual DbSet<HolidayTypeLookup>?    HolidayTypes     { get; set; }
        public virtual DbSet<CostCenter>?           CostCenters      { get; set; }
        public virtual DbSet<CostCenterLog>?        CostCentersLog   { get; set; }
        public virtual DbSet<JobGrade>?             JobGrades        { get; set; }
        public virtual DbSet<JobGradeLog>?          JobGradesLog     { get; set; }
        public virtual DbSet<HolidayCalendar>?      HolidayCalendars { get; set; }
        public virtual DbSet<HolidayCalendarLog>?   HolidayCalendarsLog { get; set; }
        public virtual DbSet<HolidayDate>?          HolidayDates     { get; set; }

        // Deterministic seed ids — referenced by other module seeds.
        internal static readonly Guid SeedCompanyId      = new("EEB27F7F-F7DB-479B-923F-3BE0AD7A243A");
        internal static readonly Guid SeedBranchId       = new("7AA06B50-B394-41B9-B17B-A96CD0E18B8B");
        internal static readonly Guid SeedCalendarId     = new("0C0A1D00-0000-4000-8000-000000002026");
        internal static readonly Guid SeedCcCorporateId  = new("0CC00001-0000-4000-8000-000000000001");
        internal static readonly Guid SeedCcEngineeringId= new("0CC00001-0000-4000-8000-000000000002");
        internal static readonly Guid SeedCcOperationsId = new("0CC00001-0000-4000-8000-000000000003");
        internal static readonly DateTime SeedTime       = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        internal const string SeedCreator                = "C047D662-9F0E-4358-B323-15EC3081312C";

        private void ConfigureOrgSetup(ModelBuilder mb)
        {
            mb.Entity<PositionStatusLookup>(e => { e.ToTable("PositionStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<HolidayTypeLookup>(e => { e.ToTable("HolidayTypes"); e.HasKey(x => x.Id); });

            mb.Entity<CostCenter>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CostCenterCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Description).HasMaxLength(300);
                e.Property(x => x.CreatedBy).HasMaxLength(450);
                e.Property(x => x.UpdatedBy).HasMaxLength(450);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.CostCenterCode).IsUnique().HasDatabaseName("UX_CostCenters_Code");
                e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_CostCenters_Company");
                e.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_CostCenters_Branch");
                e.HasOne<CostCenter>().WithMany().HasForeignKey(x => x.ParentCostCenterId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_CostCenters_Parent");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertCostCenters");
                    t.HasTrigger("TRG_UpdateCostCenters");
                });
            });

            mb.Entity<JobGrade>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.GradeCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Band).HasMaxLength(80);
                e.Property(x => x.CurrencyCode).HasMaxLength(10);
                e.Property(x => x.Description).HasMaxLength(300);
                e.Property(x => x.CreatedBy).HasMaxLength(450);
                e.Property(x => x.UpdatedBy).HasMaxLength(450);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.GradeCode).IsUnique().HasDatabaseName("UX_JobGrades_Code");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertJobGrades");
                    t.HasTrigger("TRG_UpdateJobGrades");
                });
            });

            mb.Entity<HolidayCalendar>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CalendarCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Country).HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(300);
                e.Property(x => x.CreatedBy).HasMaxLength(450);
                e.Property(x => x.UpdatedBy).HasMaxLength(450);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.CalendarCode).IsUnique().HasDatabaseName("UX_HolidayCalendars_Code");
                e.HasMany(x => x.HolidayDates).WithOne(d => d.Calendar!)
                    .HasForeignKey(d => d.HolidayCalendarId).OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HolidayDates_Calendar");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertHolidayCalendars");
                    t.HasTrigger("TRG_UpdateHolidayCalendars");
                });
            });

            mb.Entity<HolidayDate>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.HolidayDate1).HasColumnName("HolidayDate").HasColumnType("date");
                e.Property(x => x.Description).HasMaxLength(300);
                e.Property(x => x.CreatedBy).HasMaxLength(450);
                e.Property(x => x.UpdatedBy).HasMaxLength(450);
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => new { x.HolidayCalendarId, x.HolidayDate1 }).HasDatabaseName("IX_HolidayDates_Calendar_Date");
                e.HasOne<HolidayTypeLookup>().WithMany().HasForeignKey(x => x.HolidayTypeId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_HolidayDates_Type");
            });

            mb.Entity<CostCenterLog>(e => e.HasKey(x => x.Id));
            mb.Entity<JobGradeLog>(e => e.HasKey(x => x.Id));
            mb.Entity<HolidayCalendarLog>(e => e.HasKey(x => x.Id));

            // --- Position: org-structure FKs (entity itself mapped in the Recruitment slice)
            mb.Entity<Position>(e =>
            {
                e.Property(x => x.WorkLocation).HasMaxLength(200);
                e.Property(x => x.SalaryCurrency).HasMaxLength(10);
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.EffectiveFrom).HasColumnType("datetime");
                e.Property(x => x.EffectiveTo).HasColumnType("datetime");
                e.Property(x => x.StatusId).HasDefaultValue(2);
                e.HasOne<Company>().WithMany().HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_Company");
                e.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_Branch");
                e.HasOne<CostCenter>().WithMany().HasForeignKey(x => x.CostCenterId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_CostCenter");
                e.HasOne<JobGrade>().WithMany().HasForeignKey(x => x.JobGradeId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_JobGrade");
                e.HasOne<Position>().WithMany().HasForeignKey(x => x.ReportsToPositionId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_ReportsTo");
                e.HasOne<PositionStatusLookup>().WithMany().HasForeignKey(x => x.StatusId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_Status");
            });

            // --- Branch -> working calendar; Employee -> seat / grade / cost center
            mb.Entity<Branch>(e =>
            {
                e.Property(x => x.TimeZone).HasMaxLength(80);
                e.HasOne<HolidayCalendar>().WithMany().HasForeignKey(x => x.HolidayCalendarId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Branches_HolidayCalendar");
            });
            mb.Entity<Employee>(e =>
            {
                e.HasOne<CostCenter>().WithMany().HasForeignKey(x => x.CostCenterId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Employees_CostCenter");
                e.HasOne<Position>().WithMany().HasForeignKey(x => x.PositionId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Employees_Position");
                e.HasOne<JobGrade>().WithMany().HasForeignKey(x => x.JobGradeId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Employees_JobGrade");
            });

            // =================================================================
            // SEED DATA
            // =================================================================
            mb.Entity<PositionStatusLookup>().HasData(
                new PositionStatusLookup { Id = 1, Code = "DRAFT",   Name = "Draft",   SortOrder = 1, IsActive = true },
                new PositionStatusLookup { Id = 2, Code = "OPEN",    Name = "Open",    SortOrder = 2, IsActive = true },
                new PositionStatusLookup { Id = 3, Code = "FILLED",  Name = "Filled",  SortOrder = 3, IsActive = true },
                new PositionStatusLookup { Id = 4, Code = "ON_HOLD", Name = "On Hold", SortOrder = 4, IsActive = true },
                new PositionStatusLookup { Id = 5, Code = "CLOSED",  Name = "Closed",  SortOrder = 5, IsActive = true });

            mb.Entity<HolidayTypeLookup>().HasData(
                new HolidayTypeLookup { Id = 1, Code = "PUBLIC",    Name = "Public Holiday",        SortOrder = 1, IsActive = true },
                new HolidayTypeLookup { Id = 2, Code = "RELIGIOUS", Name = "Religious",             SortOrder = 2, IsActive = true },
                new HolidayTypeLookup { Id = 3, Code = "NATIONAL",  Name = "National",              SortOrder = 3, IsActive = true },
                new HolidayTypeLookup { Id = 4, Code = "COMPANY",   Name = "Company Holiday",       SortOrder = 4, IsActive = true },
                new HolidayTypeLookup { Id = 5, Code = "OPTIONAL",  Name = "Optional / Restricted", SortOrder = 5, IsActive = true });

            mb.Entity<JobGrade>().HasData(
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000001"), GradeCode = "GR-0001", Name = "Executive",        GradeLevel = 1, Band = "Leadership", RefMinSalary = 60000, RefMidSalary = 80000, RefMaxSalary = 100000, CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000002"), GradeCode = "GR-0002", Name = "Senior Manager",   GradeLevel = 2, Band = "Management", RefMinSalary = 40000, RefMidSalary = 50000, RefMaxSalary = 60000,  CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000003"), GradeCode = "GR-0003", Name = "Manager",          GradeLevel = 3, Band = "Management", RefMinSalary = 28000, RefMidSalary = 34000, RefMaxSalary = 40000,  CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000004"), GradeCode = "GR-0004", Name = "Senior Specialist", GradeLevel = 4, Band = "Professional", RefMinSalary = 18000, RefMidSalary = 23000, RefMaxSalary = 28000, CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000005"), GradeCode = "GR-0005", Name = "Specialist",       GradeLevel = 5, Band = "Professional", RefMinSalary = 10000, RefMidSalary = 14000, RefMaxSalary = 18000, CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new JobGrade { Id = new Guid("06AD0001-0000-4000-8000-000000000006"), GradeCode = "GR-0006", Name = "Associate",        GradeLevel = 6, Band = "Support",    RefMinSalary = 5000,  RefMidSalary = 7500,  RefMaxSalary = 10000,  CurrencyCode = "AED", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true });

            mb.Entity<CostCenter>().HasData(
                new CostCenter { Id = SeedCcCorporateId,   CostCenterCode = "CC-0001", Name = "Corporate & Administration", CompanyId = SeedCompanyId, BranchId = SeedBranchId, Description = "Executive office, HR and Finance", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new CostCenter { Id = SeedCcEngineeringId, CostCenterCode = "CC-0002", Name = "Engineering",                CompanyId = SeedCompanyId, BranchId = SeedBranchId, ParentCostCenterId = SeedCcCorporateId, Description = "Product and software engineering", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new CostCenter { Id = SeedCcOperationsId,  CostCenterCode = "CC-0003", Name = "Operations",                 CompanyId = SeedCompanyId, BranchId = SeedBranchId, ParentCostCenterId = SeedCcCorporateId, Description = "Business operations", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true });

            mb.Entity<HolidayCalendar>().HasData(
                new HolidayCalendar { Id = SeedCalendarId, CalendarCode = "HC-0001", Name = "UAE Public Holidays 2026", Year = 2026, Country = "United Arab Emirates",
                    Description = "Islamic holiday dates are moon-sighting estimates — confirm against the official announcement.",
                    CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true });

            HolidayDate Hd(int n, string name, int m, int d, int type, bool recurring) => new()
            {
                Id = new Guid($"0D0A1D00-0000-4000-8000-{n:D12}"), HolidayCalendarId = SeedCalendarId, Name = name,
                HolidayDate1 = new DateTime(2026, m, d), HolidayTypeId = type, IsRecurring = recurring,
                CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            };
            mb.Entity<HolidayDate>().HasData(
                Hd(1,  "New Year's Day",            1, 1,  1, true),
                Hd(2,  "Eid Al Fitr (Day 1)",       3, 20, 2, false),
                Hd(3,  "Eid Al Fitr (Day 2)",       3, 21, 2, false),
                Hd(4,  "Eid Al Fitr (Day 3)",       3, 22, 2, false),
                Hd(5,  "Arafat Day",                5, 26, 2, false),
                Hd(6,  "Eid Al Adha (Day 1)",       5, 27, 2, false),
                Hd(7,  "Eid Al Adha (Day 2)",       5, 28, 2, false),
                Hd(8,  "Eid Al Adha (Day 3)",       5, 29, 2, false),
                Hd(9,  "Islamic New Year",          6, 16, 2, false),
                Hd(10, "Prophet Muhammad's Birthday", 8, 25, 2, false),
                Hd(11, "Commemoration Day",         12, 1, 3, true),
                Hd(12, "UAE National Day (Day 1)",  12, 2, 3, true),
                Hd(13, "UAE National Day (Day 2)",  12, 3, 3, true));

            // Positions (seats) for the seeded company — headcount kept at 0 filled
            // so Recruitment/Onboarding can demonstrate the fill flow.
            Position Pos(int n, string title, string desigId, string deptId, Guid cc, int grade, int budget, Guid? reportsTo) => new()
            {
                Id = new Guid($"0B050000-0000-4000-8000-{n:D12}"), PositionCode = $"POS-{n:D4}", Title = title,
                DesignationId = new Guid(desigId), DepartmentId = new Guid(deptId), CompanyId = SeedCompanyId, BranchId = SeedBranchId,
                CostCenterId = cc, JobGradeId = new Guid($"06AD0001-0000-4000-8000-{grade:D12}"), ReportsToPositionId = reportsTo,
                BudgetedHeadcount = budget, FilledHeadcount = 0, StatusId = 2, EmploymentType = 1, WorkLocation = "Dubai",
                SalaryCurrency = "AED", EffectiveFrom = SeedTime, CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            };
            var ceo = new Guid("0B050000-0000-4000-8000-000000000001");
            var hrm = new Guid("0B050000-0000-4000-8000-000000000002");
            var engMgr = new Guid("0B050000-0000-4000-8000-000000000003");
            mb.Entity<Position>().HasData(
                Pos(1, "Chief Executive Officer",  "4E2E6864-B92C-4224-8BEF-20F7E72DF648", "28260DA1-0BB1-4842-A3EC-786F859DC5CA", SeedCcCorporateId,   1, 1, null),
                Pos(2, "HR Manager",               "60BBB9DB-3A9E-4560-BB0F-C93DBAF9C7DB", "2B3C7610-F979-48F0-881C-44B9F3C93555", SeedCcCorporateId,   3, 1, ceo),
                Pos(3, "Engineering Manager",      "E619D48F-BD89-404F-A406-F466F1B62088", "1F8FD7EE-C9C4-484B-AEB9-3F81F2C04609", SeedCcEngineeringId, 3, 1, ceo),
                Pos(4, "Senior Software Engineer", "51DA7CD0-7D22-4C3B-A01E-21E1EAFD27E9", "1F8FD7EE-C9C4-484B-AEB9-3F81F2C04609", SeedCcEngineeringId, 4, 3, engMgr),
                Pos(5, "Software Engineer",        "368C0E9B-E60D-40CF-912C-43744DB1B19F", "1F8FD7EE-C9C4-484B-AEB9-3F81F2C04609", SeedCcEngineeringId, 5, 5, engMgr),
                Pos(6, "Accountant",               "82B5F6D0-5403-48D0-8D1C-8EF510762631", "729197EE-0A7E-4910-81A1-BE9060A51AE7", SeedCcCorporateId,   5, 2, ceo),
                Pos(7, "HR Intern",                "F2597DAE-B3C6-47BB-B2A1-5CAA5DCFD770", "2B3C7610-F979-48F0-881C-44B9F3C93555", SeedCcCorporateId,   6, 1, hrm));
        }
        #endregion

        #region Payroll (was HumanResourcesDBContext.Payroll.cs)
        // Payroll slice of HumanResourcesDBContext. Call ConfigurePayroll(mb) at the
        // end of OnModelCreating. Payslips/PayslipLines/LoanInstallments are written
        // by SP_ProcessPayrollRun and carry no triggers (the run is the audit unit).
        public virtual DbSet<PayComponentTypeLookup>? PayComponentTypes { get; set; }
        public virtual DbSet<PayrollRunStatusLookup>? PayrollRunStatuses { get; set; }
        public virtual DbSet<LoanStatusLookup>?       LoanStatuses { get; set; }

        public virtual DbSet<PayComponent>?    PayComponents    { get; set; }
        public virtual DbSet<PayComponentLog>? PayComponentsLog { get; set; }
        public virtual DbSet<SalaryStructure>?     SalaryStructures     { get; set; }
        public virtual DbSet<SalaryStructureLog>?  SalaryStructuresLog  { get; set; }
        public virtual DbSet<SalaryStructureLine>? SalaryStructureLines { get; set; }
        public virtual DbSet<PayrollRun>?    PayrollRuns    { get; set; }
        public virtual DbSet<PayrollRunLog>? PayrollRunsLog { get; set; }
        public virtual DbSet<Payslip>?     Payslips     { get; set; }
        public virtual DbSet<PayslipLine>? PayslipLines { get; set; }
        public virtual DbSet<Loan>?            Loans            { get; set; }
        public virtual DbSet<LoanLog>?         LoansLog         { get; set; }
        public virtual DbSet<LoanInstallment>? LoanInstallments { get; set; }

        private void ConfigurePayroll(ModelBuilder mb)
        {
            mb.Entity<PayComponentTypeLookup>(e => { e.ToTable("PayComponentTypes"); e.HasKey(x => x.Id); });
            mb.Entity<PayrollRunStatusLookup>(e => { e.ToTable("PayrollRunStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<LoanStatusLookup>(e => { e.ToTable("LoanStatuses"); e.HasKey(x => x.Id); });

            mb.Entity<PayComponent>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ComponentCode).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.ComponentCode).IsUnique().HasDatabaseName("UX_PayComponents_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertPayComponents"); t.HasTrigger("TRG_UpdatePayComponents"); });
            });
            mb.Entity<PayComponentLog>(e => e.HasKey(x => x.Id));

            mb.Entity<SalaryStructure>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.StructureCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalaryStructureId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.StructureCode).IsUnique().HasDatabaseName("UX_SalaryStructures_Code");
                e.HasIndex(x => new { x.EmployeeId, x.EffectiveFrom }).HasDatabaseName("IX_SalaryStructures_Emp");
                e.ToTable(t => { t.HasTrigger("TRG_InsertSalaryStructures"); t.HasTrigger("TRG_UpdateSalaryStructures"); });
            });
            mb.Entity<SalaryStructureLog>(e => e.HasKey(x => x.Id));
            mb.Entity<SalaryStructureLine>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.PayComponent).WithMany().HasForeignKey(x => x.PayComponentId).OnDelete(DeleteBehavior.NoAction);
            });

            mb.Entity<PayrollRun>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.RunCode).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.RunCode).IsUnique().HasDatabaseName("UX_PayrollRuns_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertPayrollRuns"); t.HasTrigger("TRG_UpdatePayrollRuns"); });
            });
            mb.Entity<PayrollRunLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Payslip>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PayslipId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.PayrollRunId).HasDatabaseName("IX_Payslips_Run");
                e.HasIndex(x => x.EmployeeId).HasDatabaseName("IX_Payslips_Emp");
                // no triggers — written by SP_ProcessPayrollRun
            });
            mb.Entity<PayslipLine>(e => e.HasKey(x => x.Id));

            mb.Entity<Loan>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.LoanCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.LoanCode).IsUnique().HasDatabaseName("UX_Loans_Code");
                e.HasIndex(x => new { x.EmployeeId, x.StatusId }).HasDatabaseName("IX_Loans_Emp");
                e.ToTable(t => { t.HasTrigger("TRG_InsertLoans"); t.HasTrigger("TRG_UpdateLoans"); });
            });
            mb.Entity<LoanLog>(e => e.HasKey(x => x.Id));
            mb.Entity<LoanInstallment>(e => e.HasKey(x => x.Id));
        }
        #endregion

        #region Performance (was HumanResourcesDBContext.Performance.cs)
        // Performance slice of HumanResourcesDBContext. Call ConfigurePerformance(mb)
        // at the end of OnModelCreating. ReviewRatings are written/updated by the
        // review SPs and carry no triggers (the Review header is the audit unit).
        public virtual DbSet<AppraisalCycleStatusLookup>? AppraisalCycleStatuses { get; set; }
        public virtual DbSet<GoalStatusLookup>?           GoalStatuses           { get; set; }
        public virtual DbSet<ReviewStatusLookup>?         ReviewStatuses         { get; set; }
        public virtual DbSet<RatingItemTypeLookup>?       RatingItemTypes        { get; set; }

        public virtual DbSet<AppraisalCycle>?    AppraisalCycles    { get; set; }
        public virtual DbSet<AppraisalCycleLog>? AppraisalCyclesLog { get; set; }
        public virtual DbSet<Competency>?    Competencies    { get; set; }
        public virtual DbSet<CompetencyLog>? CompetenciesLog { get; set; }
        public virtual DbSet<Goal>?    Goals    { get; set; }
        public virtual DbSet<GoalLog>? GoalsLog { get; set; }
        public virtual DbSet<Review>?    Reviews    { get; set; }
        public virtual DbSet<ReviewLog>? ReviewsLog { get; set; }
        public virtual DbSet<ReviewRating>? ReviewRatings { get; set; }

        private void ConfigurePerformance(ModelBuilder mb)
        {
            mb.Entity<AppraisalCycleStatusLookup>(e => { e.ToTable("AppraisalCycleStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<GoalStatusLookup>(e => { e.ToTable("GoalStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<ReviewStatusLookup>(e => { e.ToTable("ReviewStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<RatingItemTypeLookup>(e => { e.ToTable("RatingItemTypes"); e.HasKey(x => x.Id); });

            mb.Entity<AppraisalCycle>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CycleCode).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.CycleCode).IsUnique().HasDatabaseName("UX_AppraisalCycles_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertAppraisalCycles"); t.HasTrigger("TRG_UpdateAppraisalCycles"); });
            });
            mb.Entity<AppraisalCycleLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Competency>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CompetencyCode).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.CompetencyCode).IsUnique().HasDatabaseName("UX_Competencies_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertCompetencies"); t.HasTrigger("TRG_UpdateCompetencies"); });
            });
            mb.Entity<CompetencyLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Goal>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.GoalCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.AppraisalCycle).WithMany().HasForeignKey(x => x.AppraisalCycleId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasIndex(x => x.GoalCode).IsUnique().HasDatabaseName("UX_Goals_Code");
                e.HasIndex(x => new { x.AppraisalCycleId, x.EmployeeId, x.StatusId }).HasDatabaseName("IX_Goals_Cycle_Emp");
                e.ToTable(t => { t.HasTrigger("TRG_InsertGoals"); t.HasTrigger("TRG_UpdateGoals"); });
            });
            mb.Entity<GoalLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Review>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ReviewCode).HasMaxLength(30).IsRequired();
                e.HasOne(x => x.AppraisalCycle).WithMany().HasForeignKey(x => x.AppraisalCycleId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Ratings).WithOne().HasForeignKey(x => x.ReviewId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.ReviewCode).IsUnique().HasDatabaseName("UX_Reviews_Code");
                e.ToTable(t => { t.HasTrigger("TRG_InsertReviews"); t.HasTrigger("TRG_UpdateReviews"); });
            });
            mb.Entity<ReviewLog>(e => e.HasKey(x => x.Id));
            mb.Entity<ReviewRating>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.ReviewId).HasDatabaseName("IX_ReviewRatings_Review");
                // no triggers — written by the review SPs
            });
        }
        #endregion

        #region Recruitment (was HumanResourcesDBContext.Recruitment.cs)
        // Recruitment slice of HumanResourcesDBContext, kept in a partial so the
        // core context file stays focused. Add `partial` to the existing
        // HumanResourcesDBContext class declaration, then this file plugs in the
        // DbSets and configuration. Call ConfigureRecruitment(mb) at the end of the
        // existing OnModelCreating.
        // Triggers are declared via HasTrigger so EF uses a trigger-safe save path
        // (no OUTPUT clause) — same as Departments/Designations/Employees.
        // lookups
        public virtual DbSet<RequisitionStatusLookup>? RequisitionStatuses { get; set; }
        public virtual DbSet<JobPostingStatusLookup>?  JobPostingStatuses  { get; set; }
        public virtual DbSet<ApplicationStageLookup>?  ApplicationStages   { get; set; }
        public virtual DbSet<InterviewModeLookup>?     InterviewModes      { get; set; }
        public virtual DbSet<OfferStatusLookup>?       OfferStatuses       { get; set; }

        // entities + logs
        public virtual DbSet<Position>?               Positions               { get; set; }
        public virtual DbSet<PositionLog>?            PositionsLog            { get; set; }
        public virtual DbSet<ManpowerRequisition>?    ManpowerRequisitions    { get; set; }
        public virtual DbSet<ManpowerRequisitionLog>? ManpowerRequisitionsLog { get; set; }
        public virtual DbSet<JobPosting>?             JobPostings             { get; set; }
        public virtual DbSet<JobPostingLog>?          JobPostingsLog          { get; set; }
        public virtual DbSet<Candidate>?              Candidates              { get; set; }
        public virtual DbSet<CandidateLog>?           CandidatesLog           { get; set; }
        public virtual DbSet<JobApplication>?         JobApplications         { get; set; }
        public virtual DbSet<JobApplicationLog>?      JobApplicationsLog      { get; set; }
        public virtual DbSet<Interview>?              Interviews              { get; set; }
        public virtual DbSet<InterviewPanelMember>?   InterviewPanelMembers   { get; set; }
        public virtual DbSet<JobOffer>?               JobOffers               { get; set; }
        public virtual DbSet<JobOfferLog>?            JobOffersLog            { get; set; }

        /// <summary>Call from OnModelCreating: ConfigureRecruitment(mb);</summary>
        private void ConfigureRecruitment(ModelBuilder mb)
        {
            // lookups
            mb.Entity<RequisitionStatusLookup>(e => { e.ToTable("RequisitionStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<JobPostingStatusLookup>(e =>  { e.ToTable("JobPostingStatuses");  e.HasKey(x => x.Id); });
            mb.Entity<ApplicationStageLookup>(e =>  { e.ToTable("ApplicationStages");   e.HasKey(x => x.Id); });
            mb.Entity<InterviewModeLookup>(e =>     { e.ToTable("InterviewModes");      e.HasKey(x => x.Id); });
            mb.Entity<OfferStatusLookup>(e =>       { e.ToTable("OfferStatuses");       e.HasKey(x => x.Id); });

            mb.Entity<Position>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.PositionCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Title).HasMaxLength(150).IsRequired();
                e.Property(x => x.MinSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.MaxSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.PositionCode).IsUnique().HasDatabaseName("UX_Positions_Code");
                e.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_Departments");
                e.HasOne(x => x.Designation).WithMany().HasForeignKey(x => x.DesignationId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Positions_Designations");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertPositions");
                    t.HasTrigger("TRG_UpdatePositions");
                    t.HasTrigger("TRG_DeletePositions");
                });
            });
            mb.Entity<PositionLog>(e => e.HasKey(x => x.Id));

            mb.Entity<ManpowerRequisition>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.RequisitionCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.BudgetMin).HasColumnType("decimal(18,2)");
                e.Property(x => x.BudgetMax).HasColumnType("decimal(18,2)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.RequisitionCode).IsUnique().HasDatabaseName("UX_ManpowerRequisitions_Code");
                e.HasOne(x => x.Position).WithMany().HasForeignKey(x => x.PositionId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_MRF_Positions");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertManpowerRequisitions");
                    t.HasTrigger("TRG_UpdateManpowerRequisitions");
                    t.HasTrigger("TRG_DeleteManpowerRequisitions");
                });
            });
            mb.Entity<ManpowerRequisitionLog>(e => e.HasKey(x => x.Id));

            mb.Entity<JobPosting>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.PostingCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.PostingCode).IsUnique().HasDatabaseName("UX_JobPostings_Code");
                e.HasOne(x => x.ManpowerRequisition).WithMany().HasForeignKey(x => x.ManpowerRequisitionId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_JobPostings_MRF");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertJobPostings");
                    t.HasTrigger("TRG_UpdateJobPostings");
                    t.HasTrigger("TRG_DeleteJobPostings");
                });
            });
            mb.Entity<JobPostingLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Candidate>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CandidateCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.Property(x => x.TotalExperienceYears).HasColumnType("decimal(4,1)");
                e.Property(x => x.CurrentSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.ExpectedSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.CandidateCode).IsUnique().HasDatabaseName("UX_Candidates_Code");
                e.HasIndex(x => x.Email).HasDatabaseName("IX_Candidates_Email");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertCandidates");
                    t.HasTrigger("TRG_UpdateCandidates");
                });
            });
            mb.Entity<CandidateLog>(e => e.HasKey(x => x.Id));

            mb.Entity<JobApplication>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ApplicationCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.ScreeningScore).HasColumnType("decimal(5,2)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.ApplicationCode).IsUnique().HasDatabaseName("UX_JobApplications_Code");
                e.HasOne(x => x.JobPosting).WithMany().HasForeignKey(x => x.JobPostingId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_JobApplications_Posting");
                e.HasOne(x => x.Candidate).WithMany().HasForeignKey(x => x.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_JobApplications_Candidate");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertJobApplications");
                    t.HasTrigger("TRG_UpdateJobApplications");
                });
            });
            mb.Entity<JobApplicationLog>(e => e.HasKey(x => x.Id));

            mb.Entity<Interview>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.OverallRating).HasColumnType("decimal(3,1)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasMany(x => x.PanelMembers).WithOne().HasForeignKey(p => p.InterviewId)
                    .OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Panel_Interview");
            });
            mb.Entity<InterviewPanelMember>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.IndividualRating).HasColumnType("decimal(3,1)");
            });

            mb.Entity<JobOffer>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.OfferCode).HasMaxLength(30).IsRequired();
                e.Property(x => x.OfferedSalary).HasColumnType("decimal(18,2)");
                e.Property(x => x.CreatedDate).HasColumnType("datetime");
                e.Property(x => x.UpdatedDate).HasColumnType("datetime");
                e.HasIndex(x => x.OfferCode).IsUnique().HasDatabaseName("UX_JobOffers_Code");
                e.HasOne(x => x.Candidate).WithMany().HasForeignKey(x => x.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_JobOffers_Candidate");
                e.HasOne(x => x.JobApplication).WithMany().HasForeignKey(x => x.JobApplicationId)
                    .OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_JobOffers_Application");
                e.ToTable(t =>
                {
                    t.HasTrigger("TRG_InsertJobOffers");
                    t.HasTrigger("TRG_UpdateJobOffers");
                });
            });
            mb.Entity<JobOfferLog>(e => e.HasKey(x => x.Id));
        }
        #endregion

        #region SeedData (was HumanResourcesDBContext.SeedData.cs)
        // EF seed data for the HR sub-modules (Recruitment → Separation): every
        // workflow lookup (Ids mirror the C# enums) plus the reference/master rows a
        // fresh tenant needs on day one — leave types, pay components, competencies,
        // shifts. All keys are fixed GUIDs so re-generating migrations is stable.
        // The guarded INSERTs left in Persistence/Sql/1x_*.sql are no-ops once these exist.
        private static T L<T>(int id, string code, string name) where T : BaseLookup, new()
            => new() { Id = id, Code = code, Name = name, SortOrder = id, IsActive = true };

        private static Guid G(string prefix, int n) => new($"{prefix}-0000-4000-8000-{n:D12}");

        private void SeedSubModuleData(ModelBuilder mb)
        {
            // ---------------- Module 1 — Recruitment ----------------
            mb.Entity<RequisitionStatusLookup>().HasData(
                L<RequisitionStatusLookup>(1, "DRAFT", "Draft"), L<RequisitionStatusLookup>(2, "PENDING", "Pending Approval"),
                L<RequisitionStatusLookup>(3, "APPROVED", "Approved"), L<RequisitionStatusLookup>(4, "REJECTED", "Rejected"),
                L<RequisitionStatusLookup>(5, "ONHOLD", "On Hold"), L<RequisitionStatusLookup>(6, "CLOSED", "Closed"),
                L<RequisitionStatusLookup>(7, "FILLED", "Filled"));
            mb.Entity<JobPostingStatusLookup>().HasData(
                L<JobPostingStatusLookup>(1, "DRAFT", "Draft"), L<JobPostingStatusLookup>(2, "PUBLISHED", "Published"),
                L<JobPostingStatusLookup>(3, "PAUSED", "Paused"), L<JobPostingStatusLookup>(4, "CLOSED", "Closed"),
                L<JobPostingStatusLookup>(5, "CANCELLED", "Cancelled"));
            mb.Entity<ApplicationStageLookup>().HasData(
                L<ApplicationStageLookup>(1, "APPLIED", "Applied"), L<ApplicationStageLookup>(2, "SCREENING", "Screening"),
                L<ApplicationStageLookup>(3, "SHORTLISTED", "Shortlisted"), L<ApplicationStageLookup>(4, "INTERVIEW", "Interviewing"),
                L<ApplicationStageLookup>(5, "OFFER", "Offer"), L<ApplicationStageLookup>(6, "HIRED", "Hired"),
                L<ApplicationStageLookup>(7, "REJECTED", "Rejected"), L<ApplicationStageLookup>(8, "WITHDRAWN", "Withdrawn"));
            mb.Entity<InterviewModeLookup>().HasData(
                L<InterviewModeLookup>(1, "INPERSON", "In Person"), L<InterviewModeLookup>(2, "VIDEO", "Video Call"),
                L<InterviewModeLookup>(3, "PHONE", "Phone"), L<InterviewModeLookup>(4, "TEST", "Assessment Test"));
            mb.Entity<OfferStatusLookup>().HasData(
                L<OfferStatusLookup>(1, "DRAFT", "Draft"), L<OfferStatusLookup>(2, "ISSUED", "Issued"),
                L<OfferStatusLookup>(3, "ACCEPTED", "Accepted"), L<OfferStatusLookup>(4, "DECLINED", "Declined"),
                L<OfferStatusLookup>(5, "EXPIRED", "Expired"), L<OfferStatusLookup>(6, "WITHDRAWN", "Withdrawn"));

            // ---------------- Module 2 — Onboarding ----------------
            mb.Entity<OnboardingStatusLookup>().HasData(
                L<OnboardingStatusLookup>(1, "INITIATED", "Initiated"), L<OnboardingStatusLookup>(2, "INPROGRESS", "In Progress"),
                L<OnboardingStatusLookup>(3, "COMPLETED", "Completed"), L<OnboardingStatusLookup>(4, "CANCELLED", "Cancelled"));
            mb.Entity<OnboardingTaskStatusLookup>().HasData(
                L<OnboardingTaskStatusLookup>(1, "PENDING", "Pending"), L<OnboardingTaskStatusLookup>(2, "INPROGRESS", "In Progress"),
                L<OnboardingTaskStatusLookup>(3, "DONE", "Done"), L<OnboardingTaskStatusLookup>(4, "NA", "Not Applicable"));

            // ---------------- Module 4 — Time & Attendance ----------------
            mb.Entity<OvertimeStatusLookup>().HasData(
                L<OvertimeStatusLookup>(1, "PENDING", "Pending"), L<OvertimeStatusLookup>(2, "APPROVED", "Approved"),
                L<OvertimeStatusLookup>(3, "REJECTED", "Rejected"), L<OvertimeStatusLookup>(4, "CANCELLED", "Cancelled"));
            mb.Entity<RegularizationStatusLookup>().HasData(
                L<RegularizationStatusLookup>(1, "PENDING", "Pending"), L<RegularizationStatusLookup>(2, "APPROVED", "Approved"),
                L<RegularizationStatusLookup>(3, "REJECTED", "Rejected"), L<RegularizationStatusLookup>(4, "CANCELLED", "Cancelled"));
            mb.Entity<RegularizationTypeLookup>().HasData(
                L<RegularizationTypeLookup>(1, "MISSED_IN", "Missed Check-In"), L<RegularizationTypeLookup>(2, "MISSED_OUT", "Missed Check-Out"),
                L<RegularizationTypeLookup>(3, "WRONG_TIME", "Wrong Punch Time"), L<RegularizationTypeLookup>(4, "FORGOT", "Forgot to Punch"),
                L<RegularizationTypeLookup>(5, "ON_DUTY", "On Duty / Field Work"));

            mb.Entity<Shift>().HasData(
                new Shift { Id = G("05F10000", 1), ShiftCode = "SH-0001", Name = "General Shift", StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(17, 30, 0),
                            IsNightShift = false, BreakMinutes = 60, GraceInMinutes = 15, GraceOutMinutes = 10, HalfDayHours = 4, FullDayHours = 8,
                            WeekOffDays = "5,6", Description = "UAE standard office hours, Fri–Sat weekend", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new Shift { Id = G("05F10000", 2), ShiftCode = "SH-0002", Name = "Ramadan Shift", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(15, 0, 0),
                            IsNightShift = false, BreakMinutes = 0, GraceInMinutes = 15, GraceOutMinutes = 10, HalfDayHours = 3, FullDayHours = 6,
                            WeekOffDays = "5,6", Description = "Reduced hours during Ramadan (UAE Labour Law)", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true },
                new Shift { Id = G("05F10000", 3), ShiftCode = "SH-0003", Name = "Night Shift", StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(6, 0, 0),
                            IsNightShift = true, BreakMinutes = 30, GraceInMinutes = 15, GraceOutMinutes = 10, HalfDayHours = 4, FullDayHours = 7.5m,
                            WeekOffDays = "5", Description = "Operations night cover", CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true });

            // ---------------- Module 5 — Leave ----------------
            mb.Entity<LeaveApplicationStatusLookup>().HasData(
                L<LeaveApplicationStatusLookup>(1, "PENDING", "Pending"), L<LeaveApplicationStatusLookup>(2, "APPROVED", "Approved"),
                L<LeaveApplicationStatusLookup>(3, "REJECTED", "Rejected"), L<LeaveApplicationStatusLookup>(4, "CANCELLED", "Cancelled"),
                L<LeaveApplicationStatusLookup>(5, "WITHDRAWN", "Withdrawn"));
            mb.Entity<LeaveTransactionTypeLookup>().HasData(
                L<LeaveTransactionTypeLookup>(1, "GRANT", "Annual Grant"), L<LeaveTransactionTypeLookup>(2, "ACCRUAL", "Monthly Accrual"),
                L<LeaveTransactionTypeLookup>(3, "CARRY_FORWARD", "Carry Forward"), L<LeaveTransactionTypeLookup>(4, "DEDUCTION", "Deduction (Approved Leave)"),
                L<LeaveTransactionTypeLookup>(5, "REVERSAL", "Reversal (Cancelled Leave)"), L<LeaveTransactionTypeLookup>(6, "ADJUSTMENT", "Manual Adjustment"),
                L<LeaveTransactionTypeLookup>(7, "ENCASHMENT", "Encashment"));

            LeaveType Lt(int n, string code, string name, bool paid, bool half, bool attach, bool cf, decimal? maxCf, decimal? accrual, string color, string desc) => new()
            {
                Id = G("01EA0000", n), LeaveTypeCode = code, Name = name, IsPaid = paid, AllowHalfDay = half,
                RequiresAttachment = attach, IsCarryForward = cf, MaxCarryForward = maxCf, AccrualPerMonth = accrual,
                ColorHex = color, Description = desc, CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            };
            mb.Entity<LeaveType>().HasData(
                Lt(1, "ANNUAL",    "Annual Leave",    true,  true,  false, true,  10, 2.5m, "#28a745", "30 calendar days/year after 1 year of service (UAE)"),
                Lt(2, "SICK",      "Sick Leave",      true,  true,  true,  false, null, null, "#dc3545", "Medical certificate required"),
                Lt(3, "CASUAL",    "Casual Leave",    true,  true,  false, false, null, null, "#17a2b8", "Short personal absence"),
                Lt(4, "UNPAID",    "Unpaid Leave",    false, true,  false, false, null, null, "#6c757d", "Leave without pay"),
                Lt(5, "MATERNITY", "Maternity Leave", true,  false, true,  false, null, null, "#e83e8c", "60 days (UAE Labour Law)"),
                Lt(6, "PATERNITY", "Paternity Leave", true,  false, false, false, null, null, "#fd7e14", "5 working days (UAE Labour Law)"),
                Lt(7, "HAJJ",      "Hajj Leave",      false, false, false, false, null, null, "#6f42c1", "Once during service"));

            // ---------------- Module 6 — Payroll ----------------
            mb.Entity<PayComponentTypeLookup>().HasData(
                L<PayComponentTypeLookup>(1, "EARNING", "Earning"), L<PayComponentTypeLookup>(2, "DEDUCTION", "Deduction"));
            mb.Entity<PayrollRunStatusLookup>().HasData(
                L<PayrollRunStatusLookup>(1, "DRAFT", "Draft"), L<PayrollRunStatusLookup>(2, "PROCESSED", "Processed"),
                L<PayrollRunStatusLookup>(3, "APPROVED", "Approved"), L<PayrollRunStatusLookup>(4, "PAID", "Paid"),
                L<PayrollRunStatusLookup>(5, "CANCELLED", "Cancelled"));
            mb.Entity<LoanStatusLookup>().HasData(
                L<LoanStatusLookup>(1, "ACTIVE", "Active"), L<LoanStatusLookup>(2, "CLOSED", "Closed"), L<LoanStatusLookup>(3, "CANCELLED", "Cancelled"));

            PayComponent Pc(int n, string code, string name, int type, int calc, decimal val, bool tax, bool prorate, string desc) => new()
            {
                Id = G("0FA10000", n), ComponentCode = code, Name = name, ComponentTypeId = type, CalcMethod = calc, DefaultValue = val,
                IsTaxable = tax, IsProrated = prorate, SortOrder = n, Description = desc, CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            };
            mb.Entity<PayComponent>().HasData(
                Pc(1, "HRA",     "Housing Allowance",        1, 2, 25, false, true,  "% of basic"),
                Pc(2, "TRANS",   "Transport Allowance",      1, 1, 800, false, true, "Fixed monthly"),
                Pc(3, "PHONE",   "Telephone Allowance",      1, 1, 200, false, true, "Fixed monthly"),
                Pc(4, "OTHER",   "Other Allowance",          1, 2, 10, false, true,  "% of basic"),
                Pc(5, "GPSSA",   "Pension (GPSSA) — UAE nationals", 2, 2, 11, false, false, "Employee share, UAE/GCC nationals only"),
                Pc(6, "LOAN",    "Loan / Advance Recovery",  2, 1, 0, false, false,  "Driven by Loans module"));

            // ---------------- Module 7 — Performance ----------------
            mb.Entity<AppraisalCycleStatusLookup>().HasData(
                L<AppraisalCycleStatusLookup>(1, "DRAFT", "Draft"), L<AppraisalCycleStatusLookup>(2, "ACTIVE", "Active"),
                L<AppraisalCycleStatusLookup>(3, "CLOSED", "Closed"));
            mb.Entity<GoalStatusLookup>().HasData(
                L<GoalStatusLookup>(1, "DRAFT", "Draft"), L<GoalStatusLookup>(2, "ACTIVE", "Active"),
                L<GoalStatusLookup>(3, "COMPLETED", "Completed"), L<GoalStatusLookup>(4, "CANCELLED", "Cancelled"));
            mb.Entity<ReviewStatusLookup>().HasData(
                L<ReviewStatusLookup>(1, "NOT_STARTED", "Not Started"), L<ReviewStatusLookup>(2, "SELF_ASSESSMENT", "Self Assessment"),
                L<ReviewStatusLookup>(3, "MANAGER_REVIEW", "Manager Review"), L<ReviewStatusLookup>(4, "COMPLETED", "Completed"),
                L<ReviewStatusLookup>(5, "ACKNOWLEDGED", "Acknowledged"));
            mb.Entity<RatingItemTypeLookup>().HasData(
                L<RatingItemTypeLookup>(1, "GOAL", "Goal"), L<RatingItemTypeLookup>(2, "COMPETENCY", "Competency"));

            Competency Cp(int n, string code, string name, string desc) => new()
            {
                Id = G("0C0E0000", n), CompetencyCode = code, Name = name, Description = desc, SortOrder = n,
                CreatedBy = SeedCreator, CreatedDate = SeedTime, IsActive = true
            };
            mb.Entity<Competency>().HasData(
                Cp(1, "COMM", "Communication", "Clarity, listening, and stakeholder updates"),
                Cp(2, "OWN",  "Ownership",     "Accountability and follow-through"),
                Cp(3, "TEAM", "Teamwork",      "Collaboration and supporting peers"),
                Cp(4, "QUAL", "Quality",       "Accuracy and attention to detail"),
                Cp(5, "INNO", "Innovation",    "Initiative and process improvement"),
                Cp(6, "CUST", "Customer Focus", "Understands and serves internal and external customers"));

            // ---------------- Module 8 — Separation ----------------
            mb.Entity<SeparationTypeLookup>().HasData(
                L<SeparationTypeLookup>(1, "RESIGNATION", "Resignation"), L<SeparationTypeLookup>(2, "TERMINATION", "Termination"),
                L<SeparationTypeLookup>(3, "RETIREMENT", "Retirement"), L<SeparationTypeLookup>(4, "END_OF_CONTRACT", "End of Contract"),
                L<SeparationTypeLookup>(5, "DEATH", "Death"));
            mb.Entity<SeparationStatusLookup>().HasData(
                L<SeparationStatusLookup>(1, "DRAFT", "Draft"), L<SeparationStatusLookup>(2, "PENDING", "Pending Approval"),
                L<SeparationStatusLookup>(3, "APPROVED", "Approved"), L<SeparationStatusLookup>(4, "IN_CLEARANCE", "In Clearance"),
                L<SeparationStatusLookup>(5, "SETTLED", "Settled"), L<SeparationStatusLookup>(6, "COMPLETED", "Completed"),
                L<SeparationStatusLookup>(7, "CANCELLED", "Cancelled"));
            mb.Entity<ClearanceStatusLookup>().HasData(
                L<ClearanceStatusLookup>(1, "PENDING", "Pending"), L<ClearanceStatusLookup>(2, "CLEARED", "Cleared"),
                L<ClearanceStatusLookup>(3, "BLOCKED", "Blocked"));
            mb.Entity<ClearanceDepartmentLookup>().HasData(
                L<ClearanceDepartmentLookup>(1, "IT", "IT / Systems"), L<ClearanceDepartmentLookup>(2, "FINANCE", "Finance / Accounts"),
                L<ClearanceDepartmentLookup>(3, "HR", "Human Resources"), L<ClearanceDepartmentLookup>(4, "ADMIN", "Admin / Facilities"),
                L<ClearanceDepartmentLookup>(5, "MANAGER", "Reporting Manager"));
        }
        #endregion

        #region Separation (was HumanResourcesDBContext.Separation.cs)
        // Separation (offboarding) slice of HumanResourcesDBContext. Call
        // ConfigureSeparation(mb) at the end of OnModelCreating, after
        // ConfigurePerformance(mb). IntegrationOutbox is owned by the Onboarding
        // slice — not declared here.
        public virtual DbSet<SeparationTypeLookup>?      SeparationTypes      { get; set; }
        public virtual DbSet<SeparationStatusLookup>?    SeparationStatuses   { get; set; }
        public virtual DbSet<ClearanceStatusLookup>?     ClearanceStatuses    { get; set; }
        public virtual DbSet<ClearanceDepartmentLookup>? ClearanceDepartments { get; set; }

        public virtual DbSet<Separation>?    Separations    { get; set; }
        public virtual DbSet<SeparationLog>? SeparationsLog { get; set; }
        public virtual DbSet<SeparationClearance>?    SeparationClearances    { get; set; }
        public virtual DbSet<SeparationClearanceLog>? SeparationClearancesLog { get; set; }
        public virtual DbSet<ExitInterview>?    ExitInterviews    { get; set; }
        public virtual DbSet<ExitInterviewLog>? ExitInterviewsLog { get; set; }

        private void ConfigureSeparation(ModelBuilder mb)
        {
            mb.Entity<SeparationTypeLookup>(e => { e.ToTable("SeparationTypes"); e.HasKey(x => x.Id); });
            mb.Entity<SeparationStatusLookup>(e => { e.ToTable("SeparationStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<ClearanceStatusLookup>(e => { e.ToTable("ClearanceStatuses"); e.HasKey(x => x.Id); });
            mb.Entity<ClearanceDepartmentLookup>(e => { e.ToTable("ClearanceDepartments"); e.HasKey(x => x.Id); });

            mb.Entity<Separation>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.SeparationCode).HasMaxLength(30).IsRequired();
                e.HasIndex(x => x.SeparationCode).IsUnique().HasDatabaseName("UX_Separations_Code");
                e.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.Clearances).WithOne().HasForeignKey(x => x.SeparationId).OnDelete(DeleteBehavior.Cascade);
                e.Property(x => x.NoticeDate).HasColumnType("datetime");
                e.Property(x => x.LastWorkingDate).HasColumnType("datetime");
                e.Property(x => x.RequestedLastWorkingDate).HasColumnType("datetime");
                e.ToTable(t => { t.HasTrigger("TRG_InsertSeparations"); t.HasTrigger("TRG_UpdateSeparations"); });
            });
            mb.Entity<SeparationLog>(e => e.HasKey(x => x.Id));

            mb.Entity<SeparationClearance>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.SeparationId, x.DepartmentId }).IsUnique().HasDatabaseName("UX_Clearances_Sep_Dept");
                e.Property(x => x.DuesAmount).HasColumnType("decimal(18,2)");
                e.ToTable(t => { t.HasTrigger("TRG_InsertSeparationClearances"); t.HasTrigger("TRG_UpdateSeparationClearances"); });
            });
            mb.Entity<SeparationClearanceLog>(e => e.HasKey(x => x.Id));

            mb.Entity<ExitInterview>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne<Separation>().WithMany().HasForeignKey(x => x.SeparationId).OnDelete(DeleteBehavior.NoAction);
                e.Property(x => x.InterviewDate).HasColumnType("datetime");
                e.ToTable(t => { t.HasTrigger("TRG_InsertExitInterviews"); t.HasTrigger("TRG_UpdateExitInterviews"); });
            });
            mb.Entity<ExitInterviewLog>(e => e.HasKey(x => x.Id));
        }
        #endregion

    }
}
