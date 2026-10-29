using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class CostCenterRequest
    {
        public string? ActionName { get; set; }   // Save / Update
        public string? Id { get; set; }
        [Required] public string? Name { get; set; }
        public string? CompanyId { get; set; }
        public string? BranchId { get; set; }
        public string? ParentCostCenterId { get; set; }
        public string? OwnerEmployeeId { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class JobGradeRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? Name { get; set; }
        [Range(1, 100)] public int GradeLevel { get; set; }
        public string? Band { get; set; }
        public decimal? RefMinSalary { get; set; }
        public decimal? RefMidSalary { get; set; }
        public decimal? RefMaxSalary { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class HolidayCalendarRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? Name { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class HolidayDateRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? HolidayCalendarId { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public DateTime HolidayDate { get; set; }
        public int HolidayTypeId { get; set; } = 1;
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class PositionFillRequest
    {
        [Required] public string? PositionId { get; set; }
        public int Delta { get; set; }             // +1 assign / -1 release
        public string? CreateUpdateBy { get; set; }
    }

    public class PositionStatusRequest
    {
        [Required] public string? Id { get; set; }
        [Range(1, 5)] public int StatusId { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class CostCenterResponse
    {
        public Guid Id { get; set; }
        public string? CostCenterCode { get; set; }
        public string? Name { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid? ParentCostCenterId { get; set; }
        public string? ParentName { get; set; }
        public Guid? OwnerEmployeeId { get; set; }
        public string? OwnerName { get; set; }
        public string? Description { get; set; }
        public int EmployeeCount { get; set; }
        public bool? IsActive { get; set; }
    }

    public class JobGradeResponse
    {
        public Guid Id { get; set; }
        public string? GradeCode { get; set; }
        public string? Name { get; set; }
        public int GradeLevel { get; set; }
        public string? Band { get; set; }
        public decimal? RefMinSalary { get; set; }
        public decimal? RefMidSalary { get; set; }
        public decimal? RefMaxSalary { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class HolidayCalendarResponse
    {
        public Guid Id { get; set; }
        public string? CalendarCode { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public int HolidayCount { get; set; }
        public bool? IsActive { get; set; }
    }

    public class HolidayDateResponse
    {
        public Guid Id { get; set; }
        public Guid HolidayCalendarId { get; set; }
        public string? Name { get; set; }
        public DateTime HolidayDate { get; set; }
        public int HolidayTypeId { get; set; }
        public string? HolidayTypeName { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
    }
}
