using SB.Security.Models.Request;
using SB.Security.Models.Response;

namespace SB.Security.Service
{
    /// <summary>
    /// It define all the methods for authentication operations incuding the login, RefreshToken, and Revoke. Where <see cref="AuthService"/> implements this methods.
    /// </summary>
    public interface IAuthService
    {
        //Task<DataResponse> AuthenticateUserAsync(LoginRequest? request);
        Task<DataResponse> AuthenticateUserAsync(LoginRequest? request);
        Task<DataResponse> RefreshTokenAsync(RefreshTokenRequest? refreshTokenReq);
        Task<DataResponse> RevokeAsync(string userName);
    }
}
