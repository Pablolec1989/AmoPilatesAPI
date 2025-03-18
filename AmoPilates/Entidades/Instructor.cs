using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Entidades
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        [Required (ErrorMessage = "El campo {1} es requerido")]
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Apellido { get; set; }

        public List<Turno> Turnos { get; set; } = [];
    }
}
