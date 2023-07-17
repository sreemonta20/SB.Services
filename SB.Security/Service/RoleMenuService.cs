using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SB.DataAccessLayer;
using SB.EmailService.Service;
using SB.Security.Helper;
using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using SB.Security.Models.Request;
using SB.Security.Models.Response;
using SB.Security.Persistence;
using System.Net;

namespace SB.Security.Service
{
    /// <summary>
    /// includes all the asynchronous methods for role and menu operation incuding the user GetAllRoles, GetAllRoles using pagination, specific role by id, save or update role, and delete role. 
    /// It implements  <see cref="IRoleMenuService"/>.
    /// </summary>
    public class RoleMenuService : IRoleMenuService
    {
        #region Variable declaration & constructor initialization
        public IConfiguration _configuration;
        private readonly SBSecurityDBContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings? _appSettings;
        private readonly ISecurityLogService _securityLogService;
        private readonly IDatabaseManager _dbmanager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="context"></param>
        /// <param name="emailService"></param>
        /// <param name="options"></param>
        /// <param name="securityLogService"></param>
        public RoleMenuService(IConfiguration config, SBSecurityDBContext context, IEmailService emailService, IOptions<AppSettings> options,
        ISecurityLogService securityLogService, IDatabaseManager dbManager)
        {
            _configuration = config;
            _context = context;
            _emailService = emailService;
            _appSettings = options.Value;
            _securityLogService = securityLogService;
            _dbmanager = dbManager;
            _dbmanager.InitializeDatabase(_appSettings?.ConnectionStrings?.ProdSqlConnectionString, _appSettings?.ConnectionProvider);
        }
        #endregion
        public async Task<DataResponse> GetAllRolesAsync()
        {
            DataResponse? oDataResponse = null;
            _securityLogService.LogInfo(String.Format(ConstantSupplier.SERVICE_GETALLROLES_REQ_MSG, JsonConvert.SerializeObject(null, Formatting.Indented)));
            try
            {
                List<UserRole> userRoleList = await _context.UserRole.OrderByDescending(x => x.RoleName).ToListAsync();
                if (userRoleList == null)
                {
                    oDataResponse = new DataResponse { Success = false, Message = ConstantSupplier.NO_ROLE_DATA, MessageType = Enum.EnumResponseType.Warning, ResponseCode = (int)HttpStatusCode.NotFound, Result = null };
                    _securityLogService.LogWarning(String.Format(ConstantSupplier.SERVICE_GETALLROLES_RES_MSG, JsonConvert.SerializeObject(oDataResponse, Formatting.Indented)));

                }
                else
                {
                    oDataResponse = new DataResponse { Success = true, Message = ConstantSupplier.SUCCESS_ROLE_DATA, MessageType = Enum.EnumResponseType.Success, ResponseCode = (int)HttpStatusCode.OK, Result = userRoleList };
                }
                
            }
            catch (Exception)
            {

                throw;
            }
            return oDataResponse;
        }

        public Task<PagingResult<UserInfo>> GetAllRolesPaginationAsync(PaginationFilter paramRequest)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse> GetRoleByIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse> SaveUpdateRoleAsync(RoleSaveUpdateRequest roleSaveUpdateRequest)
        {
            throw new NotImplementedException();
        }
        public Task<DataResponse> DeleteRoleAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse> GetAllMenuByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        
    }
}
