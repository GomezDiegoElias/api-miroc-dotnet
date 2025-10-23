using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableMovementWhitRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_concept",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_concept", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_movement",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    cod_movement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payment_method = table.Column<int>(type: "int", nullable: false),
                    concept_id = table.Column<int>(type: "int", nullable: false),
                    client_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    provider_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_movement", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_movement_tbl_client_client_id",
                        column: x => x.client_id,
                        principalTable: "tbl_client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_movement_tbl_concept_concept_id",
                        column: x => x.concept_id,
                        principalTable: "tbl_concept",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_movement_tbl_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "tbl_employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_movement_tbl_provider_provider_id",
                        column: x => x.provider_id,
                        principalTable: "tbl_provider",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_client_id",
                table: "tbl_movement",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_concept_id",
                table: "tbl_movement",
                column: "concept_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_employee_id",
                table: "tbl_movement",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_movement_provider_id",
                table: "tbl_movement",
                column: "provider_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_movement");

            migrationBuilder.DropTable(
                name: "tbl_concept");
        }
    }
}
