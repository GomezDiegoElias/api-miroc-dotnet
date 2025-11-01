using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class AddMovementContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_cod_movement",
                table: "tbl_movement",
                column: "cod_movement",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_movement_cod_movement",
                table: "tbl_movement");
        }
    }
}
