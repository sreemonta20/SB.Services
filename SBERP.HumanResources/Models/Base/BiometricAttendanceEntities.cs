using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    // Lookups — Ids mirror enums in SBERP.HumanResources.Enum (BiometricAttendanceEnums.cs).
    public class BiometricTypeLookup               : BaseLookup { }
    public class BiometricRegistrationStatusLookup : BaseLookup { }
    public class PunchSourceLookup                 : BaseLookup { }
    public class PunchStatusLookup                 : BaseLookup { }

    /// <summary>
    /// A fingerprint / face terminal at an office door. The terminal pushes punches to
    /// /iclock/cdata (ZKTeco ADMS) or to the generic devicePunches API; it is recognised by
    /// its SerialNumber and, for the generic API, a device key whose SHA-256 is stored here.
    /// </summary>
    public class AttendanceDevice
    {
        [Key] public Guid Id { get; set; }
        public string? DeviceCode { get; set; }
        public string? Name { get; set; }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public int Protocol { get; set; }                 // EnumDeviceProtocol
        public Guid? BranchId { get; set; }
        public string? Location { get; set; }
        /// <summary>Optional: only accept pushes from this IP (the terminal's fixed LAN address).</summary>
        public string? IpAddress { get; set; }
        public string? ApiKeyHash { get; set; }
        public DateTime? LastSeenDate { get; set; }
        public DateTime? LastPunchDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AttendanceDeviceLog
    {
        [Key] public Guid Id { get; set; }
        public Guid AttendanceDeviceId { get; set; }
        public string? DeviceCode { get; set; }
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public int? Protocol { get; set; }
        public string? IpAddress { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    /// <summary>
    /// "Inside the office" for mobile punches. A punch passes when the caller's IP is in
    /// AllowedIpRanges (checked by the server, the reliable test), or the phone reports a Wi-Fi
    /// access point in AllowedWifiBssids, or a GPS position inside the radius (both reported by
    /// the phone, so weaker).
    /// </summary>
    public class AttendanceZone
    {
        [Key] public Guid Id { get; set; }
        public string? ZoneCode { get; set; }
        public string? Name { get; set; }
        public Guid? BranchId { get; set; }
        /// <summary>CSV of CIDR ranges, e.g. "192.168.1.0/24,203.0.113.10/32".</summary>
        public string? AllowedIpRanges { get; set; }
        /// <summary>CSV of Wi-Fi access point MAC addresses (BSSID), e.g. "a4:2b:b0:11:22:33".</summary>
        public string? AllowedWifiBssids { get; set; }
        [Column(TypeName = "decimal(9,6)")] public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9,6)")] public decimal? Longitude { get; set; }
        public int? RadiusMeters { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AttendanceZoneLog
    {
        [Key] public Guid Id { get; set; }
        public Guid AttendanceZoneId { get; set; }
        public string? ZoneCode { get; set; }
        public string? Name { get; set; }
        public string? AllowedIpRanges { get; set; }
        public string? AllowedWifiBssids { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    /// <summary>
    /// One biometric enrolment of an employee.
    /// <para><b>RegistrationNo</b> is generated from the biometric type, the employee code and a
    /// database sequence (e.g. BIO-FP-EMP-0001-000001). It is also the name of the encrypted file
    /// in the biometric vault folder when template data is stored.</para>
    /// <para>Mobile app (MethodId 1): the phone's fingerprint / Face ID unlocks a private key that never
    /// leaves the phone; <b>PublicKey</b> verifies its signatures. No fingerprint or face image is
    /// needed for this method.</para>
    /// <para>Door terminal (MethodId 2): the template stays on the terminal; <b>DeviceUserPin</b> maps
    /// the PIN the terminal reports to this employee.</para>
    /// </summary>
    public class EmployeeBiometricRegistration
    {
        [Key] public Guid Id { get; set; }
        public string? RegistrationNo { get; set; }
        public Guid EmployeeId { get; set; }
        public int BiometricTypeId { get; set; }          // EnumBiometricType
        public int MethodId { get; set; }                 // EnumBiometricMethod
        public int StatusId { get; set; }                 // EnumBiometricRegistrationStatus

        // mobile app
        public string? DeviceInstallationId { get; set; }
        public string? DevicePlatform { get; set; }       // Android / iOS
        public string? DeviceModel { get; set; }
        public string? AppVersion { get; set; }
        public string? PublicKey { get; set; }            // Base64 SubjectPublicKeyInfo, EC P-256
        public string? KeyAlgorithm { get; set; }         // ES256

        // door terminal
        public Guid? AttendanceDeviceId { get; set; }
        public string? DeviceUserPin { get; set; }

        // optional template file in the vault (encrypted), named after RegistrationNo
        public string? TemplateFileName { get; set; }
        public string? TemplateFormat { get; set; }
        public string? TemplateSha256 { get; set; }
        public int? TemplateSizeBytes { get; set; }
        public string? CapturedAngles { get; set; }       // e.g. "Front,Left,Right"

        public bool ConsentGiven { get; set; }
        public DateTime? ConsentDate { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokedReason { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeBiometricRegistrationLog
    {
        [Key] public Guid Id { get; set; }
        public Guid RegistrationId { get; set; }
        public string? RegistrationNo { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? BiometricTypeId { get; set; }
        public int? MethodId { get; set; }
        public int? StatusId { get; set; }
        public string? DeviceInstallationId { get; set; }
        public Guid? AttendanceDeviceId { get; set; }
        public string? DeviceUserPin { get; set; }
        public string? RevokedReason { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    /// <summary>
    /// Every check-in / check-out attempt, as received (append-only, including rejected and
    /// duplicate ones). The daily dbo.Attendances row is calculated from the accepted punches:
    /// earliest punch of the work day = check-in, latest = check-out.
    /// </summary>
    public class AttendancePunch
    {
        [Key] public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }             // null when a terminal PIN is unknown
        public DateTime PunchTimeUtc { get; set; }
        public DateTime PunchLocalTime { get; set; }      // office time zone (HRSettings.TimeZoneId)
        public DateTime AttendanceDate { get; set; }      // work date after the day cut-off
        public int PunchTypeId { get; set; }              // EnumPunchType
        public int SourceId { get; set; }                 // EnumPunchSource
        public int StatusId { get; set; }                 // EnumPunchStatus
        public string? RejectReason { get; set; }
        public int? BiometricTypeId { get; set; }
        public Guid? BiometricRegistrationId { get; set; }
        public Guid? AttendanceDeviceId { get; set; }
        public string? DeviceUserPin { get; set; }
        /// <summary>Idempotency key: the app's request id, or "{SN}|{PIN}|{time}" for terminals.</summary>
        public string? ClientRequestId { get; set; }
        public DateTime? ClientTime { get; set; }
        public string? ClientIp { get; set; }
        [Column(TypeName = "decimal(9,6)")] public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9,6)")] public decimal? Longitude { get; set; }
        public string? WifiBssid { get; set; }
        public string? WifiSsid { get; set; }
        public bool? IsWithinOffice { get; set; }
        public string? LocationCheck { get; set; }        // which rule passed, e.g. "IP:HQ"
        public string? RawData { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
