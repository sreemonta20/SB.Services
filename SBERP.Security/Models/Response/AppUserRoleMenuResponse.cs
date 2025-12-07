namespace SBERP.Security.Models.Response
{
    public class AppUserRoleMenuResponse
    {
        //public Guid Id { get; set; }
        //public Guid? AppUserRoleId { get; set; }
        //public string? RoleName { get; set; }
        //public Guid? AppUserMenuId { get; set; }
        //public string? MenuName { get; set; }
        //public bool? IsView { get; set; }
        //public bool? IsCreate { get; set; }
        //public bool? IsUpdate { get; set; }
        //public bool? IsDelete { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public bool? IsActive { get; set; }

        public Guid? RoleMenuId { get; set; }
        public Guid MenuId { get; set; }
        public Guid RoleId { get; set; }

        public string MenuName { get; set; }

        // READ-ONLY METADATA (coming from AppUserMenus)
        public bool IsHeader { get; set; }
        public bool IsModule { get; set; }
        public bool IsComponent { get; set; }
        public bool IsRouteLink { get; set; }
        public string Url { get; set; }
        public int SerialNo { get; set; }
        public bool IsActiveMenu { get; set; }

        // EDITABLE PERMISSIONS (to be saved)
        public bool IsView { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
    }
}
