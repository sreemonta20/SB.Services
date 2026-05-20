using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Top-level org unit. Supports self-referencing parent for sub-departments
    /// (e.g. Engineering → Platform → Identity). HeadEmployeeId points to the
    /// department head — nullable because seed data can't reference an employee
    /// that doesn't exist yet.
    /// </summary>
    public class Department
    {
        [Key] public Guid Id { get; set; }
        public string? DepartmentCode { get; set; }   // unique short code, e.g. ENG
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; } // null for top-level
        public Guid? HeadEmployeeId { get; set; }     // department head — nullable

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Department? ParentDepartment { get; set; }
        public virtual ICollection<Department>? ChildDepartments { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
    }

    public class DepartmentLog
    {
        [Key] public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public Guid? HeadEmployeeId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
