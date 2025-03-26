using AmoPilates.Entidades;
using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class TurnoCreationDTO
    {
        public int Id { get; set; }
        public required string Dia { get; set; }
        public required string Hora { get; set; }
        public int InstructorId { get; set; }
        public List<int> AlumnosIds { get; set; } = [];
        public int Cupos { get; set; }
    }
}
