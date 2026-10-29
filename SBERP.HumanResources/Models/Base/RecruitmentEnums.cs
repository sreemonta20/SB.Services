namespace SBERP.HumanResources.Enum
{
    // =========================================================================
    // Recruitment enums — Ids MUST match the seeded lookup table rows
    // (RequisitionStatuses, JobPostingStatuses, ApplicationStages, InterviewModes,
    // OfferStatuses). Same convention as EnumGender / EnumEmploymentType etc.
    // =========================================================================

    public enum EnumRequisitionStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4,
        OnHold = 5,
        Closed = 6,
        Filled = 7
    }

    public enum EnumJobPostingStatus
    {
        Draft = 1,
        Published = 2,
        Paused = 3,
        Closed = 4,
        Cancelled = 5
    }

    public enum EnumApplicationStage
    {
        Applied = 1,
        Screening = 2,
        Shortlisted = 3,
        Interview = 4,
        Offer = 5,
        Hired = 6,
        Rejected = 7,
        Withdrawn = 8
    }

    public enum EnumInterviewMode
    {
        InPerson = 1,
        Video = 2,
        Phone = 3,
        AssessmentTest = 4
    }

    public enum EnumInterviewRecommendation
    {
        Hire = 1,
        Hold = 2,
        Reject = 3
    }

    public enum EnumOfferStatus
    {
        Draft = 1,
        Issued = 2,
        Accepted = 3,
        Declined = 4,
        Expired = 5,
        Withdrawn = 6
    }
}
