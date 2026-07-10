using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarIngreso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // COMENTAMOS ESTAS 3 LÍNEAS porque están causando el error de "Constraint not found"
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_Animales_AnimalIdAnimal",
                table: "Gastos");

            migrationBuilder.DropIndex(
                name: "IX_Gastos_AnimalIdAnimal",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "AnimalIdAnimal",
                table: "Gastos");
            */

            // Mantenemos el resto del código tal cual:
            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_IdAnimal",
                table: "Ingresos",
                column: "IdAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_IdAnimal",
                table: "Gastos",
                column: "IdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_Animales_IdAnimal",
                table: "Gastos",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingresos_Animales_IdAnimal",
                table: "Ingresos",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_Animales_IdAnimal",
                table: "Gastos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingresos_Animales_IdAnimal",
                table: "Ingresos");

            migrationBuilder.DropIndex(
                name: "IX_Ingresos_IdAnimal",
                table: "Ingresos");

            migrationBuilder.DropIndex(
                name: "IX_Gastos_IdAnimal",
                table: "Gastos");

            migrationBuilder.AddColumn<int>(
                name: "AnimalIdAnimal",
                table: "Gastos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_AnimalIdAnimal",
                table: "Gastos",
                column: "AnimalIdAnimal");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_Animales_AnimalIdAnimal",
                table: "Gastos",
                column: "AnimalIdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
