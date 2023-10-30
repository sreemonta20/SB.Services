using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    /// <summary>
    /// This class used to perform the user authentication and validation.
    /// </summary>
    public class AppLoggedInUser
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AppUserProfileId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLoginAttemptAt { get; set; }
        public int? LoginFailedAttemptsCount { get; set; }
        public bool? IsActive { get; set; }
        public virtual AppUserProfile? AppUserProfile { get; set; }
    }
}
