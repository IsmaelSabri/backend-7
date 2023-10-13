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
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lng = table.Column<double>(type: "double precision", nullable: false),
                    Calle = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Cp = table.Column<int>(type: "integer", nullable: false),
                    Habitaciones = table.Column<string>(type: "text", nullable: true),
                    Aseos = table.Column<string>(type: "text", nullable: true),
                    AseoEnsuite = table.Column<bool>(type: "boolean", nullable: false),
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
                    PiscinaPrivada = table.Column<bool>(type: "boolean", nullable: false),
                    Parquet = table.Column<bool>(type: "boolean", nullable: false),
                    Antiguedad = table.Column<string>(type: "text", nullable: true),
                    Garage = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    DistanciaAlMar = table.Column<string>(type: "text", nullable: true),
                    Creador = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroVisitas = table.Column<string>(type: "text", nullable: true),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Destacar = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ImageName = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true),
                    Amueblado = table.Column<bool>(type: "boolean", nullable: false),
                    StreetView = table.Column<string>(type: "text", nullable: true),
                    DireccionAproximada = table.Column<bool>(type: "boolean", nullable: false)
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
                    Toallas = table.Column<bool>(type: "boolean", nullable: false),
                    Sabanas = table.Column<bool>(type: "boolean", nullable: false),
                    Tv = table.Column<bool>(type: "boolean", nullable: false),
                    TvCable = table.Column<bool>(type: "boolean", nullable: false),
                    Microondas = table.Column<bool>(type: "boolean", nullable: false),
                    Lavavajillas = table.Column<bool>(type: "boolean", nullable: false),
                    Lavadora = table.Column<bool>(type: "boolean", nullable: false),
                    Secadora = table.Column<bool>(type: "boolean", nullable: false),
                    Cafetera = table.Column<bool>(type: "boolean", nullable: false),
                    Plancha = table.Column<bool>(type: "boolean", nullable: false),
                    Cuna = table.Column<bool>(type: "boolean", nullable: false),
                    SecadorDePelo = table.Column<bool>(type: "boolean", nullable: false),
                    Wifi = table.Column<bool>(type: "boolean", nullable: false),
                    PrimeraLineaPlaya = table.Column<bool>(type: "boolean", nullable: false),
                    Chimenea = table.Column<bool>(type: "boolean", nullable: false),
                    MueblesJardin = table.Column<bool>(type: "boolean", nullable: false),
                    Barbacoa = table.Column<bool>(type: "boolean", nullable: false),
                    ValoracionesUsuarios = table.Column<string>(type: "text", nullable: true),
                    Valoraciones = table.Column<int>(type: "integer", nullable: false),
                    StarRatingAverage = table.Column<int>(type: "integer", nullable: false)
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
                    Alarma = table.Column<bool>(type: "boolean", nullable: false),
                    Recepcion24_7 = table.Column<bool>(type: "boolean", nullable: false),
                    VideoVigilancia = table.Column<bool>(type: "boolean", nullable: false),
                    AlarmaIncendios = table.Column<bool>(type: "boolean", nullable: false),
                    Extintores = table.Column<bool>(type: "boolean", nullable: false),
                    AireAcondicionado = table.Column<bool>(type: "boolean", nullable: false),
                    Calefaccion = table.Column<bool>(type: "boolean", nullable: false),
                    PanelesSolares = table.Column<bool>(type: "boolean", nullable: false),
                    EficienciaEnergetica = table.Column<bool>(type: "boolean", nullable: false),
                    Colegios = table.Column<string>(type: "text", nullable: true),
                    Universidades = table.Column<string>(type: "text", nullable: true),
                    Supermercados = table.Column<string>(type: "text", nullable: true),
                    Metro = table.Column<string>(type: "text", nullable: true),
                    Bus = table.Column<string>(type: "text", nullable: true),
                    Consumo = table.Column<string>(type: "text", nullable: true),
                    Emisiones = table.Column<string>(type: "text", nullable: true)
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
                    SepuedeFumar = table.Column<bool>(type: "boolean", nullable: false),
                    SeadmitenParejas = table.Column<bool>(type: "boolean", nullable: false),
                    SeadmitenMenoresdeedad = table.Column<bool>(type: "boolean", nullable: false),
                    SeadmitenMochileros = table.Column<bool>(type: "boolean", nullable: false),
                    SeadmitenJubilados = table.Column<bool>(type: "boolean", nullable: false),
                    SeadmiteLGTBI = table.Column<bool>(type: "boolean", nullable: false),
                    PropietarioviveEnlacasa = table.Column<bool>(type: "boolean", nullable: false),
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
                    Ascensor = table.Column<bool>(type: "boolean", nullable: false),
                    Trastero = table.Column<bool>(type: "boolean", nullable: false),
                    InstalacionesDiscapacitados = table.Column<bool>(type: "boolean", nullable: false),
                    Jardin = table.Column<bool>(type: "boolean", nullable: false),
                    PiscinaComp = table.Column<bool>(type: "boolean", nullable: false),
                    Columpios = table.Column<bool>(type: "boolean", nullable: false),
                    Gym = table.Column<bool>(type: "boolean", nullable: false),
                    Tenis = table.Column<bool>(type: "boolean", nullable: false),
                    Padel = table.Column<bool>(type: "boolean", nullable: false),
                    Sauna = table.Column<bool>(type: "boolean", nullable: false),
                    Jacuzzi = table.Column<bool>(type: "boolean", nullable: false),
                    Golf = table.Column<bool>(type: "boolean", nullable: false),
                    VistasDespejadas = table.Column<string>(type: "text", nullable: true),
                    BajoOplantabaja = table.Column<string>(type: "text", nullable: true),
                    Puerta = table.Column<string>(type: "text", nullable: true),
                    Piso = table.Column<string>(type: "text", nullable: true),
                    Balcon = table.Column<string>(type: "text", nullable: true),
                    VideoPortero = table.Column<bool>(type: "boolean", nullable: false)
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
                    Mascotas = table.Column<bool>(type: "boolean", nullable: false),
                    Fianza = table.Column<int>(type: "integer", nullable: false),
                    Disponibilidad = table.Column<string>(type: "text", nullable: true),
                    EstanciaMinima = table.Column<int>(type: "integer", nullable: false)
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
