namespace org.apimiroc.core.shared.Dto.General
{
    public record StandardResponse<T>(
            bool Success,
            string Message,
            T? Data = default,
            ErrorDetails? Error = null,
            int Status = 200
        )
    { }
}
