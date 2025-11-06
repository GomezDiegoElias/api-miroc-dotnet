namespace org.apimiroc.core.shared.Dto.Response.Movements.V2
{
    public record MovementResponseV2(
        int CodeMovement,
        DateTime Date,
        decimal Amount,
        string PaymentMethod,
        string ConceptName,
        string ConceptType,
        string ConceptDescription,
        AssociatedEntityV2? AssociatedEntity
    ) { }
}
