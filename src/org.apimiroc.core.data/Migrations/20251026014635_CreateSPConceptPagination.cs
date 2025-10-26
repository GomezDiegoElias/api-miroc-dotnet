using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPConceptPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getConceptPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FName NVARCHAR(150) = NULL,
                    @FType NVARCHAR(20) = NULL,
                    @Sort NVARCHAR(50) = 'id,asc'
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @Offset INT = (@PageSize * (@PageIndex - 1));

                    DECLARE @SortField NVARCHAR(50) = 'id';
                    DECLARE @SortDirection NVARCHAR(4) = 'ASC';

                    IF @Sort IS NOT NULL AND CHARINDEX(',', @Sort) > 0
                    BEGIN
                        SET @SortField = LTRIM(RTRIM(LEFT(@Sort, CHARINDEX(',', @Sort) - 1)));
                        SET @SortDirection = UPPER(LTRIM(RTRIM(SUBSTRING(@Sort, CHARINDEX(',', @Sort) + 1, LEN(@Sort)))));
                    END

                    -- Procesamiento de Q (búsqueda múltiple con split)
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

                    ;WITH ConceptData AS (
                        SELECT
                            c.id,
                            c.name,
                            c.type,
                            c.description,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_concept c
                        WHERE
                            (@FName IS NULL OR c.name LIKE '%' + @FName + '%')
                            AND (@FType IS NULL OR c.type = @FType)
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        c.name LIKE '%' + st.Term + '%'
                                        OR c.description LIKE '%' + st.Term + '%'
                                        OR c.type LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM ConceptData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'name' AND @SortDirection = 'ASC' THEN name END ASC,
                        CASE WHEN @SortField = 'name' AND @SortDirection = 'DESC' THEN name END DESC,
                        CASE WHEN @SortField = 'type' AND @SortDirection = 'ASC' THEN type END ASC,
                        CASE WHEN @SortField = 'type' AND @SortDirection = 'DESC' THEN type END DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getConceptPaginationAdvanced;");
        }
    }
}
