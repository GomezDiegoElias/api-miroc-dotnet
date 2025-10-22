namespace org.apimiroc.core.shared.Dto.Response
{
    public record ConstrucctionResponse(
        string Name,
        DateTime StartDate,
        DateTime EndDate,
        string Address,
        string Description
    )
    { }
}
