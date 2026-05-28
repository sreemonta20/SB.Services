using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// Single-row settings table — controls org-wide HR behavior.
    /// AttendanceSource flips between manual Excel upload and biometric DB pull.
    /// When Biometric is selected, the BiometricConnectionString and
    /// BiometricProvider tell the attendance pipeline where to read from.
    ///
    /// Implemented as a row (not appsettings) so HR admins can change it
    /// without a redeploy. There should only ever be one active row.
    /// </summary>
    public class HRSettings
    {
        [Key] public Guid Id { get; set; }

        /// <summary>1 = ManualExcelUpload, 2 = Biometric. See EnumAttendanceSource.</summary>
        public int AttendanceSource { get; set; } = 1;

        /// <summary>0 None / 1 Fingerprint / 2 Face / 3 Iris / 4 Multi.</summary>
        public int? BiometricProvider { get; set; }

        /// <summary>Connection string for the biometric system DB (encrypted at rest).</summary>
        public string? BiometricConnectionString { get; set; }

        /// <summary>Bio DB table or stored procedure that exposes daily punches.</summary>
        public string? BiometricSourceObject { get; set; }

        /// <summary>"08:30" — local time. Anyone clocking in after this is late.</summary>
        public string? OfficeStartTime { get; set; }
        public string? OfficeEndTime { get; set; }
        public int? GracePeriodMinutes { get; set; }

        /// <summary>CSV of weekday numbers (0=Sun..6=Sat). UAE default = "5,6".</summary>
        public string? WeeklyOffDays { get; set; }

        /// <summary>Number of paid leave days per year per employee.</summary>
        public int? AnnualLeaveDays { get; set; }
        public int? SickLeaveDays { get; set; }
        public int? CasualLeaveDays { get; set; }

        /// <summary>Auto-process attendance every night at this time. "23:30".</summary>
        public string? AutoProcessTime { get; set; }
        public bool? AutoProcessEnabled { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsHardDelete { get; set; }
    }
}
