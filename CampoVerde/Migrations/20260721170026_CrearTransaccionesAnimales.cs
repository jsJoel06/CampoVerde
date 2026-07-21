using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class CrearTransaccionesAnimales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionAnimal_Animales_IdAnimal",
                table: "TransaccionAnimal");

            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionAnimal_Clientes_ClienteId",
                table: "TransaccionAnimal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransaccionAnimal",
                table: "TransaccionAnimal");

            migrationBuilder.RenameTable(
                name: "TransaccionAnimal",
                newName: "TransaccionesAnimales");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionAnimal_IdAnimal",
                table: "TransaccionesAnimales",
                newName: "IX_TransaccionesAnimales_IdAnimal");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionAnimal_ClienteId",
                table: "TransaccionesAnimales",
                newName: "IX_TransaccionesAnimales_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransaccionesAnimales",
                table: "TransaccionesAnimales",
                column: "IdTransaccion");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionesAnimales_Animales_IdAnimal",
                table: "TransaccionesAnimales",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionesAnimales_Clientes_ClienteId",
                table: "TransaccionesAnimales",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionesAnimales_Animales_IdAnimal",
                table: "TransaccionesAnimales");

            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionesAnimales_Clientes_ClienteId",
                table: "TransaccionesAnimales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransaccionesAnimales",
                table: "TransaccionesAnimales");

            migrationBuilder.RenameTable(
                name: "TransaccionesAnimales",
                newName: "TransaccionAnimal");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionesAnimales_IdAnimal",
                table: "TransaccionAnimal",
                newName: "IX_TransaccionAnimal_IdAnimal");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionesAnimales_ClienteId",
                table: "TransaccionAnimal",
                newName: "IX_TransaccionAnimal_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransaccionAnimal",
                table: "TransaccionAnimal",
                column: "IdTransaccion");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionAnimal_Animales_IdAnimal",
                table: "TransaccionAnimal",
                column: "IdAnimal",
                principalTable: "Animales",
                principalColumn: "IdAnimal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionAnimal_Clientes_ClienteId",
                table: "TransaccionAnimal",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
