namespace org.apimiroc.core.shared.Dto.Response.Movements
{
    public record TotalSummaryOfMovements(
        decimal TotalIncome,
        decimal TotalExpense,
        decimal NetBalance
    );
}
