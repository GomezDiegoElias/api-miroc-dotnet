namespace org.apimiroc.core.shared.Dto.Response.Movements
{
    public record MovementResponse(
        int CodeMovement,
        DateTime Date,
        decimal Amount,
        string PaymentMethod,
        string ConceptName,
        string ConceptType,
        string ConceptDescription,
        AssociatedEntity? AssociatedEntity
    ) { }
}
