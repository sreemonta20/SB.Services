using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    // ---------------------------------------------------------------- mobile app

    /// <summary>One captured template (e.g. a face image per angle) — only for the optional
    /// "store biometric data on the server" path. The phone-unlock method sends none.</summary>
    public class BiometricTemplateItem
    {
        public string? Angle { get; set; }        // Front / Left / Right, or finger name
        public string? Format { get; set; }       // e.g. "image/jpeg", "ISO19794-2"
        [Required] public string? DataBase64 { get; set; }
    }

    /// <summary>Registration from the mobile app (caller is logged in with the Security login API).</summary>
    public class MobileBiometricRegistrationRequest
    {
        [Required] public string? EmployeeCode { get; set; }
        [Range(1, 2)] public int BiometricTypeId { get; set; }          // 1 Fingerprint / 2 Face
        [Required] public string? DeviceInstallationId { get; set; }
        public string? DevicePlatform { get; set; }
        public string? DeviceModel { get; set; }
        public string? AppVersion { get; set; }
        /// <summary>Base64 SubjectPublicKeyInfo of an EC P-256 key created in the phone's secure hardware.</summary>
        [Required] public string? PublicKey { get; set; }
        public bool ConsentGiven { get; set; }
        public List<BiometricTemplateItem>? Templates { get; set; }
    }

    /// <summary>Proves the new key works: signature over "{challenge}|{registrationNo}|verify|".</summary>
    public class VerifyBiometricRegistrationRequest
    {
        [Required] public string? RegistrationNo { get; set; }
        [Required] public string? ChallengeId { get; set; }
        [Required] public string? Signature { get; set; }
    }

    public class BiometricChallengeRequest
    {
        [Required] public string? RegistrationNo { get; set; }
        /// <summary>"punch" (default) or "verify".</summary>
        public string? Purpose { get; set; }
    }

    /// <summary>
    /// The Identification API. The app shows the fingerprint / face prompt, the phone signs
    /// "{challenge}|{registrationNo}|punch|{clientRequestId}" and posts it here. The server
    /// takes the time itself — ClientTime is kept for information only.
    /// </summary>
    public class MobilePunchRequest
    {
        [Required] public string? RegistrationNo { get; set; }
        [Required] public string? ChallengeId { get; set; }
        [Required] public string? Signature { get; set; }
        [Required, MaxLength(100)] public string? ClientRequestId { get; set; }
        public int PunchTypeId { get; set; }                            // 0 auto / 1 in / 2 out
        public DateTime? ClientTime { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? WifiBssid { get; set; }
        public string? WifiSsid { get; set; }
    }

    // ---------------------------------------------------------------- door terminal (generic JSON bridge)

    public class DevicePunchItem
    {
        [Required] public string? UserPin { get; set; }
        /// <summary>Terminal's local time (office time zone).</summary>
        public DateTime PunchTime { get; set; }
        public int PunchTypeId { get; set; }
        /// <summary>1 Fingerprint / 2 Face, if the terminal reports it.</summary>
        public int? BiometricTypeId { get; set; }
        public string? DeviceLogId { get; set; }
    }

    public class DevicePunchBatchRequest
    {
        [Required] public string? SerialNumber { get; set; }
        [Required] public List<DevicePunchItem>? Punches { get; set; }
    }

    // ---------------------------------------------------------------- admin screens

    public class AttendanceDeviceRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? DeviceCode { get; set; }
        [Required] public string? Name { get; set; }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        [Required] public string? SerialNumber { get; set; }
        [Range(1, 2)] public int Protocol { get; set; } = 1;
        public string? BranchId { get; set; }
        public string? Location { get; set; }
        public string? IpAddress { get; set; }
        public string? Remarks { get; set; }
        /// <summary>Create a new device key (shown once). Always done on create.</summary>
        public bool RegenerateKey { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreateUpdateBy { get; set; }
    }

    public class AttendanceZoneRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? ZoneCode { get; set; }
        [Required] public string? Name { get; set; }
        public string? BranchId { get; set; }
        public string? AllowedIpRanges { get; set; }
        public string? AllowedWifiBssids { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? RadiusMeters { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>HR maps a door-terminal user PIN to an employee (the template stays on the terminal).</summary>
    public class TerminalEnrollmentRequest
    {
        [Required] public string? EmployeeId { get; set; }
        [Range(1, 2)] public int BiometricTypeId { get; set; }
        [Required] public string? DeviceUserPin { get; set; }
        public string? AttendanceDeviceId { get; set; }
        public bool ConsentGiven { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class RevokeBiometricRegistrationRequest
    {
        [Required] public string? Id { get; set; }
        public string? Reason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ReprocessAttendanceRequest
    {
        [Required] public DateTime FromDate { get; set; }
        [Required] public DateTime ToDate { get; set; }
        public string? EmployeeId { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class BiometricChallengeResponse
    {
        public string? RegistrationNo { get; set; }
        public string? ChallengeId { get; set; }
        public string? Challenge { get; set; }
        public int ExpiresInSeconds { get; set; }
    }

    public class MobilePunchResponse
    {
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? Result { get; set; }             // CheckedIn / CheckedOut / Duplicate
        public DateTime PunchTime { get; set; }         // office local time
        public DateTime AttendanceDate { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
    }

    public class AttendanceDeviceResponse
    {
        public Guid Id { get; set; }
        public string? DeviceCode { get; set; }
        public string? Name { get; set; }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public int Protocol { get; set; }
        public string? ProtocolName { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? Location { get; set; }
        public string? IpAddress { get; set; }
        public bool HasApiKey { get; set; }
        public DateTime? LastSeenDate { get; set; }
        public DateTime? LastPunchDate { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AttendanceZoneResponse
    {
        public Guid Id { get; set; }
        public string? ZoneCode { get; set; }
        public string? Name { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? AllowedIpRanges { get; set; }
        public string? AllowedWifiBssids { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? RadiusMeters { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BiometricRegistrationResponse
    {
        public Guid Id { get; set; }
        public string? RegistrationNo { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public int BiometricTypeId { get; set; }
        public string? BiometricTypeName { get; set; }
        public int MethodId { get; set; }
        public string? MethodName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? DevicePlatform { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceUserPin { get; set; }
        public string? DeviceName { get; set; }
        public bool HasTemplate { get; set; }
        public bool ConsentGiven { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokedReason { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class AttendancePunchResponse
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime PunchLocalTime { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int PunchTypeId { get; set; }
        public int SourceId { get; set; }
        public string? SourceName { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? RejectReason { get; set; }
        public string? BiometricTypeName { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceUserPin { get; set; }
        public string? ClientIp { get; set; }
        public bool? IsWithinOffice { get; set; }
        public string? LocationCheck { get; set; }
    }
}
