using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModeloVacuna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "frecuenciaMeses",
                table: "Vacunas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "frecuenciaMeses",
                table: "Vacunas");
        }
    }
}
