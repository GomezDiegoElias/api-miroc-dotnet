namespace org.apimiroc.core.shared.Dto.Filter
{
    public class ClientFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public long? FDni { get; set; }
        public string? FFirstName { get; set; }
        public string? FAddress { get; set; }
    }
}
