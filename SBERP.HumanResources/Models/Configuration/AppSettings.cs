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
        public string? HRSqlConnectionString { get; set; }
        public string? HRDefaultConnectionString { get; set; }
    }

    public class AttendanceSettings
    {
        /// <summary>1 = ManualExcelUpload, 2 = Biometric. See EnumAttendanceSource.</summary>
        public int DefaultSource { get; set; } = 1;
        public string? BiometricConnectionString { get; set; }
        public string? BiometricProviderName { get; set; }
        public string? UploadRootPath { get; set; }

        // ---- biometric attendance ----
        /// <summary>Folder for encrypted biometric template files (dev: local folder; prod: dedicated server folder).</summary>
        public string? BiometricVaultPath { get; set; }
        /// <summary>Folder for the encryption keys of the vault. Keep it apart from the vault and back it up.</summary>
        public string? BiometricKeyRingPath { get; set; }
        public int MaxTemplateBytes { get; set; } = 2 * 1024 * 1024;
        /// <summary>Lifetime of the one-time challenge the phone signs.</summary>
        public int ChallengeSeconds { get; set; } = 120;
        /// <summary>CSV of proxies (the API gateway) whose X-Forwarded-For header is trusted.</summary>
        public string? TrustedProxies { get; set; }
    }

    public class RedisSettings
    {
        public string? ConnectionString { get; set; }
        public string? InstanceName { get; set; }
    }
}
