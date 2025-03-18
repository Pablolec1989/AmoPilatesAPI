using AmoPilates.Entidades;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class TurnoDTO
    {
        public int Id { get; set; }
        public required DiaHoraDTO DiaHora { get; set; }
        public InstructorTurnoGetDTO Instructor { get; set; } = null!;
        public List<AlumnoTurnoGetDTO> Alumnos { get; set; } = [];

    }
}
