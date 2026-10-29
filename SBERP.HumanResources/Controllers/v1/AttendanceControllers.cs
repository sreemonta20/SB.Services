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
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _service;
        private readonly IHRLogService _log;
        public ShiftController(IShiftService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_SHIFTS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllShiftsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllShiftsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.SHIFT_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.SHIFT_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllShiftsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_SHIFT_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateShiftAsync([FromBody] ShiftRequest request)
        {
            try { return await _service.CreateUpdateShiftAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateShiftAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_SHIFT_ROUTE)]
        public async Task<object> DeleteShiftAsync([FromQuery] string id)
        {
            try { return await _service.DeleteShiftAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteShiftAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class RosterController : ControllerBase
    {
        private readonly IRosterService _service;
        private readonly IHRLogService _log;
        public RosterController(IRosterService service, IHRLogService log) { _service = service; _log = log; }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_ROSTER_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateRosterAsync([FromBody] ShiftRosterRequest request)
        {
            try { return await _service.CreateUpdateRosterAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateRosterAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_EMPLOYEE_ROSTER_ROUTE)]
        public async Task<object> GetEmployeeRosterAsync([FromQuery] string id)
        {
            try { return await _service.GetEmployeeRosterAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeRosterAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_ROSTER_ROUTE)]
        public async Task<object> DeleteRosterAsync([FromQuery] string id)
        {
            try { return await _service.DeleteRosterAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteRosterAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class OvertimeController : ControllerBase
    {
        private readonly IOvertimeService _service;
        private readonly IHRLogService _log;
        public OvertimeController(IOvertimeService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_OVERTIME_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllOvertimePagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllOvertimePagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.OVERTIME_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.OVERTIME_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllOvertimePagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_OVERTIME_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateOvertimeAsync([FromBody] OvertimeRequestRequest request)
        {
            try { return await _service.CreateUpdateOvertimeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateOvertimeAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_OVERTIME_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetOvertimeStatusAsync([FromBody] AttendanceApprovalRequest request)
        {
            try { return await _service.SetOvertimeStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetOvertimeStatusAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class RegularizationController : ControllerBase
    {
        private readonly IRegularizationService _service;
        private readonly IHRLogService _log;
        public RegularizationController(IRegularizationService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_REGULARIZATIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllRegularizationsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllRegularizationsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.REG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.REG_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllRegularizationsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_REGULARIZATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateRegularizationAsync([FromBody] RegularizationRequestRequest request)
        {
            try { return await _service.CreateUpdateRegularizationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateRegularizationAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.SET_REGULARIZATION_STATUS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SetRegularizationStatusAsync([FromBody] AttendanceApprovalRequest request)
        {
            try { return await _service.SetRegularizationStatusAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetRegularizationStatusAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class AttendanceSummaryController : ControllerBase
    {
        private readonly IAttendanceSummaryService _service;
        private readonly IHRLogService _log;
        public AttendanceSummaryController(IAttendanceSummaryService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_MONTHLY_SUMMARY_ROUTE)]
        public async Task<object> GetMonthlyAttendanceSummaryAsync([FromQuery] DateTime month, [FromQuery] string? departmentId, [FromQuery] string? employeeId)
        {
            try { return await _service.GetMonthlyAttendanceSummaryAsync(month, departmentId, employeeId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetMonthlyAttendanceSummaryAsync)); }
        }
    }
}
