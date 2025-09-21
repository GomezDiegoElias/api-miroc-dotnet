namespace org.apimiroc.core.shared.Dto.Request
{
    public record ClientRequest(
        long Dni,
        string FirstName,
        string Address
    )
    { }
}
