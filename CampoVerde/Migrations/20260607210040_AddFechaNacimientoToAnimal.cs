using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaNacimientoToAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "especie",
                table: "Animales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "fechaNacimiento",
                table: "Animales",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "especie",
                table: "Animales");

            migrationBuilder.DropColumn(
                name: "fechaNacimiento",
                table: "Animales");
        }
    }
}
