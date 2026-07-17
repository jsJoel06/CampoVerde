using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class ClienteN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "AlimentosBovinos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlimentosBovinos_ClienteId",
                table: "AlimentosBovinos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlimentosBovinos_Clientes_ClienteId",
                table: "AlimentosBovinos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlimentosBovinos_Clientes_ClienteId",
                table: "AlimentosBovinos");

            migrationBuilder.DropIndex(
                name: "IX_AlimentosBovinos_ClienteId",
                table: "AlimentosBovinos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "AlimentosBovinos");
        }
    }
}
