using SB.EmailService.Models;

namespace SB.Security.Models.Configuration
{
    /// <summary>
    /// This AppSettings used to read the appsettings.json's AppSettings object attributes.
    /// </summary>
    public class AppSettings
    {
        public ConnectionStrings? ConnectionStrings { get; set; }
        public double AccessTokenExpireTime { get; set; }
        public JWT? JWT { get; set; }
        public int MaxNumberOfFailedAttempts { get; set; }
        public int BlockMinutes { get; set; }
        public EmailConfiguration? EmailConfiguration { get; set; }
        public string? EncryptKey { get; set; }
        public string? EncryptIV { get; set; }
    }
}
