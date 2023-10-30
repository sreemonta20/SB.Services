namespace SB.Security.Models.Request
{
    public class AppUserRegisterRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; } 
        public string? RoleId { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
