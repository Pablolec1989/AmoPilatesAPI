using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.DTOs
{
    public class AlumnoCreationDTO
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Apellido { get; set; }

        [MaxLength(25)]
        public required string NroTelefono { get; set; }
        public string? Observaciones { get; set; }
    }
}
