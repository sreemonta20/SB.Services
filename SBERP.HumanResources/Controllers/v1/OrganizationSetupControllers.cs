using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Service;
using System.Net;

namespace SBERP.HumanResources.Controllers.v1
{
    // Same shape as DepartmentController — uses the project's existing
    // ConstantSupplier / Utilities / IHRLogService / ValidateModelAttribute
    // directly, since this is the same project, not a new microservice.
    // Add the route-name constants below into ConstantSupplier alongside
    // GET_ALL_DEPARTMENTS_ROUTE etc.
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;
        private readonly IHRLogService _log;

        public CompanyController(ICompanyService service, IHRLogService log)
        {
            _service = service; _log = log;
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_COMPANIES_ROUTE)]
        public async Task<object> GetAllCompaniesAsync()
        {
            try { return await _service.GetAllCompaniesAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCompaniesAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_COMPANIES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllCompaniesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllCompaniesPagingWithSearchAsync(filter);

                if (page == null)
                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.COMPANY_LIST_EMPTY,
                        MessageType = Enum.EnumResponseType.Warning,
                        ResponseCode = (int)HttpStatusCode.NotFound
                    };

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.COMPANY_FETCH_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.Found,
                    Result = page
                };
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCompaniesPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_COMPANY_BY_ID_ROUTE)]
        public async Task<object> GetCompanyByIdAsync([FromQuery] string id)
        {
            try { return await _service.GetCompanyByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetCompanyByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_COMPANY_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateCompanyAsync([FromBody] CompanyRequest request)
        {
            try { return await _service.CreateUpdateCompanyAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateCompanyAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_COMPANY_ROUTE)]
        public async Task<object> DeleteCompanyAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteCompanyAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteCompanyAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _service;
        private readonly IHRLogService _log;

        public BranchController(IBranchService service, IHRLogService log)
        {
            _service = service; _log = log;
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_BRANCHES_ROUTE)]
        public async Task<object> GetAllBranchesAsync()
        {
            try { return await _service.GetAllBranchesAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllBranchesAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_BRANCHES_BY_COMPANY_ROUTE)]
        public async Task<object> GetAllBranchesByCompanyIdAsync([FromQuery] string companyId)
        {
            try { return await _service.GetAllBranchesByCompanyIdAsync(companyId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllBranchesByCompanyIdAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_BRANCHES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllBranchesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllBranchesPagingWithSearchAsync(filter);

                if (page == null)
                    return new DataResponse
                    {
                        Success = false,
                        Message = ConstantSupplier.BRANCH_LIST_EMPTY,
                        MessageType = Enum.EnumResponseType.Warning,
                        ResponseCode = (int)HttpStatusCode.NotFound
                    };

                return new DataResponse
                {
                    Success = true,
                    Message = ConstantSupplier.BRANCH_FETCH_SUCCESS,
                    MessageType = Enum.EnumResponseType.Success,
                    ResponseCode = (int)HttpStatusCode.Found,
                    Result = page
                };
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllBranchesPagingWithSearchAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_BRANCH_BY_ID_ROUTE)]
        public async Task<object> GetBranchByIdAsync([FromQuery] string id)
        {
            try { return await _service.GetBranchByIdAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetBranchByIdAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_BRANCH_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateBranchAsync([FromBody] BranchRequest request)
        {
            try { return await _service.CreateUpdateBranchAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateBranchAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_BRANCH_ROUTE)]
        public async Task<object> DeleteBranchAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteBranchAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteBranchAsync)); }
        }
    }
}
