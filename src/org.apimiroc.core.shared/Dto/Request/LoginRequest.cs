namespace org.apimiroc.core.shared.Dto.Request
{
    public record LoginRequest(
        string Email,
        string Password
    )
    { }
}
