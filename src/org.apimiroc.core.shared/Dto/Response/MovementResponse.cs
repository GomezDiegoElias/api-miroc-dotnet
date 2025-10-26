namespace org.apimiroc.core.shared.Dto.Response
{
    public record MovementResponse(
        int CodeMovement,
        DateTime Date,
        decimal Amount,
        string PaymentMethod,
        string ConceptName
    ) { }
}
