namespace org.apimiroc.core.shared.Dto.General
{
    public record ErrorDetails(
            int StatusCode,
            string Message,
            string Path,
            string? Details
        )
    { }
}
