using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    /// <summary>In-app notification (feeds the AdminLTE navbar bell).</summary>
    public class Notification
    {
        [Key] public Guid Id { get; set; }
        /// <summary>AppUserProfiles.Id of the person who should see it.</summary>
        public Guid RecipientProfileId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; }          // Onboarding / System / ...
        public string? RouteLink { get; set; }     // optional SPA link
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Transactional e-mail outbox: rows are written in the same transaction as
    /// the business change and delivered later by EmailOutboxSender. The body is
    /// cleared once sent, so one-time credentials don't stay at rest.
    /// </summary>
    public class EmailOutbox
    {
        [Key] public Guid Id { get; set; }
        public string? ToAddress { get; set; }
        public string? ToName { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Category { get; set; }      // WelcomeEmail / ...
        public int Attempts { get; set; }
        public string? LastError { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
