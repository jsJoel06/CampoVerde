using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampoVerde.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Animales",
                columns: table => new
                {
                    IdAnimal = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    fechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    raza = table.Column<string>(type: "text", nullable: false),
                    pesoActual = table.Column<double>(type: "double precision", nullable: false),
                    lote = table.Column<string>(type: "text", nullable: false),
                    imagen = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animales", x => x.IdAnimal);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    IdGasto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: true),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Concepto = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.IdGasto);
                });

            migrationBuilder.CreateTable(
                name: "Ingresos",
                columns: table => new
                {
                    IdIngreso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: true),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Concepto = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingresos", x => x.IdIngreso);
                });

            migrationBuilder.CreateTable(
                name: "Potreros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    RutasFotos = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Potreros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    telefono = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Vacunas",
                columns: table => new
                {
                    IdVacuna = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: false),
                    nombreVacuna = table.Column<string>(type: "text", nullable: false),
                    frecuenciaMeses = table.Column<int>(type: "integer", nullable: false),
                    fechaAplicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fechaProximaAplicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacunas", x => x.IdVacuna);
                });

            migrationBuilder.CreateTable(
                name: "Partos",
                columns: table => new
                {
                    IdParto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: false),
                    FechaParto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NombreCria = table.Column<string>(type: "text", nullable: false),
                    CodigoCria = table.Column<string>(type: "text", nullable: false),
                    SexoCria = table.Column<string>(type: "text", nullable: false),
                    PesoCria = table.Column<double>(type: "double precision", nullable: false),
                    EstadoCria = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partos", x => x.IdParto);
                    table.ForeignKey(
                        name: "FK_Partos_Animales_IdAnimal",
                        column: x => x.IdAnimal,
                        principalTable: "Animales",
                        principalColumn: "IdAnimal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producciones",
                columns: table => new
                {
                    IdProduccion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: false),
                    fechaProduccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cantidadLeche = table.Column<double>(type: "double precision", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    Turno = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producciones", x => x.IdProduccion);
                    table.ForeignKey(
                        name: "FK_Producciones_Animales_IdAnimal",
                        column: x => x.IdAnimal,
                        principalTable: "Animales",
                        principalColumn: "IdAnimal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tareas",
                columns: table => new
                {
                    IdTarea = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAnimal = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Completada = table.Column<bool>(type: "boolean", nullable: false),
                    Prioridad = table.Column<int>(type: "integer", nullable: false),
                    Encargado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.IdTarea);
                    table.ForeignKey(
                        name: "FK_Tareas_Animales_IdAnimal",
                        column: x => x.IdAnimal,
                        principalTable: "Animales",
                        principalColumn: "IdAnimal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partos_IdAnimal",
                table: "Partos",
                column: "IdAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Producciones_IdAnimal",
                table: "Producciones",
                column: "IdAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_IdAnimal",
                table: "Tareas",
                column: "IdAnimal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlimentosBovinos");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "Ingresos");

            migrationBuilder.DropTable(
                name: "Partos");

            migrationBuilder.DropTable(
                name: "Potreros");

            migrationBuilder.DropTable(
                name: "Producciones");

            migrationBuilder.DropTable(
                name: "Tareas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Vacunas");

            migrationBuilder.DropTable(
                name: "Animales");
        }
    }
}
