using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SB.Security.Helper;

namespace SB.Security.Models.Base
{
    /// <summary>
    /// This is base user class used for getting user, user list, registerng, updating, and deleting the user.
    /// </summary>
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SaltKey { get; set; }
        public string? Email { get; set; }
        public string UserRole { get; set; } = ConstantSupplier.USER;
        public DateTime? LastLoginAttemptAt { get; set; }
        public int LoginFailedAttemptsCount { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
