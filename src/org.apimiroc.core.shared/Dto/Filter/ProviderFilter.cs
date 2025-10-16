namespace org.apimiroc.core.shared.Dto.Filter
{
    public class ProviderFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public long? FCuit { get; set; }
        public string? FFirstName { get; set; }
        public string? FDescription { get; set; }
        public string? FAddress { get; set; }
    }
}
