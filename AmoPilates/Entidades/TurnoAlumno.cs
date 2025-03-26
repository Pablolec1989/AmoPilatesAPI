using AmoPilates.DTOs;

namespace AmoPilates.Entidades
{
    public class TurnoAlumno
    {
        public int AlumnoId { get; set; }
        public Alumno? Alumnos { get; set; }
        public int TurnoId { get; set; }
        public Turno? Turnos { get; set; }

    }
}
