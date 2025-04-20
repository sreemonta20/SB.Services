using SBERP.Security.Helper;

namespace SBERP.Security.Models.Configuration
{
    /// <summary>
    /// This brief user class is used to send the user data through the <see cref="Token"/> class.
    /// </summary>
    public class User
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? UserRole { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
