using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class ClienteIdOpcional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Vacunas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Tareas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Producciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Potreros",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Partos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Ingresos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Gastos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Animales",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransaccionAnimal",
                columns: table => new
                {
                    IdTransaccion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    IdAnimal = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Tercero = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaccionAnimal", x => x.IdTransaccion);
                    table.ForeignKey(
                        name: "FK_TransaccionAnimal_Animales_IdAnimal",
                        column: x => x.IdAnimal,
                        principalTable: "Animales",
                        principalColumn: "IdAnimal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransaccionAnimal_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacunas_ClienteId",
                table: "Vacunas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_ClienteId",
                table: "Tareas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_ClienteId",
                table: "Producciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Potreros_ClienteId",
                table: "Potreros",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Partos_ClienteId",
                table: "Partos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_ClienteId",
                table: "Ingresos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_ClienteId",
                table: "Gastos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Animales_ClienteId",
                table: "Animales",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionAnimal_ClienteId",
                table: "TransaccionAnimal",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionAnimal_IdAnimal",
                table: "TransaccionAnimal",
                column: "IdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_Animales_Clientes_ClienteId",
                table: "Animales",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_Clientes_ClienteId",
                table: "Gastos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Clientes_ClienteId",
                table: "Ingresos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partos_Clientes_ClienteId",
                table: "Partos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Potreros_Clientes_ClienteId",
                table: "Potreros",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Producciones_Clientes_ClienteId",
                table: "Producciones",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Clientes_ClienteId",
                table: "Tareas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacunas_Clientes_ClienteId",
                table: "Vacunas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animales_Clientes_ClienteId",
                table: "Animales");

            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_Clientes_ClienteId",
                table: "Gastos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Clientes_ClienteId",
                table: "Ingresos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partos_Clientes_ClienteId",
                table: "Partos");

            migrationBuilder.DropForeignKey(
                name: "FK_Potreros_Clientes_ClienteId",
                table: "Potreros");

            migrationBuilder.DropForeignKey(
                name: "FK_Producciones_Clientes_ClienteId",
                table: "Producciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Clientes_ClienteId",
                table: "Tareas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacunas_Clientes_ClienteId",
                table: "Vacunas");

            migrationBuilder.DropTable(
                name: "TransaccionAnimal");

            migrationBuilder.DropIndex(
                name: "IX_Vacunas_ClienteId",
                table: "Vacunas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_ClienteId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Producciones_ClienteId",
                table: "Producciones");

            migrationBuilder.DropIndex(
                name: "IX_Potreros_ClienteId",
                table: "Potreros");

            migrationBuilder.DropIndex(
                name: "IX_Partos_ClienteId",
                table: "Partos");

            migrationBuilder.DropIndex(
                name: "IX_Ingresos_ClienteId",
                table: "Ingresos");

            migrationBuilder.DropIndex(
                name: "IX_Gastos_ClienteId",
                table: "Gastos");

            migrationBuilder.DropIndex(
                name: "IX_Animales_ClienteId",
                table: "Animales");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Vacunas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Producciones");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Potreros");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Partos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Animales");
        }
    }
}
