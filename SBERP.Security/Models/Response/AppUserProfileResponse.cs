namespace SBERP.Security.Models.Response
{
    public class AppUserProfileResponse
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Guid? AppUserRoleId { get; set; }
        public string? RoleName { get; set; }
        public string? UserName { get; set; } // Added for AppUser
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
