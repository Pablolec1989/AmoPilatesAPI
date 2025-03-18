using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Entidades
{
    public class Turno
    {
        public int Id { get; set; }
        public int DiaHoraId { get; set; }
        public required DiaHoraDTO DiaHora { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public List<TurnoAlumno> TurnosAlumnos { get; set; } = [];
    }

}
