namespace SB.Security.Models.Response
{
    public class PagingResult<T>
    {
        public int RowCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public List<T>? Items { get; set; }
    }
}
