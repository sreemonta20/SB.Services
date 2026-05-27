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

        protected override void OnModelCreating(ModelBuilder mb)
        {
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

            mb.Entity<EmployeeLog>(e => e.HasKey(x => x.Id));

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
            const string seedCreator = "C047D662-9F0E-4358-B323-15EC3081312C";

            // --- Departments ---
            mb.Entity<Department>().HasData(
                new Department { Id = new Guid("A1A1A1A1-1111-4111-8111-111111111111"), DepartmentCode = "ADMIN", Name = "Administration",  Description = "Administrative and executive office",   CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("A2A2A2A2-2222-4222-8222-222222222222"), DepartmentCode = "HR",    Name = "Human Resources", Description = "People operations and talent management", CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("A3A3A3A3-3333-4333-8333-333333333333"), DepartmentCode = "ENG",   Name = "Engineering",     Description = "Software engineering and development",   CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("A4A4A4A4-4444-4444-8444-444444444444"), DepartmentCode = "FIN",   Name = "Finance",         Description = "Accounting, payroll and treasury",       CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Department { Id = new Guid("A5A5A5A5-5555-4555-8555-555555555555"), DepartmentCode = "OPS",   Name = "Operations",      Description = "Day-to-day business operations",          CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true }
            );

            // --- Designations ---
            mb.Entity<Designation>().HasData(
                new Designation { Id = new Guid("D1111111-1111-4111-8111-111111111111"), Name = "Chief Executive Officer", Code = "CEO", Grade = 1, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D2222222-2222-4222-8222-222222222222"), Name = "Manager",                  Code = "MGR", Grade = 4, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D3333333-3333-4333-8333-333333333333"), Name = "Team Lead",                Code = "TL",  Grade = 5, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D4444444-4444-4444-8444-444444444444"), Name = "Senior Engineer",          Code = "SE",  Grade = 6, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D5555555-5555-4555-8555-555555555555"), Name = "Software Engineer",        Code = "SWE", Grade = 7, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D6666666-6666-4666-8666-666666666666"), Name = "HR Manager",               Code = "HRM", Grade = 4, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D7777777-7777-4777-8777-777777777777"), Name = "Accountant",               Code = "ACC", Grade = 6, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true },
                new Designation { Id = new Guid("D8888888-8888-4888-8888-888888888888"), Name = "Intern",                   Code = "INT", Grade = 9, CreatedBy = seedCreator, CreatedDate = seedTime, IsActive = true }
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
                    CreatedBy = seedCreator,
                    CreatedDate = seedTime,
                    IsActive = true
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
        }
    }
}
