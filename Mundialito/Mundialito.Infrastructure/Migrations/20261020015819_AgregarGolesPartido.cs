using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mundialito.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGolesPartido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GolesPartido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JugadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CantidadGoles = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GolesPartido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GolesPartido_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GolesPartido_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GolesPartido_JugadorId",
                table: "GolesPartido",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_GolesPartido_PartidoId",
                table: "GolesPartido",
                column: "PartidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GolesPartido");
        }
    }
}
