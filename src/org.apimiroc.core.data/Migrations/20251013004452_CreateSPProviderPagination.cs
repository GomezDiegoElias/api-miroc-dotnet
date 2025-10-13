using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPProviderPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getProviderPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FCuit BIGINT = NULL,
                    @FFirstName NVARCHAR(100) = NULL,
                    @FDescription NVARCHAR(200) = NULL,
                    @FAddress NVARCHAR(200) = NULL,
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

                    ;WITH ProviderData AS (
                        SELECT
                            p.id,
                            p.cuit,
                            p.first_name,
                            p.description,
                            p.address,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_provider p
                        WHERE p.is_deleted = 0
                            AND (@FCuit IS NULL OR p.cuit = @FCuit)
                            AND (@FFirstName IS NULL OR p.first_name LIKE '%' + @FFirstName + '%')
                            AND (@FDescription IS NULL OR p.description LIKE '%' + @FDescription + '%')
                            AND (@FAddress IS NULL OR p.address LIKE '%' + @FAddress + '%')
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        p.first_name LIKE '%' + st.Term + '%'
                                        OR p.description LIKE '%' + st.Term + '%'
                                        OR p.address LIKE '%' + st.Term + '%'
                                        OR CAST(p.cuit AS NVARCHAR) LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM ProviderData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'cuit' AND @SortDirection = 'ASC' THEN cuit END ASC,
                        CASE WHEN @SortField = 'cuit' AND @SortDirection = 'DESC' THEN cuit END DESC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'ASC' THEN first_name END ASC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'DESC' THEN first_name END DESC,
                        CASE WHEN @SortField = 'description' AND @SortDirection = 'ASC' THEN description END ASC,
                        CASE WHEN @SortField = 'description' AND @SortDirection = 'DESC' THEN description END DESC,
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
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getProviderPaginationAdvanced;");
        }
    }
}