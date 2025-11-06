namespace org.apimiroc.core.shared.Dto.Response
{
    public record ConstructionResponse(
        string Name,
        DateTime StartDate,
        DateTime EndDate,
        string Address,
        string Description,
        long ClientDni
    )
    { }
}
