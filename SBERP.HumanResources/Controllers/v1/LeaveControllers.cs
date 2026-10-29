using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Service;
using System.Net;

namespace SBERP.HumanResources.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeService _service;
        private readonly IHRLogService _log;
        public LeaveTypeController(ILeaveTypeService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_LEAVE_TYPES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllLeaveTypesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllLeaveTypesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.LEAVE_TYPE_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.LEAVE_TYPE_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllLeaveTypesPagingWithSearchAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_ALL_LEAVE_TYPES_LOOKUP_ROUTE)]
        public async Task<object> GetLeaveTypesLookupAsync()
        {
            try { return await _service.GetLeaveTypesLookupAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetLeaveTypesLookupAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_LEAVE_TYPE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateLeaveTypeAsync([FromBody] LeaveTypeRequest request)
        {
            try { return await _service.CreateUpdateLeaveTypeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateLeaveTypeAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_LEAVE_TYPE_ROUTE)]
        public async Task<object> DeleteLeaveTypeAsync([FromQuery] string id)
        {
            try { return await _service.DeleteLeaveTypeAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteLeaveTypeAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class LeavePolicyController : ControllerBase
    {
        private readonly ILeavePolicyService _service;
        private readonly IHRLogService _log;
        public LeavePolicyController(ILeavePolicyService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_LEAVE_POLICIES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllLeavePoliciesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllLeavePoliciesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.LEAVE_POLICY_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.LEAVE_POLICY_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllLeavePoliciesPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_LEAVE_POLICY_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateLeavePolicyAsync([FromBody] LeavePolicyRequest request)
        {
            try { return await _service.CreateUpdateLeavePolicyAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateLeavePolicyAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_LEAVE_POLICY_ROUTE)]
        public async Task<object> DeleteLeavePolicyAsync([FromQuery] string id)
        {
            try { return await _service.DeleteLeavePolicyAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteLeavePolicyAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.GRANT_ENTITLEMENTS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GrantEntitlementsAsync([FromBody] GrantEntitlementsRequest request)
        {
            try { return await _service.GrantEntitlementsAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GrantEntitlementsAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class LeaveApplicationController : ControllerBase
    {
        private readonly ILeaveApplicationService _service;
        private readonly IHRLogService _log;
        public LeaveApplicationController(ILeaveApplicationService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_LEAVE_APPLICATIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllLeaveApplicationsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllLeaveApplicationsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.LEAVE_APP_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.LEAVE_APP_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllLeaveApplicationsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.APPLY_LEAVE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> ApplyLeaveAsync([FromBody] ApplyLeaveRequest request)
        {
            try { return await _service.ApplyLeaveAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ApplyLeaveAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_LEAVE_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetLeaveApplicationStatusAsync([FromBody] LeaveActionRequest request)
        {
            try { return await _service.SetLeaveApplicationStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetLeaveApplicationStatusAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_EMPLOYEE_BALANCES_ROUTE)]
        public async Task<object> GetEmployeeLeaveBalancesAsync([FromQuery] string id, [FromQuery] int year)
        {
            try { return await _service.GetEmployeeLeaveBalancesAsync(id, year); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeLeaveBalancesAsync)); }
        }
    }
}
