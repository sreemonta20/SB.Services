using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class BranchRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? CompanyId { get; set; }
        [Required] public string? BranchCode { get; set; }
        [Required] public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? IsHeadOffice { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
