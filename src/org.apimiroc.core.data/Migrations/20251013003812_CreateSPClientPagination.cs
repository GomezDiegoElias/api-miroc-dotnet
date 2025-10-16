using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPClientPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getClientPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FDni BIGINT = NULL,
                    @FFirstName NVARCHAR(100) = NULL,
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

                    ;WITH ClientData AS (
                        SELECT
                            c.id,
                            c.dni,
                            c.first_name,
                            c.address,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_client c
                        WHERE c.is_deleted != 1
                            AND (@FDni IS NULL OR c.dni = @FDni)
                            AND (@FFirstName IS NULL OR c.first_name LIKE '%' + @FFirstName + '%')
                            AND (@FAddress IS NULL OR c.address LIKE '%' + @FAddress + '%')
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        c.first_name LIKE '%' + st.Term + '%'
                                        OR c.address LIKE '%' + st.Term + '%'
                                        OR CAST(c.dni AS NVARCHAR) LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM ClientData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'ASC' THEN dni END ASC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'DESC' THEN dni END DESC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'ASC' THEN first_name END ASC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'DESC' THEN first_name END DESC,
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
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getClientPaginationAdvanced;");
        }
    }
}