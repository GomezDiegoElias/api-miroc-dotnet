using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateSPEmployeePagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE getEmployeePaginationAdvanced
                    @PageIndex INT = 1,
                    @PageSize INT = 10,
                    @Q NVARCHAR(200) = NULL,
                    @FDni BIGINT = NULL,
                    @FFirstName NVARCHAR(100) = NULL,
                    @FLastName NVARCHAR(100) = NULL,
                    @FWorkStation NVARCHAR(100) = NULL,
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

                    ;WITH EmployeeData AS (
                        SELECT
                            e.id,
                            e.dni,
                            e.first_name,
                            e.last_name,
                            e.workstation,
                            e.is_deleted,
                            COUNT(*) OVER() AS TotalFilas
                        FROM tbl_employee e
                        WHERE e.is_deleted = 0
                            AND (@FDni IS NULL OR e.dni = @FDni)
                            AND (@FFirstName IS NULL OR e.first_name LIKE '%' + @FFirstName + '%')
                            AND (@FLastName IS NULL OR e.last_name LIKE '%' + @FLastName + '%')
                            AND (@FWorkStation IS NULL OR e.workstation LIKE '%' + @FWorkStation + '%')
                            AND (
                                @Q IS NULL
                                OR NOT EXISTS (
                                    SELECT 1
                                    FROM @SearchTerms st
                                    WHERE NOT (
                                        e.first_name LIKE '%' + st.Term + '%'
                                        OR e.last_name LIKE '%' + st.Term + '%'
                                        OR e.workstation LIKE '%' + st.Term + '%'
                                        OR CAST(e.dni AS NVARCHAR) LIKE '%' + st.Term + '%'
                                    )
                                )
                            )
                    )
                    SELECT *
                    FROM EmployeeData
                    ORDER BY
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'ASC' THEN id END ASC,
                        CASE WHEN @SortField = 'id' AND @SortDirection = 'DESC' THEN id END DESC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'ASC' THEN dni END ASC,
                        CASE WHEN @SortField = 'dni' AND @SortDirection = 'DESC' THEN dni END DESC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'ASC' THEN first_name END ASC,
                        CASE WHEN @SortField = 'first_name' AND @SortDirection = 'DESC' THEN first_name END DESC,
                        CASE WHEN @SortField = 'last_name' AND @SortDirection = 'ASC' THEN last_name END ASC,
                        CASE WHEN @SortField = 'last_name' AND @SortDirection = 'DESC' THEN last_name END DESC,
                        CASE WHEN @SortField = 'work_station' AND @SortDirection = 'ASC' THEN workstation END ASC,
                        CASE WHEN @SortField = 'work_station' AND @SortDirection = 'DESC' THEN workstation END DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS getEmployeePaginationAdvanced;");
        }
    }
}