using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    public class AppraisalCycleRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? CycleCode { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public DateTime PeriodStart { get; set; }
        [Required] public DateTime PeriodEnd { get; set; }
        public DateTime? GoalDueDate { get; set; }
        public DateTime? ReviewDueDate { get; set; }
        public int RatingScaleMax { get; set; } = 5;
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class CompetencyRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? CompetencyCode { get; set; }
        [Required] public string? Name { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class GoalRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? AppraisalCycleId { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Weight { get; set; }
        public string? TargetValue { get; set; }
        public decimal ProgressPercent { get; set; }
        public int StatusId { get; set; } = 1;
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class GoalProgressRequest
    {
        [Required] public string? Id { get; set; }
        public decimal ProgressPercent { get; set; }
        public int StatusId { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class InitiateReviewsRequest
    {
        [Required] public string? CycleId { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ReviewRatingItem
    {
        public string? Id { get; set; }
        public decimal? SelfScore { get; set; }
        public decimal? ManagerScore { get; set; }
        public string? Comments { get; set; }
    }

    public class SaveReviewRatingsRequest
    {
        [Required] public string? ReviewId { get; set; }
        public List<ReviewRatingItem>? Ratings { get; set; }
        public string? SelfComments { get; set; }
        public string? ManagerComments { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ReviewActionRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }   // 2 Self / 3 Manager / 4 Completed / 5 Acknowledged
        public string? Acknowledgement { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class CycleActionRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }   // 1 Draft / 2 Active / 3 Closed
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class AppraisalCycleResponse
    {
        public Guid Id { get; set; }
        public string? CycleCode { get; set; }
        public string? Name { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public DateTime? GoalDueDate { get; set; }
        public DateTime? ReviewDueDate { get; set; }
        public int RatingScaleMax { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int ReviewCount { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CompetencyResponse
    {
        public Guid Id { get; set; }
        public string? CompetencyCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GoalResponse
    {
        public Guid Id { get; set; }
        public string? GoalCode { get; set; }
        public Guid AppraisalCycleId { get; set; }
        public string? CycleName { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Title { get; set; }
        public decimal Weight { get; set; }
        public decimal ProgressPercent { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public string? ReviewCode { get; set; }
        public Guid AppraisalCycleId { get; set; }
        public string? CycleName { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public Guid? ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal? SelfRating { get; set; }
        public decimal? ManagerRating { get; set; }
        public decimal? OverallRating { get; set; }
        public int RatingScaleMax { get; set; }
    }
}
