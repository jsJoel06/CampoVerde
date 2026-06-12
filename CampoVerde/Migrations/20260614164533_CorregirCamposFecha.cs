using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class CorregirCamposFecha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fechaAplicacion",
                table: "Vacunas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "fechaProximaAplicacion",
                table: "Vacunas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fechaAplicacion",
                table: "Vacunas");

            migrationBuilder.DropColumn(
                name: "fechaProximaAplicacion",
                table: "Vacunas");
        }
    }
}
