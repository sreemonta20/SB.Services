using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Models
{
    /// <summary>
    /// JWT configuration shared across ALL microservices.
    /// Values MUST be identical to SBERP.Security AppSettings:JWT.
    /// Add "SharedJwtSettings" block in every service's appsettings.json.
    /// </summary>
    public class JwtSharedSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
