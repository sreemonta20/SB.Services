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
        Task<DataResponse> GetUserByIdAsync(string id);
        Task<DataResponse> GetUserByIdAdoAsync(string id);
        Task<PageResult<UserInfo>> GetAllUserAsync(PaginationFilter paramRequest);
        Task<PagingResult<UserInfo>> GetAllUserExtnAsync(PaginationFilter paramRequest);
        Task<PagingResult<UserInfo>?> GetAllUserAdoAsync(PaginationFilter paramRequest);
        Task<DataResponse> RegisterUserAsync(UserRegisterRequest request);
        Task<DataResponse> DeleteUserAsync(string id);
    }
}
