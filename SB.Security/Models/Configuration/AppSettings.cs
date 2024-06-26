﻿using SB.EmailService.Models;

namespace SB.Security.Models.Configuration
{
    /// <summary>
    /// This AppSettings used to read the appsettings.json's AppSettings object attributes.
    /// </summary>
    public class AppSettings
    {
        public string? AppDB { get; set; }
        public ConnectionStrings? ConnectionStrings { get; set; }
        public string? ConnectionProvider { get; set; }
        public double AccessTokenExpireTime { get; set; }
        public int Expires { get; set; }
        public JWT? JWT { get; set; }
        public int MaxNumberOfFailedAttempts { get; set; }
        public int BlockMinutes { get; set; }
        public EmailConfiguration? EmailConfiguration { get; set; }
        public string? EncryptKey { get; set; }
        public string? EncryptIV { get; set; }
        public bool IsUserDelate { get; set; } 
    }
}
