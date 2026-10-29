namespace SBERP.Security.Models.Request
{
    public class RevokeRequest
    {
        /// <summary>
        /// Refresh token of the session to sign out. Possession of it is the proof: the endpoint no
        /// longer accepts a user name, which let anyone sign out any user. The access token, when
        /// still valid, is read from the Authorization header and blacklisted.
        /// </summary>
        public string? Refresh_Token { get; set; }
    }
}
