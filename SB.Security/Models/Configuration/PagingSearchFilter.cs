namespace SB.Security.Models.Configuration
{
    public class PagingSearchFilter
    {
        public string SearchTerm { get; set; }
        public string SortColumnName { get; set; }
        public string SortColumnDirection { get; set; } 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PagingSearchFilter()
        {
            this.SearchTerm = string.Empty;
            this.SortColumnName = string.Empty;
            this.SortColumnDirection = string.Empty;
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PagingSearchFilter(string searchTerm, string sortColumnName, string sortColumnDirection, int pageNumber, int pageSize)
        {
            this.SearchTerm = searchTerm;
            this.SortColumnName = sortColumnName;
            this.SortColumnDirection = sortColumnDirection;
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
    
}
