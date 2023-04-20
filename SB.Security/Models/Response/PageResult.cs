namespace SB.Security.Models.Response
{
    /// <summary>
    /// PageResult generic class holder. It track the current page number, page size, and list of user details item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T>
    {
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T>? Items { get; set; }
    }
}
