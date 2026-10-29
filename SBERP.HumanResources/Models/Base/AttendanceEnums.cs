namespace SBERP.HumanResources.Enum
{
    // Ids mirror seeded lookup rows. EnumAttendanceStatus and EnumAttendanceSource
    // already exist in the codebase — not redefined here.

    public enum EnumOvertimeStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum EnumRegularizationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public enum EnumRegularizationType
    {
        MissedCheckIn = 1,
        MissedCheckOut = 2,
        WrongPunchTime = 3,
        ForgotToPunch = 4,
        OnDutyFieldWork = 5
    }
}
