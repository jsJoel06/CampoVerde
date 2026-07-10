using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "Vacunas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "AnimalIdAnimal",
                table: "Gastos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vacunas_IdAnimal",
                table: "Vacunas",
                column: "IdAnimal");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Vacunas_Animales_IdAnimal",
                table: "Vacunas",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_Animales_AnimalIdAnimal",
                table: "Gastos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacunas_Animales_IdAnimal",
                table: "Vacunas");

            migrationBuilder.DropIndex(
                name: "IX_Vacunas_IdAnimal",
                table: "Vacunas");

            migrationBuilder.DropIndex(
                name: "IX_Gastos_AnimalIdAnimal",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "AnimalIdAnimal",
                table: "Gastos");

            migrationBuilder.AlterColumn<string>(
                name: "observaciones",
                table: "Vacunas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
