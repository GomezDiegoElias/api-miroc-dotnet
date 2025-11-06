using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class ModifySPConstructionPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getConstructionPaginationAdvanced;");
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getConstructionPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FName NVARCHAR(50) = NULL,
                    @FAddress NVARCHAR(100) = NULL,
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

                    ;WITH ConstructionData AS (
                        SELECT
                            c.id,
                            c.name,
                            c.startDate,
                            c.endDate,
                            c.address,
                            c.description,
                            c.client_id,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_construction c
                        JOIN tbl_client cl ON c.client_id = cl.id
                        WHERE c.is_deleted = 0
                            AND (@FName IS NULL OR c.name LIKE '%' + @FName + '%')
                            AND (@FAddress IS NULL OR c.address LIKE '%' + @FAddress + '%')
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        c.name LIKE '%' + st.Term + '%'
                                        OR c.address LIKE '%' + st.Term + '%'
                                        OR c.description LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM ConstructionData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'name' AND @SortDirection = 'ASC' THEN name END ASC,
                        CASE WHEN @SortField = 'name' AND @SortDirection = 'DESC' THEN name END DESC,
                        CASE WHEN @SortField = 'address' AND @SortDirection = 'ASC' THEN address END ASC,
                        CASE WHEN @SortField = 'address' AND @SortDirection = 'DESC' THEN address END DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getConstructionPaginationAdvanced;");
        }
    }
}
