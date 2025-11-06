namespace org.apimiroc.core.shared.Dto.Response
{
    public record ConstructionResponseV2(
        string Name,
        DateTime StartDate,
        DateTime EndDate,
        string Address,
        string Description,
        string ClientId
    )
    { }
}
