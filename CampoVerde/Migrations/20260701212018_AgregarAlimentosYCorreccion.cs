using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlimentosYCorreccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notas",
                table: "Tareas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Encargado",
                table: "Tareas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "Producciones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "fechaProduccion",
                table: "Producciones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AlimentosBovinos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    CantidadDisponible = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Unidad = table.Column<int>(type: "integer", nullable: false),
                    NivelAlerta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlimentosBovinos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_IdAnimal",
                table: "Producciones",
                column: "IdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_Producciones_Animales_IdAnimal",
                table: "Producciones",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producciones_Animales_IdAnimal",
                table: "Producciones");

            migrationBuilder.DropTable(
                name: "AlimentosBovinos");

            migrationBuilder.DropIndex(
                name: "IX_Producciones_IdAnimal",
                table: "Producciones");

            migrationBuilder.DropColumn(
                name: "fechaProduccion",
                table: "Producciones");

            migrationBuilder.AlterColumn<string>(
                name: "Notas",
                table: "Tareas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Encargado",
                table: "Tareas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "Producciones",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
