namespace org.apimiroc.core.shared.Dto.Response
{
    public record ProviderResponse(
        long Cuit,
        string FirstName,
        string Address,
        string Description
    )
    { }
}
