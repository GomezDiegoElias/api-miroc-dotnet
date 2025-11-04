using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPMovementPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getMovementPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FDateFrom DATETIME = NULL,
                    @FDateTo DATETIME = NULL,
                    @FPaymentMethod NVARCHAR(50) = NULL,
                    @FCode INT = NULL,
                    @Sort NVARCHAR(50) = 'date,desc'
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @Offset INT = (@PageSize * (@PageIndex - 1));

                    DECLARE @SortField NVARCHAR(50) = 'date';
                    DECLARE @SortDirection NVARCHAR(4) = 'DESC';

                    IF @Sort IS NOT NULL AND CHARINDEX(',', @Sort) > 0
                    BEGIN
                        SET @SortField = LTRIM(RTRIM(LEFT(@Sort, CHARINDEX(',', @Sort) - 1)));
                        SET @SortDirection = UPPER(LTRIM(RTRIM(SUBSTRING(@Sort, CHARINDEX(',', @Sort) + 1, LEN(@Sort)))));
                    END

                    -- Procesamiento de palabras para búsqueda avanzada
                    DECLARE @SearchTerms TABLE (Term NVARCHAR(200));
                    IF @Q IS NOT NULL AND LTRIM(RTRIM(@Q)) <> ''
                    BEGIN
                        DECLARE @Temp NVARCHAR(MAX) = REPLACE(@Q, '+', ' ');
                        DECLARE @Pos INT;
                        DECLARE @Word NVARCHAR(200);

                        WHILE LEN(@Temp) > 0
                        BEGIN
                            SET @Pos = CHARINDEX(' ', @Temp);
                            IF @Pos > 0
                            BEGIN
                                SET @Word = LTRIM(RTRIM(SUBSTRING(@Temp, 1, @Pos - 1)));
                                SET @Temp = LTRIM(RTRIM(SUBSTRING(@Temp, @Pos + 1, LEN(@Temp))));
                            END
                            ELSE
                            BEGIN
                                SET @Word = LTRIM(RTRIM(@Temp));
                                SET @Temp = '';
                            END

                            IF (@Word <> '')
                                INSERT INTO @SearchTerms (Term) VALUES (@Word);
                        END
                    END

                    ;WITH MovementData AS 
                    (
                        SELECT
                            -- Movement
                            m.cod_movement,
                            m.amount,
                            m.date,
                            m.payment_method,

                            -- Concept
                            m.concept_id,
                            c.name AS concept_name,
                            c.type AS concept_type,
                            c.description AS concept_description,

                            -- Associated entities
                            m.client_id,
                            m.provider_id,
                            m.employee_id,
                            m.construction_id,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_movement m
                        INNER JOIN tbl_concept c ON m.concept_id = c.id
                        LEFT JOIN tbl_client cli ON m.client_id = cli.id
                        LEFT JOIN tbl_provider prv ON m.provider_id = prv.id
                        LEFT JOIN tbl_employee emp ON m.employee_id = emp.id
                        LEFT JOIN tbl_construction con ON m.construction_id = con.id
                        WHERE m.is_deleted != 1
                            AND (@FCode IS NULL OR m.cod_movement = @FCode)
                            AND (@FPaymentMethod IS NULL OR m.payment_method = @FPaymentMethod)
                            AND (@FDateFrom IS NULL OR m.date >= @FDateFrom)
                            AND (@FDateTo IS NULL OR m.date <= @FDateTo)
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        c.name LIKE '%' + st.Term + '%'
                                        OR c.description LIKE '%' + st.Term + '%'
                                        OR m.payment_method LIKE '%' + st.Term + '%'
                                        OR CAST(m.cod_movement AS NVARCHAR) LIKE '%' + st.Term + '%'
                                        OR CAST(m.amount AS NVARCHAR) LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM MovementData
                    ORDER BY
                        CASE WHEN @SortField = 'cod_movement' AND @SortDirection = 'ASC' THEN cod_movement END ASC,
                        CASE WHEN @SortField = 'cod_movement' AND @SortDirection = 'DESC' THEN cod_movement END DESC,

                        CASE WHEN @SortField = 'date' AND @SortDirection = 'ASC' THEN date END ASC,
                        CASE WHEN @SortField = 'date' AND @SortDirection = 'DESC' THEN date END DESC,

                        CASE WHEN @SortField = 'amount' AND @SortDirection = 'ASC' THEN amount END ASC,
                        CASE WHEN @SortField = 'amount' AND @SortDirection = 'DESC' THEN amount END DESC

                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getMovementPaginationAdvanced;");
        }
    }
}
