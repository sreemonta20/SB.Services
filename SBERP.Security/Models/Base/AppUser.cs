using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.Security.Models.Base
{
    /// <summary>
    /// This class used to perform the user authentication and validation.
    /// </summary>
    public class AppUser
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SaltKey { get; set; }
        /// <summary>Consecutive failed logins since the last success or lockout (AppSettings:MaxNumberOfFailedAttempts).</summary>
        public int FailedLoginAttempts { get; set; }
        /// <summary>UTC time until which login is refused (AppSettings:BlockMinutes after the last allowed failure).</summary>
        public DateTime? LockoutEnd { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public virtual AppUserProfile? AppUserProfile { get; set; }
    }
}
