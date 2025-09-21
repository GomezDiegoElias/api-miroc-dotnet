namespace org.apimiroc.core.shared.Dto.Request
{
    public record UserCreateRequest(
        long Dni,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Role
    )
    { }
}
