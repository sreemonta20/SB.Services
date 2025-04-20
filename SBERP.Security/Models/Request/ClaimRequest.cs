namespace SBERP.Security.Models.Request
{
    public class ClaimRequest
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
