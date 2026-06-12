using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class Animal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagen",
                table: "Animales",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagen",
                table: "Animales");
        }
    }
}
