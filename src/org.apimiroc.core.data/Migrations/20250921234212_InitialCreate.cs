using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace org.apimiroc.core.data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_client",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    dni = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_client", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_employee",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    dni = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    workstation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_employee", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_role_permission_tbl_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "tbl_permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_role_permission_tbl_role_role_id",
                        column: x => x.role_id,
                        principalTable: "tbl_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    dni = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_user_tbl_role_role_id",
                        column: x => x.role_id,
                        principalTable: "tbl_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_client_dni",
                table: "tbl_client",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_employee_dni",
                table: "tbl_employee",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_permission_permission_id",
                table: "tbl_role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_permission_role_id",
                table: "tbl_role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_dni",
                table: "tbl_user",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_email",
                table: "tbl_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_role_id",
                table: "tbl_user",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_client");

            migrationBuilder.DropTable(
                name: "tbl_employee");

            migrationBuilder.DropTable(
                name: "tbl_role_permission");

            migrationBuilder.DropTable(
                name: "tbl_user");

            migrationBuilder.DropTable(
                name: "tbl_permission");

            migrationBuilder.DropTable(
                name: "tbl_role");
        }
    }
}
