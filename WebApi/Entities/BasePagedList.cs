namespace WebApi.Entities
{
    public class BasePagedList
    {
        public string Search { get; set; }
        public bool? Descending { get; set; }
        public int? Page { get; set; }
        public int? RowsPerPage { get; set; }
        public string SortBy { get; set; }
        public int? TotalItems { get; set; }
    }
}