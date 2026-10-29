using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    // =========================================================================
    // Organization Setup (Module 0 foundation) — the parts NOT already covered
    // by Company / Branch (hr2). Position lives in RecruitmentEntities.cs (one
    // shared entity for Org Setup + Recruitment + Onboarding + Separation).
    // Lookup Ids mirror EnumPositionStatus / EnumHolidayType.
    // =========================================================================
    public class PositionStatusLookup : BaseLookup { }
    public class HolidayTypeLookup    : BaseLookup { }

    /// <summary>Budget / accounting unit. Optional self-parent for a hierarchy.</summary>
    public class CostCenter
    {
        [Key] public Guid Id { get; set; }
        public string? CostCenterCode { get; set; }
        public string? Name { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ParentCostCenterId { get; set; }
        public Guid? OwnerEmployeeId { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>Organizational seniority ladder — distinct from pay (Payroll owns actual pay).</summary>
    public class JobGrade
    {
        [Key] public Guid Id { get; set; }
        public string? GradeCode { get; set; }
        public string? Name { get; set; }
        public int GradeLevel { get; set; }
        public string? Band { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? RefMinSalary { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? RefMidSalary { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? RefMaxSalary { get; set; }
        public string? CurrencyCode { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>Named calendar (e.g. "UAE 2026"). Branches point at one; Leave and
    /// Attendance use dbo.fn_IsHoliday against it.</summary>
    public class HolidayCalendar
    {
        [Key] public Guid Id { get; set; }
        public string? CalendarCode { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public virtual ICollection<HolidayDate>? HolidayDates { get; set; }
    }

    public class HolidayDate
    {
        [Key] public Guid Id { get; set; }
        public Guid HolidayCalendarId { get; set; }
        public string? Name { get; set; }
        public DateTime HolidayDate1 { get; set; }   // column "HolidayDate" (property can't share the class name)
        public int HolidayTypeId { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public virtual HolidayCalendar? Calendar { get; set; }
    }

    // ---- audit log entities (rows written by TRG_* triggers) ----
    public class CostCenterLog
    {
        [Key] public Guid Id { get; set; }
        public Guid CostCenterId { get; set; }
        public string? CostCenterCode { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class JobGradeLog
    {
        [Key] public Guid Id { get; set; }
        public Guid JobGradeId { get; set; }
        public string? GradeCode { get; set; }
        public string? Name { get; set; }
        public int? GradeLevel { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class HolidayCalendarLog
    {
        [Key] public Guid Id { get; set; }
        public Guid HolidayCalendarId { get; set; }
        public string? CalendarCode { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
