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
        Task<PageResult<AppUserProfile>> GetAllUserAsync(PaginationFilter paramRequest);
        Task<PagingResult<AppUserProfile>> GetAllUserExtnAsync(PaginationFilter paramRequest);
        Task<PagingResult<AppUserProfile>?> GetAllUserAdoAsync(PaginationFilter paramRequest);
        Task<DataResponse> GetUserByIdAsync(string id);
        Task<DataResponse> GetUserByIdAdoAsync(string id);
        Task<DataResponse> CreateUpdateAppUserProfileAsync(AppUserProfileRegisterRequest request);
        Task<DataResponse> DeleteAppUserProfileAsync(string id);
        Task<DataResponse> CreateUpdateAppUserAsync(AppUserRequest request);
    }
}
