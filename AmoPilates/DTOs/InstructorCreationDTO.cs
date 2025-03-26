using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmoPilates.DTOs
{
    public class InstructorCreationDTO
    {
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public int PorcentajeDeGanancia { get; set; }
    }
}
