using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    /// <summary>
    /// A leaver's login switch-off, created from HR's EmployeeOffboardedEvent.
    /// One row per separation (unique SeparationId = idempotency key). Rows whose
    /// DeactivateOn is still in the future are picked up by LoginDeactivationScheduler.
    /// </summary>
    public class LoginDeactivation
    {
        [Key] public Guid Id { get; set; }
        public Guid SeparationId { get; set; }
        public string? SeparationCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        /// <summary>UTC instant the login is disabled (00:00 local after the last working day).</summary>
        public DateTime DeactivateOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        /// <summary>Disabled / NoLogin / AlreadyInactive.</summary>
        public string? Outcome { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
