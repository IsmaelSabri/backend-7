using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Homes.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Homes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ViviendaId = table.Column<string>(type: "text", nullable: true),
                    Lat = table.Column<double>(type: "double precision", nullable: true),
                    Lng = table.Column<double>(type: "double precision", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Cp = table.Column<int>(type: "integer", nullable: true),
                    Habitaciones = table.Column<string>(type: "text", nullable: true),
                    Aseos = table.Column<string>(type: "text", nullable: true),
                    Superficie = table.Column<string>(type: "text", nullable: true),
                    Condicion = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Ascensor = table.Column<bool>(type: "boolean", nullable: true),
                    PrecioInicial = table.Column<string>(type: "text", nullable: true),
                    Descuento = table.Column<string>(type: "text", nullable: true),
                    PrecioFinal = table.Column<string>(type: "text", nullable: true),
                    Duracion = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    AireAcondicionado = table.Column<bool>(type: "boolean", nullable: true),
                    Calefaccion = table.Column<bool>(type: "boolean", nullable: true),
                    PanelesSolares = table.Column<string>(type: "text", nullable: true),
                    ArmariosEmpotrados = table.Column<string>(type: "text", nullable: true),
                    Terraza = table.Column<string>(type: "text", nullable: true),
                    Parquet = table.Column<bool>(type: "boolean", nullable: true),
                    Balcon = table.Column<string>(type: "text", nullable: true),
                    Antiguedad = table.Column<string>(type: "text", nullable: true),
                    Garage = table.Column<string>(type: "text", nullable: true),
                    MetroMasProximo = table.Column<string>(type: "text", nullable: true),
                    AutobusMasProximo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Jardin = table.Column<string>(type: "text", nullable: true),
                    Piscina = table.Column<string>(type: "text", nullable: true),
                    Trastero = table.Column<bool>(type: "boolean", nullable: true),
                    VistasDespejadas = table.Column<string>(type: "text", nullable: true),
                    DistanciaAlMar = table.Column<string>(type: "text", nullable: true),
                    BajoOplantabaja = table.Column<string>(type: "text", nullable: true),
                    InstalacionesDiscapacitados = table.Column<bool>(type: "boolean", nullable: true),
                    NuevoProyecto = table.Column<string>(type: "text", nullable: true),
                    Creador = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroVisitas = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ImageName = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true),
                    Mascotas = table.Column<bool>(type: "boolean", nullable: true),
                    Fianza = table.Column<int>(type: "integer", nullable: true),
                    Disponibilidad = table.Column<string>(type: "text", nullable: true),
                    EstanciaMinima = table.Column<int>(type: "integer", nullable: true),
                    SepuedeFumar = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenParejas = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenMenoresdeedad = table.Column<bool>(type: "boolean", nullable: true),
                    PerfilCompartir = table.Column<string>(type: "text", nullable: true),
                    HabitantesActualmente = table.Column<int>(type: "integer", nullable: true),
                    PropietarioviveEnlacasa = table.Column<bool>(type: "boolean", nullable: true),
                    Ambiente = table.Column<string>(type: "text", nullable: true),
                    Internet = table.Column<bool>(type: "boolean", nullable: true),
                    Gastos = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<int>(type: "integer", nullable: true),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Calle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Homes");
        }
    }
}
