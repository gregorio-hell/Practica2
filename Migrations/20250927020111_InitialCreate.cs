using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace pc2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inmuebles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ciudad = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Dormitorios = table.Column<int>(type: "INTEGER", nullable: false),
                    Banos = table.Column<int>(type: "INTEGER", nullable: false),
                    MetrosCuadrados = table.Column<double>(type: "REAL", nullable: false),
                    Precio = table.Column<double>(type: "REAL", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inmuebles", x => x.Id);
                    table.CheckConstraint("CK_Inmueble_MetrosCuadrados", "MetrosCuadrados > 0");
                    table.CheckConstraint("CK_Inmueble_Precio", "Precio > 0");
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InmuebleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Inmuebles_InmuebleId",
                        column: x => x.InmuebleId,
                        principalTable: "Inmuebles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InmuebleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Notas = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitas", x => x.Id);
                    table.CheckConstraint("CK_Visita_Fechas", "FechaInicio < FechaFin");
                    table.ForeignKey(
                        name: "FK_Visitas_Inmuebles_InmuebleId",
                        column: x => x.InmuebleId,
                        principalTable: "Inmuebles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Inmuebles",
                columns: new[] { "Id", "Activo", "Banos", "Ciudad", "Codigo", "Direccion", "Dormitorios", "Imagen", "MetrosCuadrados", "Precio", "Tipo", "Titulo" },
                values: new object[,]
                {
                    { 1, true, 2, "Lima", "DEP001", "Av. Central 123", 2, "dep1.jpg", 80.0, 120000.0, 0, "Departamento céntrico" },
                    { 2, true, 3, "Arequipa", "CASA002", "Calle Sur 45", 3, "casa2.jpg", 150.0, 250000.0, 1, "Casa familiar" },
                    { 3, true, 1, "Cusco", "OFI003", "Jr. Comercio 10", 0, "ofi3.jpg", 60.0, 90000.0, 2, "Oficina moderna" },
                    { 4, true, 2, "Trujillo", "LOC004", "Av. Norte 77", 0, "loc4.jpg", 100.0, 180000.0, 3, "Local comercial" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inmuebles_Codigo",
                table: "Inmuebles",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_InmuebleId",
                table: "Reservas",
                column: "InmuebleId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_InmuebleId",
                table: "Visitas",
                column: "InmuebleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Visitas");

            migrationBuilder.DropTable(
                name: "Inmuebles");
        }
    }
}
