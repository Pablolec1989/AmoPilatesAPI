using AmoPilates.DTOs;

namespace AmoPilates.Entidades
{
    public class TurnoAlumno
    {
        public int AlumnoId { get; set; }
        public Alumno Alumnos { get; set; } = null!;
        public int TurnoId { get; set; }
        public Turno Turnos { get; set; } = null!;

    }
}
