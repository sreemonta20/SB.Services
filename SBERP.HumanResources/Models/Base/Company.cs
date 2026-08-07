using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Module 0 — root tenant/legal-entity record. Lives in HumanResourcesDB,
    /// same as Department/Employee (NOT a separate microservice/database).
    /// Department and Employee carry a real EF FK + navigation to this,
    /// same as they already do to each other — unlike Employee.AppUserId,
    /// which stays a soft reference because SecurityDB genuinely is a
    /// separate microservice.
    /// </summary>
    public class Company
    {
        [Key] public Guid Id { get; set; }
        public string? CompanyCode { get; set; }        // unique short code, e.g. SBERP
        public string? Name { get; set; }
        public string? LegalName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }           // VAT/TRN
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? CurrencyCode { get; set; }        // ISO 4217, e.g. AED
        public int? FinancialYearStartMonth { get; set; } // 1-12

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Branch>? Branches { get; set; }
        public virtual ICollection<Department>? Departments { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
    }

    public class CompanyLog
    {
        [Key] public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyCode { get; set; }
        public string? Name { get; set; }
        public string? LegalName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? CurrencyCode { get; set; }
        public int? FinancialYearStartMonth { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
