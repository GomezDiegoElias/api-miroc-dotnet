namespace org.apimiroc.core.shared.Dto.Response
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken
    )
    { }
}
