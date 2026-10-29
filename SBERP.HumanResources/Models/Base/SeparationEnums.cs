namespace SBERP.HumanResources.Enum
{
    public enum EnumSeparationType
    {
        Resignation = 1,
        Termination = 2,
        Retirement = 3,
        EndOfContract = 4,
        Death = 5
    }

    public enum EnumSeparationStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        InClearance = 4,
        Settled = 5,
        Completed = 6,
        Cancelled = 7
    }

    public enum EnumClearanceStatus
    {
        Pending = 1,
        Cleared = 2,
        Blocked = 3
    }

    public enum EnumClearanceDepartment
    {
        IT = 1,
        Finance = 2,
        HR = 3,
        Admin = 4,
        Manager = 5
    }
}
