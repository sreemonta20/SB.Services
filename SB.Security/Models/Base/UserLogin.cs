using System.ComponentModel.DataAnnotations;

namespace SB.Security.Models.Base
{
    public class UserLogin
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
