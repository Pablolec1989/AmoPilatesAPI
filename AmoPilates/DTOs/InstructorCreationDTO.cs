using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class InstructorCreationDTO
    {
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Apellido { get; set; }
    }
}
