using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using Microsoft.AspNetCore.Http;

namespace SBERP.HumanResources.Service
{
    public interface IHRSettingsService
    {
        Task<DataResponse> GetHRSettingsAsync();
        Task<DataResponse> SaveUpdateHRSettingsAsync(HRSettingsRequest request);
        Task<DataResponse> UploadAttendanceExcelAsync(IFormFile file, string uploadedBy);
    }
}
