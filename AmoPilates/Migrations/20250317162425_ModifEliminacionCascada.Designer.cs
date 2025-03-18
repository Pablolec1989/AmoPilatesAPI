﻿// <auto-generated />
using AmoPilates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AmoPilates.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250317162425_ModifEliminacionCascada")]
    partial class ModifEliminacionCascada
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AmoPilates.Entidades.Alumno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NroTelefono")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Observaciones")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Alumnos");
                });

            modelBuilder.Entity("AmoPilates.Entidades.DiaHoraDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cupos")
                        .HasMaxLength(6)
                        .HasColumnType("int");

                    b.Property<string>("Dia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hora")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DiaHoras");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Instructor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Instructores");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Turno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DiaHoraId")
                        .HasColumnType("int");

                    b.Property<int>("InstructorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DiaHoraId");

                    b.HasIndex("InstructorId");

                    b.ToTable("Turnos");
                });

            modelBuilder.Entity("AmoPilates.Entidades.TurnoAlumno", b =>
                {
                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<int>("TurnoId")
                        .HasColumnType("int");

                    b.HasKey("AlumnoId", "TurnoId");

                    b.HasIndex("TurnoId");

                    b.ToTable("TurnosAlumnos");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Turno", b =>
                {
                    b.HasOne("AmoPilates.Entidades.DiaHoraDTO", "DiaHora")
                        .WithMany()
                        .HasForeignKey("DiaHoraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AmoPilates.Entidades.Instructor", "Instructor")
                        .WithMany("Turnos")
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiaHora");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("AmoPilates.Entidades.TurnoAlumno", b =>
                {
                    b.HasOne("AmoPilates.Entidades.Alumno", "Alumnos")
                        .WithMany("TurnosAlumno")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AmoPilates.Entidades.Turno", "Turnos")
                        .WithMany("TurnosAlumnos")
                        .HasForeignKey("TurnoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alumnos");

                    b.Navigation("Turnos");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Alumno", b =>
                {
                    b.Navigation("TurnosAlumno");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Instructor", b =>
                {
                    b.Navigation("Turnos");
                });

            modelBuilder.Entity("AmoPilates.Entidades.Turno", b =>
                {
                    b.Navigation("TurnosAlumnos");
                });
#pragma warning restore 612, 618
        }
    }
}
