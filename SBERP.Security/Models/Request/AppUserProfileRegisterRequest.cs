using SBERP.Security.Helper;

namespace SBERP.Security.Models.Request
{
    /// <summary>
    /// UserRegisterRequest is extension of  <see cref="UserInfo"/>.
    /// </summary>
    public class AppUserProfileRegisterRequest
    {
        //public string? ActionName { get; set; }
        //public string? Id { get; set; }
        //public string? FullName { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }
        //public string? Email { get; set; }
        //public string RoleId { get; set; }
        //public Boolean? IsActive { get; set; } = true;
        public string? Id { get; set; }
        public string? ActionName { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? AppUserRoleId { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }

        // AppUser fields
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
