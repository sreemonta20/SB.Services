using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    public class AppUserProfileLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserProfileId { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Guid? AppUserRoleId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
