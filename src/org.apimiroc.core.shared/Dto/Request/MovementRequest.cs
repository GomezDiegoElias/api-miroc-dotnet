namespace org.apimiroc.core.shared.Dto.Request
{
    public record MovementRequest(
        decimal Amount,
        string PaymentMethod,
        int ConceptId,
        long? ClientDni,
        long? ProviderCuit,
        long? EmployeeDni,
        string? ConstructionName
    ) { }
}
