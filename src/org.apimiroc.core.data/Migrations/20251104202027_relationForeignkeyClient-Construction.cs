using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class relationForeignkeyClientConstruction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cient_id",
                table: "tbl_construction",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_construction_cient_id",
                table: "tbl_construction",
                column: "cient_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_construction_tbl_client_cient_id",
                table: "tbl_construction",
                column: "cient_id",
                principalTable: "tbl_client",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_construction_tbl_client_cient_id",
                table: "tbl_construction");

            migrationBuilder.DropIndex(
                name: "IX_tbl_construction_cient_id",
                table: "tbl_construction");

            migrationBuilder.DropColumn(
                name: "cient_id",
                table: "tbl_construction");
        }
    }
}
