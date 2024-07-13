using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    public class AppUserRoleMenuLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AppUserRoleMenuId { get; set; }
        public Guid? AppUserRoleId { get; set; }
        public Guid? AppUserMenuId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsCreate { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
