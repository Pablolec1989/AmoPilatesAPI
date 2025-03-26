using AmoPilates.Entidades;
using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class TurnoDTO
    {
        public int Id { get; set; }
        public required string Dia { get; set; }
        public required string Hora { get; set; }
        public InstructorTurnoGetDTO Instructor { get; set; } = null!;
        public List<AlumnoTurnoGetDTO> Alumnos { get; set; } = [];
        public int Cupos { get; set; }

    }
}
