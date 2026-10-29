namespace SBERP.Security.Models.Base
{
    public class AppUserRoleMenuInitialData
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        /// <summary>Menu type of a parentMenuList row: Header, Module, SubModule or Component.</summary>
        public string? Type { get; set; }
    }
}
