namespace SBERP.HumanResources.Enum
{
    /// <summary>
    /// Decides where daily attendance comes from. Set via HRSettings table — the
    /// settings UI flips this and the Attendance pipeline reads it on every run.
    /// </summary>
    public enum EnumAttendanceSource
    {
        /// <summary>HR uploads an Excel sheet manually.</summary>
        ManualExcelUpload = 1,

        /// <summary>Pulls from a biometric (fingerprint / face / iris) DB.</summary>
        Biometric = 2
    }

    public enum EnumBiometricProvider
    {
        None = 0,
        Fingerprint = 1,
        Face = 2,
        Iris = 3,
        Multi = 4
    }

    public enum EnumGender { Male = 1, Female = 2, Other = 3 }

    public enum EnumMaritalStatus { Single = 1, Married = 2, Divorced = 3, Widowed = 4, Separated = 5 }

    public enum EnumEmploymentStatus
    {
        Active = 1,
        OnLeave = 2,
        Suspended = 3,
        Resigned = 4,
        Terminated = 5,
        Retired = 6
    }

    public enum EnumEmploymentType
    {
        FullTime = 1,
        PartTime = 2,
        Contract = 3,
        Intern = 4,
        Consultant = 5,
        Probation = 6
    }

    public enum EnumBloodGroup
    {
        Unknown = 0,
        APositive = 1,  ANegative = 2,
        BPositive = 3,  BNegative = 4,
        ABPositive = 5, ABNegative = 6,
        OPositive = 7,  ONegative = 8
    }

    public enum EnumAttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Leave = 3,
        Holiday = 4,
        WeekOff = 5,
        HalfDay = 6,
        Late = 7
    }
}
