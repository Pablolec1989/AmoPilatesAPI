using AmoPilates.Validaciones;
using System;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Entidades
{
    public class Alumno
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

        [Required(ErrorMessage = "El campo {2} es requerido")]
        [MaxLength(20)]
        [Phone]
        public required string NroTelefono { get; set; }
        [MaxLength(500)]
        public string? Observaciones { get; set; }

        public List<TurnoAlumno> TurnosAlumno { get; set; } = [];
    }
}
