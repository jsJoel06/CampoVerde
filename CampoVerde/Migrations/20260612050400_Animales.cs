using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class Animales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "especie",
                table: "Animales");

            migrationBuilder.AddColumn<string>(
                name: "lote",
                table: "Animales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "pesoActual",
                table: "Animales",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lote",
                table: "Animales");

            migrationBuilder.DropColumn(
                name: "pesoActual",
                table: "Animales");

            migrationBuilder.AddColumn<int>(
                name: "especie",
                table: "Animales",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
