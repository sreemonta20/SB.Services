using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace SBERP.Security.Helper
{
    /// <summary>
    /// JSON for log messages only: secrets (passwords, salts, tokens) are written as "***".
    /// <para>
    /// Requests are logged with <c>JsonConvert.SerializeObject(request)</c>, which wrote login and
    /// user passwords and refresh tokens to the log files and the SecurityLog table. Requests are bound
    /// by Newtonsoft too, so [JsonIgnore] on the models is not an option; use this for every log line.
    /// </para>
    /// </summary>
    public static class LogSafe
    {
        private static readonly HashSet<string> SensitiveNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Password", "ConfirmPassword", "OldPassword", "NewPassword", "CurrentPassword", "SaltKey",
            "RefreshToken", "Refresh_Token", "Access_Token", "AccessToken", "Token", "TokenHash", "ReplacedByTokenHash"
        };

        private static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new MaskingContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string Serialize(object? value, Formatting formatting = Formatting.Indented)
        {
            try
            {
                return JsonConvert.SerializeObject(value, formatting, Settings);
            }
            catch (Exception ex)
            {
                return $"<not serializable: {ex.Message}>";
            }
        }

        private sealed class MaskingContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (property.PropertyName != null && SensitiveNames.Contains(property.PropertyName) && property.ValueProvider != null)
                    property.ValueProvider = new MaskingValueProvider(property.ValueProvider);
                return property;
            }
        }

        private sealed class MaskingValueProvider(IValueProvider inner) : IValueProvider
        {
            public object? GetValue(object target) => inner.GetValue(target) == null ? null : "***";

            public void SetValue(object target, object? value) => inner.SetValue(target, value);
        }
    }
}
