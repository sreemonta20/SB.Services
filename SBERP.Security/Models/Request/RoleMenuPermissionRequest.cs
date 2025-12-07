namespace SBERP.Security.Models.Request
{
    public class RoleMenuPermissionRequest
    {
        public Guid? RoleMenuId { get; set; }
        public Guid MenuId { get; set; }

        // ONLY permissions allowed to update/create
        public bool IsView { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
    }
}
