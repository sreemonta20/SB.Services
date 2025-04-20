using SBERP.Security.Models.Response;

namespace SBERP.Security.Service
{
    public interface IDataAnalyticsService
    {
        Task<DataResponse> GetTotalDataInfo();
    }
}
