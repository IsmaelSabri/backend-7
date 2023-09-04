using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Homes.Migrations
{
    /// <inheritdoc />
    public partial class firstmigration : Migration
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
                    Calle = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<int>(type: "integer", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Cp = table.Column<int>(type: "integer", nullable: true),
                    Habitaciones = table.Column<string>(type: "text", nullable: true),
                    Aseos = table.Column<string>(type: "text", nullable: true),
                    AseoEnsuite = table.Column<bool>(type: "boolean", nullable: true),
                    Superficie = table.Column<string>(type: "text", nullable: true),
                    Condicion = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    PrecioInicial = table.Column<string>(type: "text", nullable: true),
                    Descuento = table.Column<string>(type: "text", nullable: true),
                    PrecioFinal = table.Column<string>(type: "text", nullable: true),
                    PrecioM2 = table.Column<string>(type: "text", nullable: true),
                    Duracion = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    ArmariosEmpotrados = table.Column<string>(type: "text", nullable: true),
                    Terraza = table.Column<string>(type: "text", nullable: true),
                    PiscinaPrivada = table.Column<bool>(type: "boolean", nullable: true),
                    Parquet = table.Column<bool>(type: "boolean", nullable: true),
                    Antiguedad = table.Column<string>(type: "text", nullable: true),
                    Garage = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    DistanciaAlMar = table.Column<string>(type: "text", nullable: true),
                    Creador = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NumeroVisitas = table.Column<string>(type: "text", nullable: true),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Destacar = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ImageName = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayRent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    NumeroRegistro = table.Column<string>(type: "text", nullable: true),
                    Personas = table.Column<string>(type: "text", nullable: true),
                    Camas = table.Column<string>(type: "text", nullable: true),
                    Toallas = table.Column<bool>(type: "boolean", nullable: true),
                    Sabanas = table.Column<bool>(type: "boolean", nullable: true),
                    Tv = table.Column<bool>(type: "boolean", nullable: true),
                    TvCable = table.Column<bool>(type: "boolean", nullable: true),
                    Microondas = table.Column<bool>(type: "boolean", nullable: true),
                    Lavavajillas = table.Column<bool>(type: "boolean", nullable: true),
                    Lavadora = table.Column<bool>(type: "boolean", nullable: true),
                    Secadora = table.Column<bool>(type: "boolean", nullable: true),
                    Cafetera = table.Column<bool>(type: "boolean", nullable: true),
                    Plancha = table.Column<bool>(type: "boolean", nullable: true),
                    Cuna = table.Column<bool>(type: "boolean", nullable: true),
                    SecadorDePelo = table.Column<bool>(type: "boolean", nullable: true),
                    Wifi = table.Column<bool>(type: "boolean", nullable: true),
                    PrimeraLineaPlaya = table.Column<bool>(type: "boolean", nullable: true),
                    Chimenea = table.Column<bool>(type: "boolean", nullable: true),
                    MueblesJardin = table.Column<bool>(type: "boolean", nullable: true),
                    Barbacoa = table.Column<bool>(type: "boolean", nullable: true),
                    ValoracionesUsuarios = table.Column<string>(type: "text", nullable: true),
                    Valoraciones = table.Column<int>(type: "integer", nullable: true),
                    StarRatingAverage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayRent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HolidayRent_Homes_Id",
                        column: x => x.Id,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "House",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Alarma = table.Column<bool>(type: "boolean", nullable: true),
                    Recepcion24_7 = table.Column<bool>(type: "boolean", nullable: true),
                    VideoVigilancia = table.Column<bool>(type: "boolean", nullable: true),
                    AlarmaIncendios = table.Column<bool>(type: "boolean", nullable: true),
                    Extintores = table.Column<bool>(type: "boolean", nullable: true),
                    AireAcondicionado = table.Column<bool>(type: "boolean", nullable: true),
                    Calefaccion = table.Column<bool>(type: "boolean", nullable: true),
                    PanelesSolares = table.Column<bool>(type: "boolean", nullable: true),
                    AltaEficienciaEnergetica = table.Column<bool>(type: "boolean", nullable: true),
                    NombreColegios = table.Column<string>(type: "text", nullable: true),
                    EnseñanzaColegios = table.Column<string>(type: "text", nullable: true),
                    InstitucionColegios = table.Column<string>(type: "text", nullable: true),
                    DistanciaColegios = table.Column<string>(type: "text", nullable: true),
                    LineaMetro = table.Column<string>(type: "text", nullable: true),
                    ParadaMetro = table.Column<string>(type: "text", nullable: true),
                    DistanciaMetro = table.Column<string>(type: "text", nullable: true),
                    LineaBus = table.Column<string>(type: "text", nullable: true),
                    ParadaBus = table.Column<string>(type: "text", nullable: true),
                    DistanciaBus = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_House", x => x.Id);
                    table.ForeignKey(
                        name: "FK_House_Homes_Id",
                        column: x => x.Id,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SepuedeFumar = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenParejas = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenMenoresdeedad = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenMochileros = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmitenJubilados = table.Column<bool>(type: "boolean", nullable: true),
                    SeadmiteLGTBI = table.Column<bool>(type: "boolean", nullable: true),
                    PropietarioviveEnlacasa = table.Column<bool>(type: "boolean", nullable: true),
                    PerfilCompartir = table.Column<string>(type: "text", nullable: true),
                    HabitantesActualmente = table.Column<string>(type: "text", nullable: true),
                    Ambiente = table.Column<string>(type: "text", nullable: true),
                    Gastos = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Homes_Id",
                        column: x => x.Id,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Ascensor = table.Column<bool>(type: "boolean", nullable: true),
                    Trastero = table.Column<bool>(type: "boolean", nullable: true),
                    InstalacionesDiscapacitados = table.Column<bool>(type: "boolean", nullable: true),
                    Jardin = table.Column<bool>(type: "boolean", nullable: true),
                    PiscinaComp = table.Column<bool>(type: "boolean", nullable: true),
                    Columpios = table.Column<bool>(type: "boolean", nullable: true),
                    Gym = table.Column<bool>(type: "boolean", nullable: true),
                    Tenis = table.Column<bool>(type: "boolean", nullable: true),
                    Padel = table.Column<bool>(type: "boolean", nullable: true),
                    Sauna = table.Column<bool>(type: "boolean", nullable: true),
                    Jacuzzi = table.Column<bool>(type: "boolean", nullable: true),
                    Golf = table.Column<bool>(type: "boolean", nullable: true),
                    VistasDespejadas = table.Column<string>(type: "text", nullable: true),
                    BajoOplantabaja = table.Column<string>(type: "text", nullable: true),
                    Puerta = table.Column<string>(type: "text", nullable: true),
                    Piso = table.Column<string>(type: "text", nullable: true),
                    Balcon = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flat_House_Id",
                        column: x => x.Id,
                        principalTable: "House",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Planificacion = table.Column<string>(type: "text", nullable: true),
                    InicioDeVentas = table.Column<string>(type: "text", nullable: true),
                    InicioConstruccion = table.Column<string>(type: "text", nullable: true),
                    Mudandose = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewProject_House_Id",
                        column: x => x.Id,
                        principalTable: "House",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Home4rent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Mascotas = table.Column<bool>(type: "boolean", nullable: true),
                    Fianza = table.Column<int>(type: "integer", nullable: true),
                    Disponibilidad = table.Column<string>(type: "text", nullable: true),
                    EstanciaMinima = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home4rent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Home4rent_Flat_Id",
                        column: x => x.Id,
                        principalTable: "Flat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidayRent");

            migrationBuilder.DropTable(
                name: "Home4rent");

            migrationBuilder.DropTable(
                name: "NewProject");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Flat");

            migrationBuilder.DropTable(
                name: "House");

            migrationBuilder.DropTable(
                name: "Homes");
        }
    }
}
