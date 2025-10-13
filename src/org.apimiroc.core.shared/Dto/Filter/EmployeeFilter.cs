namespace org.apimiroc.core.shared.Dto.Filter
{
    public class EmployeeFilter : PaginationFilter
    {
        public string? Q { get; set; }
        public long? FDni { get; set; }
        public string? FFirstName { get; set; }
        public string? FLastName { get; set; }
        public string? FWorkStation { get; set; }
    }
}
