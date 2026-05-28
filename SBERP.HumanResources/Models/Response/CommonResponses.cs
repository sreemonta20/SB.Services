namespace SBERP.HumanResources.Models.Response
{
    public class DepartmentResponse
    {
        public Guid Id { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public string? ParentDepartmentName { get; set; }
        public Guid? HeadEmployeeId { get; set; }
        public string? HeadEmployeeName { get; set; }
        public int? EmployeeCount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class DesignationResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public int? EmployeeCount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class HRSettingsResponse
    {
        public Guid Id { get; set; }
        public int AttendanceSource { get; set; }
        public string? AttendanceSourceName { get; set; }
        public int? BiometricProvider { get; set; }
        public string? BiometricProviderName { get; set; }
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
        public bool? IsActive { get; set; }
        public bool? IsHardDelete { get; set; }
    }
}
