using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class SeparationRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public int SeparationTypeId { get; set; }
        public string? Reason { get; set; }
        public DateTime? NoticeDate { get; set; }
        public DateTime? RequestedLastWorkingDate { get; set; }
        public int? NoticePeriodDays { get; set; }
        public bool IsNoticeWaived { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class SeparationStatusRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }   // 2 submit / 6 complete / 7 cancel
        public string? CreateUpdateBy { get; set; }
    }

    public class ApproveSeparationRequest
    {
        [Required] public string? Id { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class SettleSeparationRequest
    {
        [Required] public string? Id { get; set; }
        public string? SettlementRemarks { get; set; }
        public bool? IsRehireEligible { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ClearanceStatusRequest
    {
        [Required] public string? Id { get; set; }            // clearance line id
        public int ClearanceStatusId { get; set; }            // 1 Pending / 2 Cleared / 3 Blocked
        public string? Remarks { get; set; }
        public decimal? DuesAmount { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ExitInterviewRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? SeparationId { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string? InterviewerId { get; set; }
        public string? PrimaryReason { get; set; }
        public int? OverallExperienceRating { get; set; }
        public bool? WouldRecommend { get; set; }
        public string? Feedback { get; set; }
        public string? Suggestions { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class SeparationResponse
    {
        public Guid Id { get; set; }
        public string? SeparationCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public int SeparationTypeId { get; set; }
        public string? SeparationTypeName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime? NoticeDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public bool ExitInterviewDone { get; set; }
        public DateTime? SettlementDate { get; set; }
        public int ClearanceTotal { get; set; }
        public int ClearanceCleared { get; set; }
        public bool? IsActive { get; set; }
    }
}
