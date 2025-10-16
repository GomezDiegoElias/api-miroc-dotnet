namespace org.apimiroc.core.shared.Dto.Request
{
    public record ProviderRequest(
        long Cuit,
        string FirstName,
        string Address,
        string Description
    )
    { }
}
