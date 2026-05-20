using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// One employee can have multiple addresses — Permanent, Current, Office.
    /// AddressType: 1=Permanent, 2=Current, 3=Office, 4=Other.
    /// </summary>
    public class EmployeeAddress
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int AddressType { get; set; }       // 1 Permanent / 2 Current / 3 Office / 4 Other
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool? IsPrimary { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeEducation
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Qualification { get; set; }   // B.Tech, MBA, etc.
        public string? Institution { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Grade { get; set; }          // GPA or %
        public string? DocumentUrl { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeExperience
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }       // null = current
        public string? Responsibilities { get; set; }
        public string? Location { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeSkill
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; } // Beginner / Intermediate / Expert
        public int? YearsOfExperience { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeTraining
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? TrainingName { get; set; }
        public string? Provider { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Outcome { get; set; }
        public string? CertificateUrl { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeCertification
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? CertificationName { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? CertificationNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeDocument
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? DocumentType { get; set; }    // Passport, NID, Visa, Offer Letter, etc.
        public string? DocumentNumber { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Remark { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeEmergencyContact
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Relationship { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? AlternatePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool? IsPrimary { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeBank
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }          // 1-to-1
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanNumber { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public string? Currency { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
