using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;

namespace SB.Security.Service
{
    
    /// <summary>
    /// It define all the asynchronous methods for role and menu operation incuding the user GetAllRoles, GetAllRoles using pagination, specific role by id, save or update role, and delete role. 
    /// Where <see cref="RoleMenuService"/> implements this methods.
    /// </summary>
    public interface IRoleMenuService
    {

        #region AppUserRole related methods
        Task<DataResponse> GetAllAppUserRolesAsync();
        Task<DataResponse> GetAllAppUserRolesPaginationAsync(PaginationFilter paramRequest);
        Task<DataResponse> GetAppUserRolesByIdAsync(string roleId);
        Task<DataResponse> CreateUpdateAppUserRoleAsync(RoleSaveUpdateRequest roleSaveUpdateRequest);
        Task<DataResponse> DeleteAppUserRoleAsync(string roleId);
        #endregion

        #region AppUserMenu related methods
        Task<PagingResult<AppUserMenu>> GetAllAppUserMenuPagingWithSearchAsync(PagingSearchFilter oPagingSearchFilter);
        Task<DataResponse> GetAllAppUserMenuByUserIdAsync(string? userId);
        Task<DataResponse> CreateUpdateAppUserMenuAsync(AppUserMenuRequest request);
        Task<DataResponse> DeleteAppUserMenuAsync(string menuId);
        Task<DataResponse> GetAllParentMenusAsync();
        #endregion

        #region AppUserRoleMenu related methods
        Task<DataResponse> GetAppUserRoleMenuInitialDataAsync();
        Task<PagingResult<AppUserRoleMenuResponse>> GetAllAppUserRoleMenusPagingWithSearchAsync(PagingSearchFilter paramRequest);
        #endregion

    }
}
