using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    /// <summary>
    /// One create/update payload — ActionName decides Save vs Update so the
    /// controller surface stays small. Sub-collections are full replacements:
    /// what the client sends becomes the new state.
    /// </summary>
    public class EmployeeRequest
    {
        public string? ActionName { get; set; }   // "Save" or "Update"
        public string? Id { get; set; }

        [Required] public string? EmployeeCode { get; set; }
        public string? AppUserId { get; set; }

        [Required] public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required] public string? LastName { get; set; }

        [EmailAddress] public string? OfficialEmail { get; set; }
        [EmailAddress] public string? PersonalEmail { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public int? MaritalStatus { get; set; }
        public int? BloodGroup { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }

        public string? MobileNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }

        public string? DepartmentId { get; set; }
        public string? DesignationId { get; set; }
        public string? ReportingManagerId { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? DateOfLeaving { get; set; }
        public int? EmploymentType { get; set; }
        public int? EmploymentStatus { get; set; }
        public string? WorkLocation { get; set; }

        public decimal? BasicSalary { get; set; }
        public string? SalaryCurrency { get; set; }

        public string? PhotoUrl { get; set; }
        public string? SignatureUrl { get; set; }

        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }

        public List<EmployeeAddressRequest>? Addresses { get; set; }
        public List<EmployeeEducationRequest>? Educations { get; set; }
        public List<EmployeeExperienceRequest>? Experiences { get; set; }
        public List<EmployeeSkillRequest>? Skills { get; set; }
        public List<EmployeeTrainingRequest>? Trainings { get; set; }
        public List<EmployeeCertificationRequest>? Certifications { get; set; }
        public List<EmployeeDocumentRequest>? Documents { get; set; }
        public List<EmployeeEmergencyContactRequest>? EmergencyContacts { get; set; }
        public EmployeeBankRequest? BankInfo { get; set; }
    }

    public class EmployeeAddressRequest
    {
        public string? Id { get; set; }
        public int AddressType { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeEducationRequest
    {
        public string? Id { get; set; }
        public string? Qualification { get; set; }
        public string? Institution { get; set; }
        public string? University { get; set; }
        public string? Specialization { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public decimal? Grade { get; set; }
        public string? DocumentUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeExperienceRequest
    {
        public string? Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Responsibilities { get; set; }
        public string? Location { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeSkillRequest
    {
        public string? Id { get; set; }
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeTrainingRequest
    {
        public string? Id { get; set; }
        public string? TrainingName { get; set; }
        public string? Provider { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Outcome { get; set; }
        public string? CertificateUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeCertificationRequest
    {
        public string? Id { get; set; }
        public string? CertificationName { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? CertificationNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeDocumentRequest
    {
        public string? Id { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeEmergencyContactRequest
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Relationship { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? AlternatePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeBankRequest
    {
        public string? Id { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IbanNumber { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public string? Currency { get; set; }
        public bool? IsActive { get; set; }
    }
}
