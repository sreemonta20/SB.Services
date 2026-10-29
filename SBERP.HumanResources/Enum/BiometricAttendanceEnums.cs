namespace SBERP.HumanResources.Enum
{
    // Biometric attendance (mobile app + door terminal). Ids mirror the seeded lookup rows
    // (BiometricTypes, BiometricRegistrationStatuses, PunchSources, PunchStatuses).

    public enum EnumBiometricType
    {
        Fingerprint = 1,
        Face = 2
    }

    /// <summary>How the employee proves who they are.</summary>
    public enum EnumBiometricMethod
    {
        /// <summary>Phone's own fingerprint / face unlock releases a private key kept in the phone's
        /// secure hardware; the server only stores the matching public key.</summary>
        MobileDeviceKey = 1,

        /// <summary>Employee is enrolled on a door terminal; the terminal reports a user PIN.</summary>
        DoorTerminal = 2
    }

    public enum EnumBiometricRegistrationStatus
    {
        PendingVerification = 1,
        Active = 2,
        Revoked = 3
    }

    public enum EnumPunchSource
    {
        MobileApp = 1,
        DoorTerminal = 2,
        Manual = 3
    }

    /// <summary>What the punch says it is. Check-in/out of the day is still earliest/latest punch.</summary>
    public enum EnumPunchType
    {
        Auto = 0,
        CheckIn = 1,
        CheckOut = 2
    }

    public enum EnumPunchStatus
    {
        Accepted = 1,
        Duplicate = 2,
        Rejected = 3
    }

    public enum EnumDeviceProtocol
    {
        /// <summary>ZKTeco "ADMS / Push SDK": the terminal posts to /iclock/cdata.</summary>
        ZKTecoAdms = 1,

        /// <summary>Any vendor via a small bridge posting JSON with the device key.</summary>
        GenericJson = 2
    }
}
