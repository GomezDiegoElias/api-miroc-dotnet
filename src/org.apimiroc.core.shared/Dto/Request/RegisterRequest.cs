namespace org.apimiroc.core.shared.Dto.Request
{
    public record RegisterRequest(
        long Dni,
        string Email,
        string Password,
        string FirstName,
        string LastName
    )
    { }
}
