using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    /// <summary>
    /// This is base user class used for getting user, user list, registerng, updating, and deleting the user.
    /// </summary>
    public class AppUserProfile
    {
        [Key]
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Guid? AppUserRoleId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public virtual AppUserRole? AppUserRole { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
