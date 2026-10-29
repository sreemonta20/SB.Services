namespace SBERP.HumanResources.Enum
{
    public enum EnumPayComponentType
    {
        Earning = 1,
        Deduction = 2
    }

    public enum EnumPayCalcMethod
    {
        Fixed = 1,
        PercentOfBasic = 2
    }

    public enum EnumPayrollRunStatus
    {
        Draft = 1,
        Processed = 2,
        Approved = 3,
        Paid = 4,
        Cancelled = 5
    }

    public enum EnumLoanStatus
    {
        Active = 1,
        Closed = 2,
        Cancelled = 3
    }
}
