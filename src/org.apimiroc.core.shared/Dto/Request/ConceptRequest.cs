namespace org.apimiroc.core.shared.Dto.Request
{
    public record ConceptRequest(
        string Name,
        string Type,
        string? Description
    ) { }
}
