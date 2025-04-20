using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    public class AppUserLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SaltKey { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
