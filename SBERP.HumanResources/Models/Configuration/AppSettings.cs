namespace SBERP.HumanResources.Models.Configuration
{
    /// <summary>
    /// Root binding for the "AppSettings" section in appsettings.json.
    /// Kept separate from "SharedJwtSettings" and "RedisSettings" which are
    /// bound by SBERP.Shared.
    /// </summary>
    public class AppSettings
    {
        public string? AppDB { get; set; }
        public ConnectionStrings? ConnectionStrings { get; set; }
        public string? ConnectionProvider { get; set; }
        public AttendanceSettings? Attendance { get; set; }
    }

    public class ConnectionStrings
    {
        public string? HRDatabase { get; set; }
    }

    public class AttendanceSettings
    {
        /// <summary>1 = ManualExcelUpload, 2 = Biometric. See EnumAttendanceSource.</summary>
        public int DefaultSource { get; set; } = 1;
        public string? BiometricConnectionString { get; set; }
        public string? BiometricProviderName { get; set; }
        public string? UploadRootPath { get; set; }
    }

    public class RedisSettings
    {
        public string? ConnectionString { get; set; }
        public string? InstanceName { get; set; }
    }
}
