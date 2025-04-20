using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Base
{
    public class AppUserMenuLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserMenuId { get; set; }
        public string? Name { get; set; }
        public bool? IsHeader { get; set; }
        public bool? IsModule { get; set; }
        public bool? IsComponent { get; set; }
        public string? CssClass { get; set; }
        public string? RouteLink { get; set; }
        public string? RouteLinkClass { get; set; }
        public string? Icon { get; set; }
        public string? Remark { get; set; }
        public Guid? ParentId { get; set; }
        public string? DropdownIcon { get; set; }
        public int? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}
