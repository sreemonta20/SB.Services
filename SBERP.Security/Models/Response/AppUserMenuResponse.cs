namespace SBERP.Security.Models.Response
{
    public class AppUserMenuResponse
    {
        public Guid Id { get; set; }
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
        public string? ParentName { get; set; }
        public string? DropdownIcon { get; set; }
        public int? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
