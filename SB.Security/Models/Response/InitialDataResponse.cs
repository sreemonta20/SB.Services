using SB.Security.Models.Base;

namespace SB.Security.Models.Response
{
    public class InitialDataResponse
    {
        public List<AppUserRoleMenuInitialData>? UserRoles { get; set; }
        public List<AppUserRoleMenuInitialData>? ParentMenu { get; set; }
        public List<AppUserRoleMenuInitialData>? CssClass { get; set; }
        public List<AppUserRoleMenuInitialData>? RouteLinkClass { get; set; }
        public int? NextMenuSlNo { get; set; }

    }
}
