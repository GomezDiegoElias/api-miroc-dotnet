namespace org.apimiroc.core.shared.Dto.Filter
{
    public class UserFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public long? FDni { get; set; }
        public string? FEmail { get; set; }
        public string? FFirstName { get; set; }
        public string? FLastName { get; set; }
    }
}
