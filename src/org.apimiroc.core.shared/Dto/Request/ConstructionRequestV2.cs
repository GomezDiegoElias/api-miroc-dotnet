namespace org.apimiroc.core.shared.Dto.Request
{
    public record ConstructionRequestV2(
        string Name,
        DateTime StartDate,
        DateTime EndDate,
        string Address,
        string Description,
        string ClientId
    )
    { }
}
