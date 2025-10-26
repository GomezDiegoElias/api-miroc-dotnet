namespace org.apimiroc.core.shared.Dto.Request
{
    public record MovementRequest(
        decimal Amount,
        string PaymentMethod,
        int ConceptId,
        string? ClientId,
        string? ProviderId,
        string? EmployeeId,
        string? ConstructionId
    ) { }
}
