namespace SBERP.HumanResources.Models.Configuration
{
    public class PagingSearchFilter
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string SortColumnName { get; set; } = string.Empty;
        public string SortColumnDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PagingSearchFilter() { }

        public PagingSearchFilter(string searchTerm, string sortColumnName,
            string sortColumnDirection, int pageNumber, int pageSize)
        {
            SearchTerm = searchTerm ?? string.Empty;
            SortColumnName = sortColumnName ?? string.Empty;
            SortColumnDirection = string.IsNullOrWhiteSpace(sortColumnDirection)
                ? "ASC" : sortColumnDirection;
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 100 ? 100 : (pageSize < 1 ? 10 : pageSize);
        }
    }
}
