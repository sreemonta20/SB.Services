using SBERP.Security.Enum;

namespace SBERP.Security.Models.Response
{
    /// <summary>
    /// This is the important class for passing the response into front-end.
    /// </summary>
    public class DataResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public EnumResponseType MessageType { get; set; }
        public int ResponseCode { get; set; }
        public object? Result { get; set; }
    }
}
