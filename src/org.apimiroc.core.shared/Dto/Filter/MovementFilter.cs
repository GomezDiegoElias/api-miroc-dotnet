namespace org.apimiroc.core.shared.Dto.Filter
{
    public class MovementFilter : PaginationFilter
    {

        public string? Q { get; set; }
        public DateTime? FDateFrom { get; set; }
        public DateTime? FDateTo { get; set; }
        public string? FPaymentMethod { get; set; }
        public int? FCode { get; set; }

    }
}
