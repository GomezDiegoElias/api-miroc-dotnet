using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPUserPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getUserPaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FDni BIGINT = NULL,
                    @FEmail NVARCHAR(100) = NULL,
                    @FFirstName NVARCHAR(100) = NULL,
                    @FLastName NVARCHAR(100) = NULL,
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

                    ;WITH UserData AS (
                        SELECT
                            u.id,
                            u.dni,
                            u.email,
                            u.first_name,
                            u.last_name,
                            r.name AS role,
                            u.status,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_user u
                        JOIN tbl_role r ON u.role_id = r.id
                        WHERE u.status != 'DELETED'
                            AND (@FDni IS NULL OR u.dni = @FDni)
                            AND (@FEmail IS NULL OR u.email LIKE '%' + @FEmail + '%')
                            AND (@FFirstName IS NULL OR u.first_name LIKE '%' + @FFirstName + '%')
                            AND (@FLastName IS NULL OR u.last_name LIKE '%' + @FLastName + '%')
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        u.email LIKE '%' + st.Term + '%'
                                        OR u.first_name LIKE '%' + st.Term + '%'
                                        OR u.last_name LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM UserData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'ASC' THEN dni END ASC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'DESC' THEN dni END DESC,
                        CASE WHEN @SortField = 'email' AND @SortDirection = 'ASC' THEN email END ASC,
                        CASE WHEN @SortField = 'email' AND @SortDirection = 'DESC' THEN email END DESC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'ASC' THEN first_name END ASC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'DESC' THEN first_name END DESC,
                        CASE WHEN @SortField = 'last_name' AND @SortDirection = 'ASC' THEN last_name END ASC,
                        CASE WHEN @SortField = 'last_name' AND @SortDirection = 'DESC' THEN last_name END DESC,
                        CASE WHEN @SortField = 'role' AND @SortDirection = 'ASC' THEN role END ASC,
                        CASE WHEN @SortField = 'role' AND @SortDirection = 'DESC' THEN role END DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getUserPaginationAdvanced;");
        }
    }
}