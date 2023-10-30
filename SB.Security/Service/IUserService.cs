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

        //Task<DataResponse> GetAppUserByIdAsync(string id);
        //Task<DataResponse> GetAppUserByIdAdoAsync(string id);
        //Task<PageResult<AppUserProfile>> GetAllAppUserAsync(PaginationFilter paramRequest);
        //Task<PagingResult<AppUserProfile>> GetAllAppUserExtnAsync(PaginationFilter paramRequest);
        //Task<PagingResult<AppUserProfile>?> GetAllAppUserAdoAsync(PaginationFilter paramRequest);
        //Task<DataResponse> RegisterAppUserAsync(UserRegisterRequest request);
        //Task<DataResponse> DeleteAppUserAsync(string id);
    }
}
