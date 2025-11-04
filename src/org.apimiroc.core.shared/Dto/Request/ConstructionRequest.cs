namespace org.apimiroc.core.shared.Dto.Request
{
    public record ConstructionRequest(
        string Name,
        DateTime StartDate,
        DateTime EndDate,
        string Address,
        string Description,
        string ClientId
    )
    { }
}
