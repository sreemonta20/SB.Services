using SB.Security.Helper;

namespace SB.Security.Models.Request
{
    /// <summary>
    /// UserRegisterRequest is extension of  <see cref="UserInfo"/>.
    /// </summary>
    public class UserRegisterRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        //public string UserRole { get; set; } = ConstantSupplier.USER;  
        public string RoleId { get; set; }
        public Boolean? IsActive { get; set; } = true;
    }
}
