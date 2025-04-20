namespace SBERP.Security.Models.Request
{
    public class AppUserRequest
    {
        //public string? ActionName { get; set; }
        //public string? Id { get; set; }
        //public string? FullName { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }
        //public string? Email { get; set; } 
        //public string? RoleId { get; set; }
        //public bool? IsActive { get; set; } = true;
        public string? Id { get; set; }
        public string? AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        //public string? SaltKey { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? CreateUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public string? ActionName { get; set; }
        
    }
}
