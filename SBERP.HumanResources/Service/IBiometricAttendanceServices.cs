using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;

namespace SBERP.HumanResources.Service
{
    /// <summary>Employee biometric enrolment — the mobile app's registration API plus HR's admin actions.</summary>
    public interface IBiometricRegistrationService
    {
        Task<DataResponse> RegisterMobileAsync(MobileBiometricRegistrationRequest request, string? userId);
        Task<DataResponse> VerifyRegistrationAsync(VerifyBiometricRegistrationRequest request, string? userId);
        Task<DataResponse> GetMyRegistrationAsync(string? userId);
        Task<PagingResult<BiometricRegistrationResponse>?> GetAllRegistrationsPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateTerminalEnrollmentAsync(TerminalEnrollmentRequest request);
        Task<DataResponse> RevokeRegistrationAsync(RevokeBiometricRegistrationRequest request);
    }

    /// <summary>Check-in / check-out intake (Identification API, door terminals) and the punch log.</summary>
    public interface IAttendancePunchService
    {
        Task<DataResponse> GetChallengeAsync(BiometricChallengeRequest request);
        Task<DataResponse> MobilePunchAsync(MobilePunchRequest request, string? clientIp);
        Task<DataResponse> DevicePunchesAsync(DevicePunchBatchRequest request, string? deviceKey, string? clientIp);
        /// <summary>ZKTeco ADMS handshake (GET /iclock/cdata). Null = unknown device.</summary>
        Task<string?> AdmsHandshakeAsync(string? serialNumber, string? clientIp);
        /// <summary>ZKTeco ADMS upload (POST /iclock/cdata). Null = unknown device.</summary>
        Task<string?> AdmsUploadAsync(string? serialNumber, string? table, string body, string? clientIp);
        Task<PagingResult<AttendancePunchResponse>?> GetAllPunchesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> ReprocessAsync(ReprocessAttendanceRequest request);
    }

    public interface IAttendanceDeviceService
    {
        Task<PagingResult<AttendanceDeviceResponse>?> GetAllDevicesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateDeviceAsync(AttendanceDeviceRequest request);
        Task<DataResponse> DeleteDeviceAsync(string id);
    }

    public interface IAttendanceZoneService
    {
        Task<PagingResult<AttendanceZoneResponse>?> GetAllZonesPagingWithSearchAsync(PagingSearchFilter filter);
        Task<DataResponse> CreateUpdateZoneAsync(AttendanceZoneRequest request);
        Task<DataResponse> DeleteZoneAsync(string id);
    }

    /// <summary>Builds the daily dbo.Attendances row from the accepted punches.</summary>
    public interface IAttendanceProcessor
    {
        Task RecalculateDayAsync(Guid employeeId, DateTime attendanceDate);
        Task<int> RecalculateRangeAsync(DateTime fromDate, DateTime toDate, Guid? employeeId);
    }
}
