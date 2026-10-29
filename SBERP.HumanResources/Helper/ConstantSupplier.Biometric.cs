namespace SBERP.HumanResources.Helper
{
    public static partial class ConstantSupplier
    {
        #region Stored Procedures — Biometric attendance
        public const string SP_GET_ALL_ATTENDANCE_DEVICES_PAGING      = "SP_GetAllAttendanceDevicesPagingWithSearch";
        public const string SP_GET_ALL_ATTENDANCE_ZONES_PAGING        = "SP_GetAllAttendanceZonesPagingWithSearch";
        public const string SP_GET_ALL_BIOMETRIC_REGISTRATIONS_PAGING = "SP_GetAllBiometricRegistrationsPagingWithSearch";
        public const string SP_GET_ALL_ATTENDANCE_PUNCHES_PAGING      = "SP_GetAllAttendancePunchesPagingWithSearch";
        #endregion

        #region Routes — Biometric attendance
        // mobile app
        public const string REGISTER_MOBILE_BIOMETRIC_ROUTE        = "registerMobileBiometric";
        public const string VERIFY_BIOMETRIC_REGISTRATION_ROUTE    = "verifyBiometricRegistration";
        public const string GET_MY_BIOMETRIC_REGISTRATION_ROUTE    = "getMyBiometricRegistration";
        public const string GET_BIOMETRIC_CHALLENGE_ROUTE          = "getBiometricChallenge";
        public const string MOBILE_PUNCH_ROUTE                     = "mobilePunch";
        // door terminal
        public const string DEVICE_PUNCHES_ROUTE                   = "devicePunches";
        // admin
        public const string GET_ALL_BIOMETRIC_REGISTRATIONS_PAGING_ROUTE = "getAllBiometricRegistrationsPagingWithSearch";
        public const string CREATE_TERMINAL_ENROLLMENT_ROUTE       = "createTerminalEnrollment";
        public const string REVOKE_BIOMETRIC_REGISTRATION_ROUTE    = "revokeBiometricRegistration";
        public const string GET_ALL_ATTENDANCE_DEVICES_PAGING_ROUTE = "getAllAttendanceDevicesPagingWithSearch";
        public const string CREATE_UPDATE_ATTENDANCE_DEVICE_ROUTE  = "createUpdateAttendanceDevice";
        public const string DELETE_ATTENDANCE_DEVICE_ROUTE         = "deleteAttendanceDevice";
        public const string GET_ALL_ATTENDANCE_ZONES_PAGING_ROUTE  = "getAllAttendanceZonesPagingWithSearch";
        public const string CREATE_UPDATE_ATTENDANCE_ZONE_ROUTE    = "createUpdateAttendanceZone";
        public const string DELETE_ATTENDANCE_ZONE_ROUTE           = "deleteAttendanceZone";
        public const string GET_ALL_ATTENDANCE_PUNCHES_PAGING_ROUTE = "getAllAttendancePunchesPagingWithSearch";
        public const string REPROCESS_ATTENDANCE_ROUTE             = "reprocessAttendance";
        #endregion

        #region Messages — Biometric attendance
        public const string BIO_REGISTERED                = "Biometric registration created. Verify it to activate.";
        public const string BIO_VERIFIED                  = "Biometric registration and verification successful.";
        public const string BIO_NOT_YOUR_EMPLOYEE         = "This employee ID is not linked to the logged-in user.";
        public const string BIO_EMPLOYEE_NOT_FOUND        = "No active employee found for this employee ID.";
        public const string BIO_CONSENT_REQUIRED          = "The employee's consent to biometric processing is required.";
        public const string BIO_INVALID_PUBLIC_KEY        = "The device public key is not a valid EC P-256 key.";
        public const string BIO_REGISTRATION_INVALID      = "Biometric registration not found or not active.";
        public const string BIO_CHALLENGE_INVALID         = "The verification request has expired. Please try again.";
        public const string BIO_SIGNATURE_INVALID         = "Biometric verification failed.";
        public const string BIO_MOBILE_DISABLED           = "Mobile attendance is switched off.";
        public const string BIO_OUTSIDE_OFFICE            = "You are not connected to the office network.";
        public const string BIO_NO_ZONE                   = "No office zone is configured for mobile attendance.";
        public const string BIO_TEMPLATE_TOO_LARGE        = "Biometric data is too large.";
        public const string BIO_PUNCH_OK_IN               = "Successfully checked in.";
        public const string BIO_PUNCH_OK_OUT              = "Successfully checked out.";
        public const string BIO_PUNCH_DUPLICATE           = "Already recorded a moment ago.";
        public const string BIO_REVOKED                   = "Biometric registration revoked.";
        public const string BIO_TERMINAL_ENROLLED         = "Terminal enrolment saved.";
        public const string BIO_PIN_IN_USE                = "This terminal PIN is already assigned to another employee.";
        public const string BIO_FETCH_SUCCESS             = "Biometric data fetched successfully.";
        public const string BIO_LIST_EMPTY                = "No records found.";
        public const string DEVICE_UNKNOWN                = "Unknown or inactive attendance device.";
        public const string DEVICE_SAVED                  = "Attendance device saved.";
        public const string DEVICE_DUPLICATE              = "Device code or serial number already exists.";
        public const string ZONE_SAVED                    = "Office zone saved.";
        public const string ZONE_DUPLICATE                = "Zone code already exists.";
        public const string ATT_REPROCESSED               = "Attendance recalculated from punches.";
        #endregion
    }
}
