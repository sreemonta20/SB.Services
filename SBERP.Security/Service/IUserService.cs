using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Security.Models.Response;
using Microsoft.AspNetCore.Identity;

namespace SBERP.Security.Service
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
        Task<PagingResult<AppUserProfileResponse>?> GetAllAppUserProfilePagingWithSearchAsync(PagingSearchFilter paramRequest);
        Task<DataResponse> GetAppUserProfileByIdAsync(string id);
        Task<DataResponse> GetAppUserProfileByIdAdoAsync(string id);
        Task<DataResponse> CreateUpdateAppUserProfileAsync(AppUserProfileRegisterRequest request);
        Task<DataResponse> CreateUpdateAppUserProfileAsyncExtn(AppUserProfileRegisterRequest request);
        Task<DataResponse> DeleteAppUserProfileAsync(string id);
        #endregion

        #region AppUserProfile photo
        Task<DataResponse> UploadAppUserProfilePhotoAsync(string id, Stream content, long length, string? updatedBy);
        Task<SBERP.Shared.Storage.PhotoFile?> GetAppUserProfilePhotoAsync(string id);
        Task<DataResponse> RemoveAppUserProfilePhotoAsync(string id, string? updatedBy);
        #endregion

    }
}
