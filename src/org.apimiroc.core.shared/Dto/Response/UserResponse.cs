namespace org.apimiroc.core.shared.Dto.Response
{
    public record UserResponse(
        string Id,
        long Dni,
        string Email,
        string FirstName,
        string LastName,
        string Role,
        string Status
    )
    { }
}
