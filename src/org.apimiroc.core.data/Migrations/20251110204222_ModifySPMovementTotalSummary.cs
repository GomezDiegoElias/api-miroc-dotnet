using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class ModifySPMovementTotalSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getTotalSummaryOfMovements;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getTotalSummaryOfMovements
                AS
                BEGIN
                    SET NOCOUNT ON;
    
                    SELECT 
                        SUM(CASE WHEN c.type = 'Ingreso' THEN m.amount ELSE 0 END) as TotalIncome,
                        SUM(CASE WHEN c.type = 'Egreso' THEN m.amount ELSE 0 END) as TotalExpense,
                        SUM(CASE WHEN c.type = 'Ingreso' THEN m.amount ELSE 0 END) - 
                        SUM(CASE WHEN c.type = 'Egreso' THEN m.amount ELSE 0 END) as NetBalance
                    FROM tbl_movement m
                    INNER JOIN tbl_concept c ON m.concept_id = c.id
                    WHERE m.is_deleted != 1 AND c.is_deleted != 1
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getTotalSummaryOfMovements;");
        }
    }
}
