namespace org.apimiroc.core.shared.Dto.Filter
{
    public class PaginationFilter
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Sort { get; set; } = "id,asc";
    }
}
