using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarTransaccionesAnimales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionesAnimales_Animales_IdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropIndex(
                name: "IX_TransaccionesAnimales_IdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.AlterColumn<int>(
                name: "IdAnimal",
                table: "TransaccionesAnimales",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AnimalIdAnimal",
                table: "TransaccionesAnimales",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "TransaccionesAnimales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreAnimal",
                table: "TransaccionesAnimales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "TransaccionesAnimales",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proveedor",
                table: "TransaccionesAnimales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Raza",
                table: "TransaccionesAnimales",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesAnimales_AnimalIdAnimal",
                table: "TransaccionesAnimales",
                column: "AnimalIdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionesAnimales_Animales_AnimalIdAnimal",
                table: "TransaccionesAnimales",
                column: "AnimalIdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionesAnimales_Animales_AnimalIdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropIndex(
                name: "IX_TransaccionesAnimales_AnimalIdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "AnimalIdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "NombreAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "Proveedor",
                table: "TransaccionesAnimales");

            migrationBuilder.DropColumn(
                name: "Raza",
                table: "TransaccionesAnimales");

            migrationBuilder.AlterColumn<int>(
                name: "IdAnimal",
                table: "TransaccionesAnimales",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesAnimales_IdAnimal",
                table: "TransaccionesAnimales",
                column: "IdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionesAnimales_Animales_IdAnimal",
                table: "TransaccionesAnimales",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
