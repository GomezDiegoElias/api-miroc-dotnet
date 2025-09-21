namespace org.apimiroc.core.shared.Dto.Request
{
    public record UserUpdateRequest(
        long Dni,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Status,
        string Role
    )
    { }
}
