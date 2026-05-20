using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Service;
using Newtonsoft.Json;
using System.Net;

namespace SBERP.HumanResources.Helper
{
    /// <summary>
    /// Static helpers used across HR services. Same shape as
    /// SBERP.Security.Helper.Utilities so devs jumping between services
    /// don't lose their place.
    /// </summary>
    public static class Utilities
    {
        public static bool IsNull<T>(T? obj) => obj == null;

        public static bool IsNull<T>(List<T>? list) => list == null || list.Count == 0;

        public static bool IsNotNull<T>(T? obj) => obj != null;

        public static bool IsNullOrEmptyOrWhiteSpace(string? value)
            => string.IsNullOrWhiteSpace(value ?? string.Empty);

        /// <summary>
        /// Detect SQL Server unique-key violation (2601 / 2627). Useful when
        /// catching DbUpdateException on Employee Code, Department Code, etc.
        /// </summary>
        public static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;
            return false;
        }

        /// <summary>
        /// Detect SQL Server FK violation (547) — used when a delete is blocked
        /// because child rows still exist (e.g. employees in a department).
        /// </summary>
        public static bool IsForeignKeyViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 547;
            return false;
        }

        public static DataResponse Fail(string message, int code = (int)HttpStatusCode.BadRequest,
                                        object? result = null)
            => new()
            {
                Success = false,
                Message = message,
                MessageType = Enum.EnumResponseType.Error,
                ResponseCode = code,
                Result = result
            };

        public static DataResponse Ok(string message, object? result = null,
                                      int code = (int)HttpStatusCode.OK)
            => new()
            {
                Success = true,
                Message = message,
                MessageType = Enum.EnumResponseType.Success,
                ResponseCode = code,
                Result = result
            };

        public static DataResponse Warn(string message, object? result = null,
                                        int code = (int)HttpStatusCode.BadRequest)
            => new()
            {
                Success = false,
                Message = message,
                MessageType = Enum.EnumResponseType.Warning,
                ResponseCode = code,
                Result = result
            };

        public static DataResponse Exception(Exception ex, IHRLogService log,
                                             string apiName)
        {
            log.LogError(string.Format(ConstantSupplier.LOG_API_EXCEPTION,
                apiName, JsonConvert.SerializeObject(ex.Message, Formatting.Indented)));
            log.LogError(JsonConvert.SerializeObject(ex, Formatting.Indented));
            return new DataResponse
            {
                Success = false,
                Message = ex.Message,
                MessageType = Enum.EnumResponseType.Error,
                ResponseCode = (int)HttpStatusCode.InternalServerError,
                Result = null
            };
        }
    }
}
