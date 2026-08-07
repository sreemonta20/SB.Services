using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Physical/legal work location under a Company. Department and Employee
    /// carry a real FK + navigation to this, same DB.
    /// </summary>
    public class Branch
    {
        [Key] public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? BranchCode { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? IsHeadOffice { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Department>? Departments { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
    }

    public class BranchLog
    {
        [Key] public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid CompanyId { get; set; }
        public string? BranchCode { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool? IsHeadOffice { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
