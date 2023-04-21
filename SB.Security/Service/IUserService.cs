using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using Microsoft.AspNetCore.Identity;

namespace SB.Security.Service
{
    /// <summary>
    /// It define all the methods for user operations incuding the user login. Where <see cref="UserService"/> implements this methods.
    /// </summary>
    public interface IUserService
    {
        Task<DataResponse> GetUserAsync(string id);
        Task<DataResponse> GetUserTestAsync(string id);
        Task<PageResult<UserInfo>> GetAllUserAsync(PaginationFilter paramRequest);
        Task<DataResponse> AuthenticateUserAsync(LoginRequest request);
        Task<DataResponse> RegisterUserAsync(UserRegisterRequest request);
        Task<DataResponse> DeleteUserAsync(string id);
    }
}
