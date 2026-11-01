using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SBERP.HumanResources.Filter;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Service;
using SBERP.Shared.Authorization;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SBERP.HumanResources.Controllers.v1
{
    internal static class RequestOrigin
    {
        /// <summary>
        /// The caller's IP. X-Forwarded-For (set by the API gateway) is trusted only when the request
        /// comes from a trusted proxy (AppSettings:Attendance:TrustedProxies, default this machine),
        /// so a client cannot fake an office address by sending the header itself.
        /// </summary>
        public static string? ClientIp(HttpContext ctx, AppSettings? settings)
        {
            var remote = ctx.Connection.RemoteIpAddress;
            if (remote != null && remote.IsIPv4MappedToIPv6) remote = remote.MapToIPv4();
            string? remoteText = remote?.ToString();

            var trusted = (settings?.Attendance?.TrustedProxies ?? "127.0.0.1,::1")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (remote != null && trusted.Any(t => OfficeNetwork.InRange(remote, t)))
            {
                string? forwarded = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                string? first = forwarded?.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(first) && IPAddress.TryParse(first, out var ip))
                    return (ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip).ToString();
            }
            return remoteText;
        }
    }

    /// <summary>Employee biometric registration — mobile app (self) and HR (admin).</summary>
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.BiometricRegistrations)]
    public class BiometricRegistrationController : ControllerBase
    {
        private readonly IBiometricRegistrationService _service;
        private readonly IHRLogService _log;
        public BiometricRegistrationController(IBiometricRegistrationService service, IHRLogService log) { _service = service; _log = log; }

        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPost, Route(ConstantSupplier.REGISTER_MOBILE_BIOMETRIC_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [AuthenticatedOnly]
        public async Task<object> RegisterMobileBiometricAsync([FromBody] MobileBiometricRegistrationRequest request)
        {
            try { return await _service.RegisterMobileAsync(request, UserId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(RegisterMobileBiometricAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.VERIFY_BIOMETRIC_REGISTRATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [AuthenticatedOnly]
        public async Task<object> VerifyBiometricRegistrationAsync([FromBody] VerifyBiometricRegistrationRequest request)
        {
            try { return await _service.VerifyRegistrationAsync(request, UserId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(VerifyBiometricRegistrationAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_MY_BIOMETRIC_REGISTRATION_ROUTE)]
        [AuthenticatedOnly]
        public async Task<object> GetMyBiometricRegistrationAsync()
        {
            try { return await _service.GetMyRegistrationAsync(UserId); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetMyBiometricRegistrationAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_ALL_BIOMETRIC_REGISTRATIONS_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllBiometricRegistrationsPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllRegistrationsPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.BIO_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.BIO_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllBiometricRegistrationsPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_TERMINAL_ENROLLMENT_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateTerminalEnrollmentAsync([FromBody] TerminalEnrollmentRequest request)
        {
            try { return await _service.CreateTerminalEnrollmentAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateTerminalEnrollmentAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.REVOKE_BIOMETRIC_REGISTRATION_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.BiometricRegistrations, Action = ScreenAction.Delete)]
        public async Task<object> RevokeBiometricRegistrationAsync([FromBody] RevokeBiometricRegistrationRequest request)
        {
            try { return await _service.RevokeRegistrationAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(RevokeBiometricRegistrationAsync)); }
        }
    }

    /// <summary>
    /// Check-in / check-out. The mobile Identification API and the generic terminal API are
    /// anonymous because the caller proves itself differently: the phone signs a one-time challenge
    /// with its biometric-protected key; a terminal sends its device key.
    /// </summary>
    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Punches)]
    public class AttendancePunchController : ControllerBase
    {
        private readonly IAttendancePunchService _service;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;
        public AttendancePunchController(IAttendancePunchService service, IHRLogService log, IOptions<AppSettings> opts)
        { _service = service; _log = log; _settings = opts.Value; }

        [AllowAnonymous]
        [HttpPost, Route(ConstantSupplier.GET_BIOMETRIC_CHALLENGE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetBiometricChallengeAsync([FromBody] BiometricChallengeRequest request)
        {
            try { return await _service.GetChallengeAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetBiometricChallengeAsync)); }
        }

        [AllowAnonymous]
        [HttpPost, Route(ConstantSupplier.MOBILE_PUNCH_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> MobilePunchAsync([FromBody] MobilePunchRequest request)
        {
            try { return await _service.MobilePunchAsync(request, RequestOrigin.ClientIp(HttpContext, _settings)); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(MobilePunchAsync)); }
        }

        [AllowAnonymous]
        [HttpPost, Route(ConstantSupplier.DEVICE_PUNCHES_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> DevicePunchesAsync([FromBody] DevicePunchBatchRequest request, [FromHeader(Name = "X-Device-Key")] string? deviceKey)
        {
            try { return await _service.DevicePunchesAsync(request, deviceKey, RequestOrigin.ClientIp(HttpContext, _settings)); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DevicePunchesAsync)); }
        }

        [HttpGet, Route(ConstantSupplier.GET_ALL_ATTENDANCE_PUNCHES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAttendancePunchesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllPunchesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.BIO_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.BIO_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllAttendancePunchesPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.REPROCESS_ATTENDANCE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Punches, Action = ScreenAction.Update)]
        public async Task<object> ReprocessAttendanceAsync([FromBody] ReprocessAttendanceRequest request)
        {
            try { return await _service.ReprocessAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ReprocessAttendanceAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Devices)]
    public class AttendanceDeviceController : ControllerBase
    {
        private readonly IAttendanceDeviceService _service;
        private readonly IHRLogService _log;
        public AttendanceDeviceController(IAttendanceDeviceService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_ATTENDANCE_DEVICES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        [RequireScreen(Screens.Devices, Screens.BiometricRegistrations)]
        public async Task<object> GetAllAttendanceDevicesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllDevicesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.BIO_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.BIO_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllAttendanceDevicesPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_ATTENDANCE_DEVICE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAttendanceDeviceAsync([FromBody] AttendanceDeviceRequest request)
        {
            try { return await _service.CreateUpdateDeviceAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateAttendanceDeviceAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_ATTENDANCE_DEVICE_ROUTE)]
        public async Task<object> DeleteAttendanceDeviceAsync([FromQuery] string id)
        {
            try { return await _service.DeleteDeviceAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteAttendanceDeviceAsync)); }
        }
    }

    [ApiVersion("1.0")]
    [Authorize]
    [Route(ConstantSupplier.CTRLER_ROUTE_PATH_NAME_VERSION_ONE)]
    [ApiController]
    [EnableCors(ConstantSupplier.CORSS_POLICY_NAME)]
    [RequireScreen(Screens.Zones)]
    public class AttendanceZoneController : ControllerBase
    {
        private readonly IAttendanceZoneService _service;
        private readonly IHRLogService _log;
        public AttendanceZoneController(IAttendanceZoneService service, IHRLogService log) { _service = service; _log = log; }

        [HttpGet, Route(ConstantSupplier.GET_ALL_ATTENDANCE_ZONES_PAGING_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> GetAllAttendanceZonesPagingWithSearchAsync([FromQuery] string param)
        {
            try
            {
                var filter = JsonConvert.DeserializeObject<PagingSearchFilter>(param) ?? new PagingSearchFilter();
                var page = await _service.GetAllZonesPagingWithSearchAsync(filter);
                if (page == null) return Utilities.Warn(ConstantSupplier.BIO_LIST_EMPTY, code: (int)HttpStatusCode.NotFound);
                return Utilities.Ok(ConstantSupplier.BIO_FETCH_SUCCESS, page, (int)HttpStatusCode.Found);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetAllAttendanceZonesPagingWithSearchAsync)); }
        }

        [HttpPost, Route(ConstantSupplier.CREATE_UPDATE_ATTENDANCE_ZONE_ROUTE)]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<object> CreateUpdateAttendanceZoneAsync([FromBody] AttendanceZoneRequest request)
        {
            try { return await _service.CreateUpdateZoneAsync(request); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateAttendanceZoneAsync)); }
        }

        [HttpDelete, Route(ConstantSupplier.DELETE_ATTENDANCE_ZONE_ROUTE)]
        public async Task<object> DeleteAttendanceZoneAsync([FromQuery] string id)
        {
            try { return await _service.DeleteZoneAsync(id); }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteAttendanceZoneAsync)); }
        }
    }

    /// <summary>
    /// ZKTeco ADMS / Push SDK endpoint. In the terminal's "Cloud Server" settings enter the
    /// server address; the terminal then calls GET /iclock/cdata (handshake), POST /iclock/cdata
    /// (punch records) and GET /iclock/getrequest (asks for commands). The terminal is recognised by
    /// its serial number (SN) registered on the Attendance Device screen, optionally its fixed IP.
    /// </summary>
    [ApiVersionNeutral]
    [AllowAnonymous]
    [Route("iclock")]
    [ApiController]
    public class IClockController : ControllerBase
    {
        private const int MaxBodyBytes = 1024 * 1024;
        private readonly IAttendancePunchService _service;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;
        public IClockController(IAttendancePunchService service, IHRLogService log, IOptions<AppSettings> opts)
        { _service = service; _log = log; _settings = opts.Value; }

        private ContentResult Text(string body, int status = 200) => new() { Content = body, ContentType = "text/plain", StatusCode = status };

        [HttpGet("cdata")]
        public async Task<IActionResult> HandshakeAsync([FromQuery(Name = "SN")] string? sn)
        {
            try
            {
                var reply = await _service.AdmsHandshakeAsync(sn, RequestOrigin.ClientIp(HttpContext, _settings));
                return reply == null ? Text("Unknown device", 401) : Text(reply);
            }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(HandshakeAsync)); return Text("ERROR", 500); }
        }

        [HttpPost("cdata")]
        public async Task<IActionResult> UploadAsync([FromQuery(Name = "SN")] string? sn, [FromQuery] string? table)
        {
            try
            {
                if (Request.ContentLength > MaxBodyBytes) return Text("Too large", 413);
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();
                if (body.Length > MaxBodyBytes) return Text("Too large", 413);
                var reply = await _service.AdmsUploadAsync(sn, table, body, RequestOrigin.ClientIp(HttpContext, _settings));
                return reply == null ? Text("Unknown device", 401) : Text(reply);
            }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(UploadAsync)); return Text("ERROR", 500); }
        }

        /// <summary>No server-to-terminal commands are sent.</summary>
        [HttpGet("getrequest")]
        public IActionResult GetRequest() => Text("OK");

        [HttpPost("devicecmd")]
        public IActionResult DeviceCommand() => Text("OK");
    }
}
