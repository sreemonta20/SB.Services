using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class CompanyRequest
    {
        public string? ActionName { get; set; }      // Save / Update
        public string? Id { get; set; }
        [Required] public string? CompanyCode { get; set; }
        [Required] public string? Name { get; set; }
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
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
