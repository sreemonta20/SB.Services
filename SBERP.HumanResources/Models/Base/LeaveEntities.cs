using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    // Lookups — Ids mirror enums in SBERP.HumanResources.Enum.
    public class LeaveApplicationStatusLookup : BaseLookup { }
    public class LeaveTransactionTypeLookup   : BaseLookup { }

    public class LeaveType
    {
        [Key] public Guid Id { get; set; }
        public string? LeaveTypeCode { get; set; }
        public string? Name { get; set; }
        public bool IsPaid { get; set; }
        public bool AllowHalfDay { get; set; }
        public bool RequiresAttachment { get; set; }
        public bool IsCarryForward { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal? MaxCarryForward { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? AccrualPerMonth { get; set; }
        public string? ColorHex { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class LeaveTypeLog
    {
        [Key] public Guid Id { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string? LeaveTypeCode { get; set; }
        public string? Name { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class LeavePolicy
    {
        [Key] public Guid Id { get; set; }
        public string? PolicyCode { get; set; }
        public string? Name { get; set; }
        public Guid LeaveTypeId { get; set; }
        public Guid? DepartmentId { get; set; }
        public int? Grade { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal AnnualEntitlement { get; set; }
        public int? MinServiceMonths { get; set; }
        public int? MaxConsecutiveDays { get; set; }
        public int EffectiveYear { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual LeaveType? LeaveType { get; set; }
    }

    public class LeavePolicyLog
    {
        [Key] public Guid Id { get; set; }
        public Guid LeavePolicyId { get; set; }
        public string? PolicyCode { get; set; }
        public Guid? LeaveTypeId { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal? AnnualEntitlement { get; set; }
        public int? EffectiveYear { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class LeaveBalance
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "decimal(6,1)")] public decimal Entitled { get; set; }
        [Column(TypeName = "decimal(6,1)")] public decimal CarriedForward { get; set; }
        [Column(TypeName = "decimal(6,1)")] public decimal Used { get; set; }
        [Column(TypeName = "decimal(6,1)")] public decimal Pending { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual LeaveType? LeaveType { get; set; }
    }

    public class LeaveApplication
    {
        [Key] public Guid Id { get; set; }
        public string? ApplicationCode { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsHalfDay { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? ContactDuringLeave { get; set; }
        public int StatusId { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApproverRemarks { get; set; }
        public int Year { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual LeaveType? LeaveType { get; set; }
    }

    public class LeaveApplicationLog
    {
        [Key] public Guid Id { get; set; }
        public Guid LeaveApplicationId { get; set; }
        public string? ApplicationCode { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? LeaveTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        [Column(TypeName = "decimal(5,1)")] public decimal? TotalDays { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    /// <summary>Immutable ledger row. +grant/+reversal, -deduction.</summary>
    public class LeaveTransaction
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public int Year { get; set; }
        public int TransactionTypeId { get; set; }
        [Column(TypeName = "decimal(6,1)")] public decimal Days { get; set; }
        public Guid? LeaveApplicationId { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
