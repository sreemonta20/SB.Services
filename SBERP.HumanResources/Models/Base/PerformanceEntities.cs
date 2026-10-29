using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    public class AppraisalCycleStatusLookup : BaseLookup { }
    public class GoalStatusLookup           : BaseLookup { }
    public class ReviewStatusLookup         : BaseLookup { }
    public class RatingItemTypeLookup       : BaseLookup { }

    public class AppraisalCycle
    {
        [Key] public Guid Id { get; set; }
        public string? CycleCode { get; set; }
        public string? Name { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime? GoalDueDate { get; set; }
        public DateTime? ReviewDueDate { get; set; }
        public int RatingScaleMax { get; set; }
        public int StatusId { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AppraisalCycleLog
    {
        [Key] public Guid Id { get; set; }
        public Guid AppraisalCycleId { get; set; }
        public string? CycleCode { get; set; }
        public string? Name { get; set; }
        public int? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class Competency
    {
        [Key] public Guid Id { get; set; }
        public string? CompetencyCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CompetencyLog
    {
        [Key] public Guid Id { get; set; }
        public Guid CompetencyId { get; set; }
        public string? CompetencyCode { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class Goal
    {
        [Key] public Guid Id { get; set; }
        public string? GoalCode { get; set; }
        public Guid AppraisalCycleId { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal Weight { get; set; }
        public string? TargetValue { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal ProgressPercent { get; set; }
        public int StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual AppraisalCycle? AppraisalCycle { get; set; }
        public virtual Employee? Employee { get; set; }
    }

    public class GoalLog
    {
        [Key] public Guid Id { get; set; }
        public Guid GoalId { get; set; }
        public string? GoalCode { get; set; }
        public Guid? AppraisalCycleId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? Title { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? ProgressPercent { get; set; }
        public int? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class Review
    {
        [Key] public Guid Id { get; set; }
        public string? ReviewCode { get; set; }
        public Guid AppraisalCycleId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? ReviewerId { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? SelfRating { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? ManagerRating { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? OverallRating { get; set; }
        public string? SelfComments { get; set; }
        public string? ManagerComments { get; set; }
        public string? EmployeeAcknowledgement { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual AppraisalCycle? AppraisalCycle { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<ReviewRating>? Ratings { get; set; }
    }

    public class ReviewLog
    {
        [Key] public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public string? ReviewCode { get; set; }
        public Guid? AppraisalCycleId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? StatusId { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? OverallRating { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class ReviewRating
    {
        [Key] public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public int ItemTypeId { get; set; }       // 1 Goal / 2 Competency
        public Guid? ItemId { get; set; }
        public string? ItemName { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal Weight { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? SelfScore { get; set; }
        [Column(TypeName = "decimal(4,2)")] public decimal? ManagerScore { get; set; }
        public string? Comments { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
