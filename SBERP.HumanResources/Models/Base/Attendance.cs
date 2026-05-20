using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// One row per (employee, date). Reading from a manual Excel and reading
    /// from a biometric DB produces rows of the same shape — the SourceType
    /// column records where it came from for auditing.
    ///
    /// SourceType: 1=ManualExcel, 2=Biometric. See EnumAttendanceSource.
    /// </summary>
    public class Attendance
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }   // date-only logically

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? WorkingHours { get; set; }
        public int? Status { get; set; }                // EnumAttendanceStatus
        public int SourceType { get; set; }             // 1 manual / 2 biometric
        public Guid? UploadBatchId { get; set; }        // links rows to upload session
        public string? Remarks { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    /// <summary>
    /// Every Excel upload creates one of these. Lets HR audit who uploaded
    /// what and roll back a bad batch by UploadBatchId.
    /// </summary>
    public class AttendanceUploadLog
    {
        [Key] public Guid Id { get; set; }              // == UploadBatchId on Attendance rows
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSizeBytes { get; set; }
        public int? TotalRows { get; set; }
        public int? SuccessRows { get; set; }
        public int? FailedRows { get; set; }
        public string? ErrorReport { get; set; }        // JSON of failed-row details
        public string? UploadedBy { get; set; }
        public DateTime? UploadedDate { get; set; }
        public bool? IsRolledBack { get; set; }
        public DateTime? RolledBackDate { get; set; }
    }
}
