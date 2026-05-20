namespace SBERP.HumanResources.Models.Response
{
    /// <summary>
    /// Flat shape returned to grid/list views. The full nested view (with
    /// addresses, education, etc.) goes through EmployeeDetailResponse.
    /// </summary>
    public class EmployeeListResponse
    {
        public Guid Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public string? MobileNumber { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public int? EmploymentStatus { get; set; }
        public string? EmploymentStatusName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeDetailResponse
    {
        public Guid Id { get; set; }
        public string? EmployeeCode { get; set; }
        public Guid? AppUserId { get; set; }
        public string? OfficialEmail { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public int? MaritalStatus { get; set; }
        public int? BloodGroup { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }

        public string? PersonalEmail { get; set; }
        public string? MobileNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }

        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public Guid? ReportingManagerId { get; set; }
        public string? ReportingManagerName { get; set; }

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

        public bool? IsActive { get; set; }

        public List<EmployeeAddressDto>? Addresses { get; set; }
        public List<EmployeeEducationDto>? Educations { get; set; }
        public List<EmployeeExperienceDto>? Experiences { get; set; }
        public List<EmployeeSkillDto>? Skills { get; set; }
        public List<EmployeeTrainingDto>? Trainings { get; set; }
        public List<EmployeeCertificationDto>? Certifications { get; set; }
        public List<EmployeeDocumentDto>? Documents { get; set; }
        public List<EmployeeEmergencyContactDto>? EmergencyContacts { get; set; }
        public EmployeeBankDto? BankInfo { get; set; }
    }

    public class EmployeeAddressDto
    {
        public Guid? Id { get; set; }
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

    public class EmployeeEducationDto
    {
        public Guid? Id { get; set; }
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

    public class EmployeeExperienceDto
    {
        public Guid? Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Responsibilities { get; set; }
        public string? Location { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeSkillDto
    {
        public Guid? Id { get; set; }
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }
        public int? YearsOfExperience { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeTrainingDto
    {
        public Guid? Id { get; set; }
        public string? TrainingName { get; set; }
        public string? Provider { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Outcome { get; set; }
        public string? CertificateUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeCertificationDto
    {
        public Guid? Id { get; set; }
        public string? CertificationName { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? CertificationNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CertificateUrl { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeDocumentDto
    {
        public Guid? Id { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeEmergencyContactDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Relationship { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? AlternatePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmployeeBankDto
    {
        public Guid? Id { get; set; }
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

    /// <summary>
    /// Dropdown data for the create/edit Employee form.
    /// </summary>
    public class EmployeeInitialDataResponse
    {
        public List<LookupItem>? Departments { get; set; }
        public List<LookupItem>? Designations { get; set; }
        public List<LookupItem>? ReportingManagers { get; set; }
        public List<LookupItem>? Genders { get; set; }
        public List<LookupItem>? MaritalStatuses { get; set; }
        public List<LookupItem>? BloodGroups { get; set; }
        public List<LookupItem>? EmploymentTypes { get; set; }
        public List<LookupItem>? EmploymentStatuses { get; set; }
        public string? NextEmployeeCode { get; set; }
    }

    public class LookupItem
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
