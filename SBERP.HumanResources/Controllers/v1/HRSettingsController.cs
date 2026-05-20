using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Service;
using System.Security.Claims;

namespace SBERP.HumanResources.Controllers.v1
{
    /// <summary>
    /// Org-wide HR settings and attendance upload entry point.
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    public class HRSettingsController : ControllerBase
    {
        private readonly IHRSettingsService _service;
        private readonly IHRLogService _log;

        public HRSettingsController(IHRSettingsService service, IHRLogService log)
        {
            _service = service; _log = log;
        }

        [HttpGet]
        [Route(ConstantSupplier.GET_HR_SETTINGS_ROUTE)]
        public async Task<object> GetHRSettingsAsync()
        {
            try { return await _service.GetHRSettingsAsync(); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetHRSettingsAsync)); }
        }

        [HttpPost]
        [Route(ConstantSupplier.SAVE_UPDATE_HR_SETTINGS_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> SaveUpdateHRSettingsAsync([FromBody] HRSettingsRequest request)
        {
            try { return await _service.SaveUpdateHRSettingsAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SaveUpdateHRSettingsAsync)); }
        }

        /// <summary>
        /// Upload an attendance Excel. Multipart form with field name "file".
        /// Header row should be: EmployeeCode | Date | CheckIn | CheckOut | Status
        /// </summary>
        [HttpPost]
        [Route(ConstantSupplier.UPLOAD_ATTENDANCE_EXCEL_ROUTE)]
        [Consumes("multipart/form-data")]
        public async Task<object> UploadAttendanceExcelAsync(IFormFile file)
        {
            try
            {
                var uploadedBy = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? User?.Identity?.Name
                                 ?? "system";
                return await _service.UploadAttendanceExcelAsync(file, uploadedBy);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(UploadAttendanceExcelAsync)); }
        }
    }
}
