using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "tbl_movement",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "tbl_movement");
        }
    }
}
