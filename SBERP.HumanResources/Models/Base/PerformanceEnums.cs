namespace SBERP.HumanResources.Enum
{
    public enum EnumAppraisalCycleStatus
    {
        Draft = 1,
        Active = 2,
        Closed = 3
    }

    public enum EnumGoalStatus
    {
        Draft = 1,
        Active = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum EnumReviewStatus
    {
        NotStarted = 1,
        SelfAssessment = 2,
        ManagerReview = 3,
        Completed = 4,
        Acknowledged = 5
    }

    public enum EnumRatingItemType
    {
        Goal = 1,
        Competency = 2
    }
}
