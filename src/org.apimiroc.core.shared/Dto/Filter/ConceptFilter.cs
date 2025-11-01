namespace org.apimiroc.core.shared.Dto.Filter
{
    public class ConceptFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public string? FName { get; set; }
        public string? FType { get; set; }
    }
}
