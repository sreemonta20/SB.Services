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

        Task<DataResponse> GetAllRolesAsync();
        Task<DataResponse> GetAllRolesPaginationAsync(PaginationFilter paramRequest);
        Task<DataResponse> GetRoleByIdAsync(string roleId);
        Task<DataResponse> SaveUpdateRoleAsync(RoleSaveUpdateRequest roleSaveUpdateRequest);
        Task<DataResponse> DeleteRoleAsync(string roleId);
        Task<DataResponse> GetAllMenuByUserIdAsync(string? userId);
        Task<PagingResult<AppUserMenu>> GetAllUserMenuPagingWithSearchAsync(PagingSearchFilter oPagingSearchFilter);
        Task<DataResponse> GetAllParentMenusAsync();
        
    }
}
