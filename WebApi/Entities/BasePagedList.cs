namespace WebApi.Entities
{
    public class BasePagedList
    {
        public string Search { get; set; }
        public bool? Descending { get; set; }
        public int Page { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public string SortBy { get; set; }
        public int TotalItems { get; set; }
    }
}