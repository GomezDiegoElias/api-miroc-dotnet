using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatioMovementWhitConstruction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "construction_id",
                table: "tbl_movement",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_construction_id",
                table: "tbl_movement",
                column: "construction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_movement_tbl_construction_construction_id",
                table: "tbl_movement",
                column: "construction_id",
                principalTable: "tbl_construction",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_movement_tbl_construction_construction_id",
                table: "tbl_movement");

            migrationBuilder.DropIndex(
                name: "IX_tbl_movement_construction_id",
                table: "tbl_movement");

            migrationBuilder.DropColumn(
                name: "construction_id",
                table: "tbl_movement");
        }
    }
}
