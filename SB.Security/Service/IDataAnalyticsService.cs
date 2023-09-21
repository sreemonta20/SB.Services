using SB.Security.Models.Response;

namespace SB.Security.Service
{
    public interface IDataAnalyticsService
    {
        Task<DataResponse> GetTotalDataInfo();
    }
}
