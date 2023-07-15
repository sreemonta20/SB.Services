using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SB.Security.Models.Base
{

    public class UserRoleMenu
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("UserRole")]
        public Guid RoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
        [ForeignKey("UserMenu")]
        public Guid MenuId { get; set; }
        public virtual UserMenu UserMenu { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean? IsActive { get; set; }
    }
}
