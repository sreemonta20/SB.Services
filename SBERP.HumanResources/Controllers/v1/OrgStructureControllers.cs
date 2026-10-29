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
    // Organization Setup (Module 0) — Cost Center, Job Grade, Holiday Calendar.
    // Company/Branch live in OrganizationSetupControllers.cs; Position in
    // RecruitmentControllers.cs (shared entity).

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class CostCenterController : ControllerBase
    {
        private readonly ICostCenterService _service;
        private readonly IHRLogService _log;
        public CostCenterController(ICostCenterService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_COSTCENTERS_ROUTE)]
        public async Task<object> GetAllCostCentersAsync()
        {
            try { return await _service.GetAllCostCentersAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCostCentersAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_COSTCENTERS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllCostCentersPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllCostCentersPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllCostCentersPagingWithSearchAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CREATE_UPDATE_COSTCENTER_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateCostCenterAsync([FromBody] CostCenterRequest request)
        {
            try { return await _service.CreateUpdateCostCenterAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateCostCenterAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_COSTCENTER_ROUTE)]
        public async Task<object> DeleteCostCenterAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteCostCenterAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteCostCenterAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class JobGradeController : ControllerBase
    {
        private readonly IJobGradeService _service;
        private readonly IHRLogService _log;
        public JobGradeController(IJobGradeService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_JOBGRADES_ROUTE)]
        public async Task<object> GetAllJobGradesAsync()
        {
            try { return await _service.GetAllJobGradesAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllJobGradesAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_JOBGRADES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllJobGradesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllJobGradesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllJobGradesPagingWithSearchAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CREATE_UPDATE_JOBGRADE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateJobGradeAsync([FromBody] JobGradeRequest request)
        {
            try { return await _service.CreateUpdateJobGradeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateJobGradeAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_JOBGRADE_ROUTE)]
        public async Task<object> DeleteJobGradeAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteJobGradeAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteJobGradeAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class HolidayCalendarController : ControllerBase
    {
        private readonly IHolidayCalendarService _service;
        private readonly IHRLogService _log;
        public HolidayCalendarController(IHolidayCalendarService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_HOLIDAYCALENDARS_ROUTE)]
        public async Task<object> GetAllHolidayCalendarsAsync()
        {
            try { return await _service.GetAllHolidayCalendarsAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllHolidayCalendarsAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_ALL_HOLIDAYCALENDARS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllHolidayCalendarsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllHolidayCalendarsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.ORG_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.ORG_FETCH, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllHolidayCalendarsPagingWithSearchAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CREATE_UPDATE_HOLIDAYCALENDAR_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateHolidayCalendarAsync([FromBody] HolidayCalendarRequest request)
        {
            try { return await _service.CreateUpdateHolidayCalendarAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateHolidayCalendarAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_HOLIDAYCALENDAR_ROUTE)]
        public async Task<object> DeleteHolidayCalendarAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteHolidayCalendarAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteHolidayCalendarAsync)); }
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_HOLIDAY_DATES_ROUTE)]
        public async Task<object> GetHolidayDatesByCalendarAsync([FromQuery] string calendarId)
        {
            try { return await _service.GetHolidayDatesAsync(calendarId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetHolidayDatesByCalendarAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.CREATE_UPDATE_HOLIDAY_DATE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateHolidayDateAsync([FromBody] HolidayDateRequest request)
        {
            try { return await _service.CreateUpdateHolidayDateAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateHolidayDateAsync)); }
        }

        [HttpDelete]
        [Route(ConstantSupplier.DELETE_HOLIDAY_DATE_ROUTE)]
        public async Task<object> DeleteHolidayDateAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return Utilities.Warn(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return await _service.DeleteHolidayDateAsync(id);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteHolidayDateAsync)); }
        }
    }
}
