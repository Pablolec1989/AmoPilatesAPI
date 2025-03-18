using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Entidades
{
    public class DiaHoraDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DiaValido]
        public required string Dia { get; set; }
        [Required(ErrorMessage = "El campo {1} es requerido")]
        [HoraValido]
        public required string Hora { get; set; }
        [MaxLength(6)]
        public int Cupos { get; set; } = 6;
    }
}
