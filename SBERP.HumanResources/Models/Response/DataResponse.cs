using SBERP.HumanResources.Enum;

namespace SBERP.HumanResources.Models.Response
{
    /// <summary>
    /// Same shape as SBERP.Security.Models.Response.DataResponse — the
    /// frontend consumes both interchangeably.
    /// </summary>
    public class DataResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public EnumResponseType MessageType { get; set; }
        public int ResponseCode { get; set; }
        public object? Result { get; set; }
    }

    public class PagingResult<T>
    {
        public int RowCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public List<T>? Items { get; set; }
    }
}
