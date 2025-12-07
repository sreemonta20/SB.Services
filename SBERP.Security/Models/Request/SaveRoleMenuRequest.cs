using SBERP.Security.Models.ViewModel;

namespace SBERP.Security.Models.Request
{
    public class SaveRoleMenuRequest
    {
        public Guid RoleId { get; set; }
        public string UserId { get; set; }
        public List<RoleMenuPermissionRequest> Permissions { get; set; }
    }
}
