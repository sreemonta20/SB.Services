using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SB.Security.Models.Request
{
    /// <summary>
    /// LoginRequest is extension of  <see cref="UserInfo"/>.
    /// </summary>
    public class LoginRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //[Required]
        //[JsonPropertyName("UserName")]
        public string? UserName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //[JsonPropertyName("Password")]
        //[Required]
        public string? Password { get; set; }
    }
}
