using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Job title with optional grade/level. Kept separate from Department so a
    /// "Senior Engineer" can exist in either Engineering or Platform.
    /// </summary>
    public class Designation
    {
        [Key] public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }   // optional band/level

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }
    }

    public class DesignationLog
    {
        [Key] public Guid Id { get; set; }
        public Guid DesignationId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
