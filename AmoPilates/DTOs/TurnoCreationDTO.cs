using AmoPilates.Entidades;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class TurnoCreationDTO
    {
        public int Id { get; set; }
        public required int DiaHoraId { get; set; }
        public required int InstructorId { get; set; }
        public List<int> AlumnosIds { get; set; } = [];
    }
}
