using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Root Employee record — personal info only. Address, education,
    /// experience, skills, training, certs, documents, bank, and emergency
    /// contacts live in their own tables (1-to-many) so this row stays narrow
    /// and the audit log doesn't balloon.
    ///
    /// <para><b>AppUserId</b> is a soft reference to SBERP.Security.AppUsers.Id
    /// — kept as a plain Guid because the two databases are separate
    /// (microservice boundary). No EF FK is defined for it.</para>
    /// </summary>
    public class Employee
    {
        [Key] public Guid Id { get; set; }

        // === Identity / lookup ===
        public string? EmployeeCode { get; set; }        // unique business key, e.g. EMP-0001
        public Guid? AppUserId { get; set; }             // soft FK to SecurityDB.AppUsers.Id
        public string? OfficialEmail { get; set; }       // unique

        // === Names ===
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }            // computed at write-time

        // === Demographics ===
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }                 // EnumGender
        public int? MaritalStatus { get; set; }          // EnumMaritalStatus
        public int? BloodGroup { get; set; }             // EnumBloodGroup
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? NationalId { get; set; }          // e.g. Emirates ID
        public string? PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }

        // === Contact ===
        public string? PersonalEmail { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }

        // === Employment ===
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? ReportingManagerId { get; set; }    // self-referencing
        public DateTime? DateOfJoining { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public int? EmploymentType { get; set; }         // EnumEmploymentType
        public int? EmploymentStatus { get; set; }       // EnumEmploymentStatus
        public string? WorkLocation { get; set; }

        // === Compensation (high-level only — payroll service holds details) ===
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BasicSalary { get; set; }
        public string? SalaryCurrency { get; set; }      // ISO 4217, e.g. AED

        // === Photo / signature ===
        public string? PhotoUrl { get; set; }
        public string? SignatureUrl { get; set; }

        // === Audit ===
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        // === Navigation ===
        public virtual Department? Department { get; set; }
        public virtual Designation? Designation { get; set; }
        public virtual Employee? ReportingManager { get; set; }
        public virtual ICollection<Employee>? DirectReports { get; set; }

        public virtual ICollection<EmployeeAddress>? Addresses { get; set; }
        public virtual ICollection<EmployeeEducation>? Educations { get; set; }
        public virtual ICollection<EmployeeExperience>? Experiences { get; set; }
        public virtual ICollection<EmployeeSkill>? Skills { get; set; }
        public virtual ICollection<EmployeeTraining>? Trainings { get; set; }
        public virtual ICollection<EmployeeCertification>? Certifications { get; set; }
        public virtual ICollection<EmployeeDocument>? Documents { get; set; }
        public virtual ICollection<EmployeeEmergencyContact>? EmergencyContacts { get; set; }
        public virtual EmployeeBank? BankInfo { get; set; }
    }

    public class EmployeeLog
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public int? EmploymentStatus { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
