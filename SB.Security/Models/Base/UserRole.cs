using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
