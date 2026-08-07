namespace SBERP.HumanResources.Models.Response
{
    public class CompanyResponse
    {
        public Guid Id { get; set; }
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
        public int? BranchCount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
