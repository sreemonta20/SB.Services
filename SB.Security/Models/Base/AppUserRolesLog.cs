using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    /// <summary>
    /// This class used to keep the track of records created, modified, deleted in AppUserRole. 
    /// </summary>
    public class AppUserRoleLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserRoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
    
}
