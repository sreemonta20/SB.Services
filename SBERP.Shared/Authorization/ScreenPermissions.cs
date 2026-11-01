using System.Security.Claims;

namespace SBERP.Shared.Authorization
{
    /// <summary>What an endpoint does to a screen's data.</summary>
    [Flags]
    public enum ScreenAction
    {
        /// <summary>Taken from the HTTP verb: GET = View, POST / PUT = Create or Update, DELETE = Delete.</summary>
        FromHttpMethod = 0,
        View = 1,
        Create = 2,
        Update = 4,
        Delete = 8
    }

    /// <summary>Custom claim types of the SBERP access token.</summary>
    public static class SberpClaims
    {
        /// <summary>The role's rights on each screen; see <see cref="ScreenPermissions"/>.</summary>
        public const string Permissions = "perm";
        /// <summary>AppUserProfiles.Id of the signed-in user.</summary>
        public const string ProfileId = "pid";
        /// <summary>The employee linked to the signed-in user's profile, when there is one.</summary>
        public const string EmployeeId = "emp";
    }

    /// <summary>
    /// The screen rights carried by the access token, in the form
    /// <c>humanresources/corehr/employee=VCUD;home=V</c>.
    /// <para>
    /// They travel in the token so that every service can check them without a database or a network
    /// call. A change to a role reaches users when their short-lived access token is renewed.
    /// </para>
    /// </summary>
    public static class ScreenPermissions
    {
        private const string RoutePrefix = "/business/";

        /// <summary>Turns a RouteLink ("/business/home/") into the screen key ("home").</summary>
        public static string ScreenKey(string routeLink)
        {
            string key = routeLink.Trim().TrimEnd('/').ToLowerInvariant();
            return key.StartsWith(RoutePrefix, StringComparison.Ordinal) ? key[RoutePrefix.Length..] : key.TrimStart('/');
        }

        public static string Encode(IEnumerable<(string RouteLink, ScreenAction Actions)> grants)
        {
            // one role can reach a screen through several rows: their rights add up
            var merged = new SortedDictionary<string, ScreenAction>(StringComparer.Ordinal);
            foreach (var (routeLink, actions) in grants)
            {
                string key = ScreenKey(routeLink);
                if (key.Length == 0 || actions == ScreenAction.FromHttpMethod) continue;
                merged[key] = merged.GetValueOrDefault(key) | actions;
            }
            return string.Join(';', merged.Select(g => $"{g.Key}={Letters(g.Value)}"));
        }

        public static Dictionary<string, ScreenAction> Decode(string? claim)
        {
            var result = new Dictionary<string, ScreenAction>(StringComparer.Ordinal);
            if (string.IsNullOrEmpty(claim)) return result;
            foreach (string entry in claim.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                int separator = entry.IndexOf('=');
                if (separator <= 0) continue;
                ScreenAction actions = default;
                foreach (char letter in entry[(separator + 1)..])
                    actions |= letter switch
                    {
                        'V' => ScreenAction.View,
                        'C' => ScreenAction.Create,
                        'U' => ScreenAction.Update,
                        'D' => ScreenAction.Delete,
                        _ => default
                    };
                result[entry[..separator]] = actions;
            }
            return result;
        }

        /// <summary>True when the user holds at least one of <paramref name="required"/> on the screen.</summary>
        public static bool HasScreen(this ClaimsPrincipal user, string screen, ScreenAction required) =>
            Decode(user.FindFirst(SberpClaims.Permissions)?.Value).TryGetValue(screen, out ScreenAction granted)
            && (granted & required) != 0;

        /// <summary>True when the id is the signed-in user's own AppUserProfile.</summary>
        public static bool IsOwnProfile(this ClaimsPrincipal user, string? profileId) =>
            Guid.TryParse(profileId, out Guid wanted)
            && Guid.TryParse(user.FindFirst(SberpClaims.ProfileId)?.Value, out Guid own)
            && wanted == own;

        private static string Letters(ScreenAction actions) => string.Concat(
            actions.HasFlag(ScreenAction.View) ? "V" : "",
            actions.HasFlag(ScreenAction.Create) ? "C" : "",
            actions.HasFlag(ScreenAction.Update) ? "U" : "",
            actions.HasFlag(ScreenAction.Delete) ? "D" : "");
    }
}
