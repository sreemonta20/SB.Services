using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SBERP.Security.Models.Request
{
    /// <summary>
    /// LoginRequest is extension of  <see cref="UserInfo"/>.
    /// </summary>
    public class LoginRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? UserName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }
    }
}
