namespace org.apimiroc.core.shared.Dto.Filter
{
    public class ConstructionFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public string? FName { get; set; }
        public string? FAddress { get; set; }
    }
}
