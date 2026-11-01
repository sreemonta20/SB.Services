namespace SBERP.Security.Models.Request
{
    public class AppUserRequest
    {
        public string? Id { get; set; }
        public string? AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? CreateUpdatedBy { get; set; }
        public bool? IsActive { get; set; }
        public string? ActionName { get; set; }
        
    }
}
