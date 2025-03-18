using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmoPilates.Migrations
{
    /// <inheritdoc />
    public partial class ModifEliminacionCascada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnosAlumnos_Alumnos_AlumnoId",
                table: "TurnosAlumnos");

            migrationBuilder.DropForeignKey(
                name: "FK_TurnosAlumnos_Turnos_TurnoId",
                table: "TurnosAlumnos");

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosAlumnos_Alumnos_AlumnoId",
                table: "TurnosAlumnos",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosAlumnos_Turnos_TurnoId",
                table: "TurnosAlumnos",
                column: "TurnoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnosAlumnos_Alumnos_AlumnoId",
                table: "TurnosAlumnos");

            migrationBuilder.DropForeignKey(
                name: "FK_TurnosAlumnos_Turnos_TurnoId",
                table: "TurnosAlumnos");

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosAlumnos_Alumnos_AlumnoId",
                table: "TurnosAlumnos",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosAlumnos_Turnos_TurnoId",
                table: "TurnosAlumnos",
                column: "TurnoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
