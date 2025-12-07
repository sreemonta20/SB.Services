namespace SBERP.Security.Models.ViewModel
{
    public class RoleMenuViewModel
    {
        public Guid? MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? ParentName { get; set; }
        public bool? IsView { get; set; } = false;
        public bool? IsCreate { get; set; } = false;
        public bool? IsUpdate { get; set; } = false;
        public bool? IsDelete { get; set; } = false;
        public bool? IsActive { get; set; } = false;
    }
}
