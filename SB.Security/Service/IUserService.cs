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
        #region AppUser related methods
        Task<DataResponse> CreateUpdateAppUserAsync(AppUserRequest request);
        #endregion

        #region AppUserProfile related methods
        Task<PageResult<AppUserProfile>> GetAllAppUserProfileAsync(PaginationFilter paramRequest);
        Task<PagingResult<AppUserProfile>> GetAllAppUserProfileExtnAsync(PaginationFilter paramRequest);
        Task<PagingResult<AppUserProfile>?> GetAllAppUserProfileAdoAsync(PaginationFilter paramRequest);
        Task<DataResponse> GetAppUserProfileByIdAsync(string id);
        Task<DataResponse> GetAppUserProfileByIdAdoAsync(string id);
        Task<DataResponse> CreateUpdateAppUserProfileAsync(AppUserProfileRegisterRequest request);
        Task<DataResponse> DeleteAppUserProfileAsync(string id);
        #endregion
        
    }
}
