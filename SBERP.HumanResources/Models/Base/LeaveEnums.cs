namespace SBERP.HumanResources.Enum
{
    // Ids mirror seeded lookup rows.

    public enum EnumLeaveApplicationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4,
        Withdrawn = 5
    }

    public enum EnumLeaveTransactionType
    {
        Grant = 1,
        Accrual = 2,
        CarryForward = 3,
        Deduction = 4,
        Reversal = 5,
        Adjustment = 6,
        Encashment = 7
    }

    /// <summary>Maps to the seeded LeaveTypes.LeaveTypeCode values.</summary>
    public enum EnumLeaveType
    {
        Annual = 1,
        Sick = 2,
        Casual = 3,
        Unpaid = 4,
        Maternity = 5,
        Paternity = 6
    }
}
