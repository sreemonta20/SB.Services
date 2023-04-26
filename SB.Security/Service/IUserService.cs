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
        /// <summary>
        /// ADO.NET Codeblock: GetUserByIdAdoAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataResponse> GetUserByIdAdoAsync(string id);
        Task<PageResult<UserInfo>> GetAllUserAsync(PaginationFilter paramRequest);
        Task<PagingResult<UserInfo>> GetAllUserExtnAsync(PaginationFilter paramRequest);
        /// <summary>
        /// ADO.NET Codeblock: GetAllUserAdoAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PagingResult<UserInfo>?> GetAllUserAdoAsync(PaginationFilter paramRequest);
        Task<DataResponse> AuthenticateUserAsync(LoginRequest request);
        Task<DataResponse> RegisterUserAsync(UserRegisterRequest request);
        Task<DataResponse> DeleteUserAsync(string id);
    }
}
