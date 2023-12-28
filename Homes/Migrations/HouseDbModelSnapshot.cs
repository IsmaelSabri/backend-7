﻿// <auto-generated />
using System;
using Homes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Homes.Migrations
{
    [DbContext(typeof(HouseDb))]
    partial class HouseDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Homes.Models.Home", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AireAcondicionado")
                        .HasColumnType("boolean");

                    b.Property<bool>("Amueblado")
                        .HasColumnType("boolean");

                    b.Property<int>("Antiguedad")
                        .HasColumnType("integer");

                    b.Property<bool>("ArmariosEmpotrados")
                        .HasColumnType("boolean");

                    b.Property<int>("AseoEnsuite")
                        .HasColumnType("integer");

                    b.Property<int>("Aseos")
                        .HasColumnType("integer");

                    b.Property<bool>("Balcon")
                        .HasColumnType("boolean");

                    b.Property<string>("Bus")
                        .HasColumnType("text");

                    b.Property<bool>("Calefaccion")
                        .HasColumnType("boolean");

                    b.Property<string>("Calle")
                        .HasColumnType("text");

                    b.Property<string>("Ciudad")
                        .HasColumnType("text");

                    b.Property<string>("Comentario")
                        .HasColumnType("text");

                    b.Property<string>("Condicion")
                        .HasColumnType("text");

                    b.Property<int>("Cp")
                        .HasColumnType("integer");

                    b.Property<string>("Creador")
                        .HasColumnType("text");

                    b.Property<string>("Descripcion")
                        .HasColumnType("text");

                    b.Property<string>("Destacar")
                        .HasColumnType("text");

                    b.Property<bool>("DireccionAproximada")
                        .HasColumnType("boolean");

                    b.Property<string>("DistanciaAlMar")
                        .HasColumnType("text");

                    b.Property<string>("Distrito")
                        .HasColumnType("text");

                    b.Property<string>("Duracion")
                        .HasColumnType("text");

                    b.Property<string>("Estado")
                        .HasColumnType("text");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("FechaUltimaModificacion")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Garage")
                        .HasColumnType("integer");

                    b.Property<bool>("GasNatural")
                        .HasColumnType("boolean");

                    b.Property<int>("Habitaciones")
                        .HasColumnType("integer");

                    b.Property<string>("ImagesAsString")
                        .HasColumnType("text");

                    b.Property<double>("Lat")
                        .HasColumnType("double precision");

                    b.Property<double>("Lng")
                        .HasColumnType("double precision");

                    b.Property<string>("Metro")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("NombreCreador")
                        .HasColumnType("text");

                    b.Property<int>("Numero")
                        .HasColumnType("integer");

                    b.Property<string>("NumeroVisitas")
                        .HasColumnType("text");

                    b.Property<string>("Orientacion")
                        .HasColumnType("text");

                    b.Property<bool>("Parquet")
                        .HasColumnType("boolean");

                    b.Property<bool>("PiscinaPrivada")
                        .HasColumnType("boolean");

                    b.Property<string>("PlantaMasAlta")
                        .HasColumnType("text");

                    b.Property<int>("PrecioAlquiler")
                        .HasColumnType("integer");

                    b.Property<int>("PrecioFinal")
                        .HasColumnType("integer");

                    b.Property<int>("PrecioInicial")
                        .HasColumnType("integer");

                    b.Property<string>("StreetView")
                        .HasColumnType("text");

                    b.Property<int>("Superficie")
                        .HasColumnType("integer");

                    b.Property<string>("Terraza")
                        .HasColumnType("text");

                    b.Property<string>("Tipo")
                        .HasColumnType("text");

                    b.Property<string>("TipoDeVia")
                        .HasColumnType("text");

                    b.Property<string>("Universidades")
                        .HasColumnType("text");

                    b.Property<bool>("Vendido")
                        .HasColumnType("boolean");

                    b.Property<string>("Video")
                        .HasColumnType("text");

                    b.Property<string>("VideoPortero")
                        .HasColumnType("text");

                    b.Property<string>("ViviendaId")
                        .HasColumnType("text");

                    b.Property<string>("ZonaDeOcio")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Homes");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Homes.Models.HolidayRent", b =>
                {
                    b.HasBaseType("Homes.Models.Home");

                    b.Property<bool>("Barbacoa")
                        .HasColumnType("boolean");

                    b.Property<bool>("Cafetera")
                        .HasColumnType("boolean");

                    b.Property<string>("Camas")
                        .HasColumnType("text");

                    b.Property<bool>("Chimenea")
                        .HasColumnType("boolean");

                    b.Property<bool>("Cuna")
                        .HasColumnType("boolean");

                    b.Property<bool>("Lavadora")
                        .HasColumnType("boolean");

                    b.Property<bool>("Lavavajillas")
                        .HasColumnType("boolean");

                    b.Property<bool>("Microondas")
                        .HasColumnType("boolean");

                    b.Property<bool>("MueblesJardin")
                        .HasColumnType("boolean");

                    b.Property<string>("NumeroRegistro")
                        .HasColumnType("text");

                    b.Property<string>("Personas")
                        .HasColumnType("text");

                    b.Property<bool>("Plancha")
                        .HasColumnType("boolean");

                    b.Property<bool>("PrimeraLineaPlaya")
                        .HasColumnType("boolean");

                    b.Property<bool>("Sabanas")
                        .HasColumnType("boolean");

                    b.Property<bool>("SecadorDePelo")
                        .HasColumnType("boolean");

                    b.Property<bool>("Secadora")
                        .HasColumnType("boolean");

                    b.Property<int>("StarRatingAverage")
                        .HasColumnType("integer");

                    b.Property<bool>("Toallas")
                        .HasColumnType("boolean");

                    b.Property<bool>("Tv")
                        .HasColumnType("boolean");

                    b.Property<bool>("TvCable")
                        .HasColumnType("boolean");

                    b.Property<int>("Valoraciones")
                        .HasColumnType("integer");

                    b.Property<string>("ValoracionesUsuarios")
                        .HasColumnType("text");

                    b.Property<bool>("Wifi")
                        .HasColumnType("boolean");

                    b.ToTable("HolidayRent", (string)null);
                });

            modelBuilder.Entity("Homes.Models.House", b =>
                {
                    b.HasBaseType("Homes.Models.Home");

                    b.Property<string>("Aeropuerto")
                        .HasColumnType("text");

                    b.Property<bool>("Alarma")
                        .HasColumnType("boolean");

                    b.Property<bool>("AlarmaIncendios")
                        .HasColumnType("boolean");

                    b.Property<string>("Colegios")
                        .HasColumnType("text");

                    b.Property<string>("Consumo")
                        .HasColumnType("text");

                    b.Property<bool>("EficienciaEnergetica")
                        .HasColumnType("boolean");

                    b.Property<string>("Emisiones")
                        .HasColumnType("text");

                    b.Property<bool>("Extintores")
                        .HasColumnType("boolean");

                    b.Property<bool>("GeneradorEmergencia")
                        .HasColumnType("boolean");

                    b.Property<bool>("InstalacionesDiscapacitados")
                        .HasColumnType("boolean");

                    b.Property<bool>("Jacuzzi")
                        .HasColumnType("boolean");

                    b.Property<bool>("PanelesSolares")
                        .HasColumnType("boolean");

                    b.Property<bool>("Recepcion24_7")
                        .HasColumnType("boolean");

                    b.Property<string>("Supermercados")
                        .HasColumnType("text");

                    b.Property<bool>("VideoVigilancia")
                        .HasColumnType("boolean");

                    b.Property<string>("VistasDespejadas")
                        .HasColumnType("text");

                    b.ToTable("House", (string)null);
                });

            modelBuilder.Entity("Homes.Models.Room", b =>
                {
                    b.HasBaseType("Homes.Models.Home");

                    b.Property<string>("Ambiente")
                        .HasColumnType("text");

                    b.Property<string>("Gastos")
                        .HasColumnType("text");

                    b.Property<string>("HabitantesActualmente")
                        .HasColumnType("text");

                    b.Property<string>("PerfilCompartir")
                        .HasColumnType("text");

                    b.Property<bool>("PropietarioviveEnlacasa")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeadmiteLGTBI")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeadmitenJubilados")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeadmitenMenoresdeedad")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeadmitenMochileros")
                        .HasColumnType("boolean");

                    b.Property<bool>("SeadmitenParejas")
                        .HasColumnType("boolean");

                    b.Property<bool>("SepuedeFumar")
                        .HasColumnType("boolean");

                    b.ToTable("Room", (string)null);
                });

            modelBuilder.Entity("Homes.Models.Flat", b =>
                {
                    b.HasBaseType("Homes.Models.House");

                    b.Property<bool>("Ascensor")
                        .HasColumnType("boolean");

                    b.Property<bool>("BajoOplantabaja")
                        .HasColumnType("boolean");

                    b.Property<bool>("Columpios")
                        .HasColumnType("boolean");

                    b.Property<bool>("Golf")
                        .HasColumnType("boolean");

                    b.Property<bool>("Gym")
                        .HasColumnType("boolean");

                    b.Property<bool>("Jardin")
                        .HasColumnType("boolean");

                    b.Property<bool>("Padel")
                        .HasColumnType("boolean");

                    b.Property<bool>("PiscinaComp")
                        .HasColumnType("boolean");

                    b.Property<string>("Piso")
                        .HasColumnType("text");

                    b.Property<bool>("Sauna")
                        .HasColumnType("boolean");

                    b.Property<bool>("Tenis")
                        .HasColumnType("boolean");

                    b.Property<bool>("Trastero")
                        .HasColumnType("boolean");

                    b.ToTable("Flat", (string)null);
                });

            modelBuilder.Entity("Homes.Models.NewProject", b =>
                {
                    b.HasBaseType("Homes.Models.House");

                    b.Property<string>("InicioConstruccion")
                        .HasColumnType("text");

                    b.Property<string>("InicioDeVentas")
                        .HasColumnType("text");

                    b.Property<string>("Mudandose")
                        .HasColumnType("text");

                    b.Property<string>("Planificacion")
                        .HasColumnType("text");

                    b.ToTable("NewProject", (string)null);
                });

            modelBuilder.Entity("Homes.Models.Home4rent", b =>
                {
                    b.HasBaseType("Homes.Models.Flat");

                    b.Property<string>("Disponibilidad")
                        .HasColumnType("text");

                    b.Property<int>("EstanciaMinima")
                        .HasColumnType("integer");

                    b.Property<int>("Fianza")
                        .HasColumnType("integer");

                    b.Property<string>("Mascotas")
                        .HasColumnType("text");

                    b.ToTable("Home4rent", (string)null);
                });

            modelBuilder.Entity("Homes.Models.HolidayRent", b =>
                {
                    b.HasOne("Homes.Models.Home", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.HolidayRent", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Homes.Models.House", b =>
                {
                    b.HasOne("Homes.Models.Home", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.House", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Homes.Models.Room", b =>
                {
                    b.HasOne("Homes.Models.Home", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.Room", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Homes.Models.Flat", b =>
                {
                    b.HasOne("Homes.Models.House", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.Flat", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Homes.Models.NewProject", b =>
                {
                    b.HasOne("Homes.Models.House", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.NewProject", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Homes.Models.Home4rent", b =>
                {
                    b.HasOne("Homes.Models.Flat", null)
                        .WithOne()
                        .HasForeignKey("Homes.Models.Home4rent", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
