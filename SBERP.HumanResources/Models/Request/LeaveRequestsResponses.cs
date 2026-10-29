using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class LeaveTypeRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? LeaveTypeCode { get; set; }
        [Required] public string? Name { get; set; }
        public bool IsPaid { get; set; } = true;
        public bool AllowHalfDay { get; set; } = true;
        public bool RequiresAttachment { get; set; }
        public bool IsCarryForward { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public decimal? AccrualPerMonth { get; set; }
        public string? ColorHex { get; set; }
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class LeavePolicyRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? PolicyCode { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public string? LeaveTypeId { get; set; }
        public string? DepartmentId { get; set; }
        public int? Grade { get; set; }
        [Required] public decimal AnnualEntitlement { get; set; }
        public int? MinServiceMonths { get; set; }
        public int? MaxConsecutiveDays { get; set; }
        [Required] public int EffectiveYear { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ApplyLeaveRequest
    {
        [Required] public string? EmployeeId { get; set; }
        [Required] public string? LeaveTypeId { get; set; }
        [Required] public DateTime FromDate { get; set; }
        [Required] public DateTime ToDate { get; set; }
        public bool IsHalfDay { get; set; }
        public string? Reason { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? ContactDuringLeave { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class LeaveActionRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }            // 2 Approved / 3 Rejected / 4 Cancelled / 5 Withdrawn
        public string? ApproverId { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class GrantEntitlementsRequest
    {
        [Required] public int Year { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class LeaveTypeResponse
    {
        public Guid Id { get; set; }
        public string? LeaveTypeCode { get; set; }
        public string? Name { get; set; }
        public bool IsPaid { get; set; }
        public bool AllowHalfDay { get; set; }
        public bool RequiresAttachment { get; set; }
        public bool IsCarryForward { get; set; }
        public decimal? MaxCarryForward { get; set; }
        public decimal? AccrualPerMonth { get; set; }
        public string? ColorHex { get; set; }
        public bool? IsActive { get; set; }
    }

    public class LeavePolicyResponse
    {
        public Guid Id { get; set; }
        public string? PolicyCode { get; set; }
        public string? Name { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? Grade { get; set; }
        public decimal AnnualEntitlement { get; set; }
        public int? MinServiceMonths { get; set; }
        public int? MaxConsecutiveDays { get; set; }
        public int EffectiveYear { get; set; }
        public bool? IsActive { get; set; }
    }

    public class LeaveApplicationResponse
    {
        public Guid Id { get; set; }
        public string? ApplicationCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? ColorHex { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsHalfDay { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? ApproverRemarks { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int Year { get; set; }
    }

    public class LeaveBalanceResponse
    {
        public Guid Id { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? ColorHex { get; set; }
        public int Year { get; set; }
        public decimal Entitled { get; set; }
        public decimal CarriedForward { get; set; }
        public decimal Used { get; set; }
        public decimal Pending { get; set; }
        public decimal Available { get; set; }
    }
}
