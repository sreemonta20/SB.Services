using SSBERP.Security.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    /// <summary>
    /// This class used to create and keep the Application user role records.
    /// </summary>
    public class AppUserRole
    {
        [Key]
        public Guid Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<AppUserProfile>? AppUserProfiles { get; set; }
        public ICollection<AppUserRoleMenu>? AppUserRoleMenus { get; set; }
    }
}
