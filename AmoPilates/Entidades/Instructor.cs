using AmoPilates.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmoPilates.Entidades
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {1} es requerido")]
        [MaxLength(50)]
        [PrimeraLetraMayuscula]
        public required string Apellido { get; set; }
        public List<Turno> Turnos { get; set; } = [];
        [Required]
        [Range(0, 100)]
        public int PorcentajeDeGanancia { get; set; }
        [Precision(18, 2)]
        public decimal Ganancia { get; set; }
    }
    
}
