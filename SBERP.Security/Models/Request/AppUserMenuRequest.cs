namespace SBERP.Security.Models.Request
{
    public class AppUserMenuRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool? IsHeader { get; set; }
        public bool? IsModule { get; set; }
        public bool? IsComponent { get; set; }
        public string? CssClass { get; set; }
        public bool? IsRouteLink { get; set; }
        public string? RouteLink { get; set; }
        public string? RouteLinkClass { get; set; }
        public string? Icon { get; set; }
        public string? Remark { get; set; }
        public string? ParentId { get; set; }
        public string? DropdownIcon { get; set; }
        public int? SerialNo { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
