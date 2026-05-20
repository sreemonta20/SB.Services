using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class DepartmentRequest
    {
        public string? ActionName { get; set; }      // Save / Update
        public string? Id { get; set; }
        [Required] public string? DepartmentCode { get; set; }
        [Required] public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ParentDepartmentId { get; set; }
        public string? HeadEmployeeId { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }

    public class DesignationRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }

    public class HRSettingsRequest
    {
        public string? Id { get; set; }
        public int AttendanceSource { get; set; }
        public int? BiometricProvider { get; set; }
        public string? BiometricConnectionString { get; set; }
        public string? BiometricSourceObject { get; set; }
        public string? OfficeStartTime { get; set; }
        public string? OfficeEndTime { get; set; }
        public int? GracePeriodMinutes { get; set; }
        public string? WeeklyOffDays { get; set; }
        public int? AnnualLeaveDays { get; set; }
        public int? SickLeaveDays { get; set; }
        public int? CasualLeaveDays { get; set; }
        public string? AutoProcessTime { get; set; }
        public bool? AutoProcessEnabled { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
