using AmoPilates.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Entidades
{
    public class Turno
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DiaValido]
        public required string Dia { get; set; }
        [Required(ErrorMessage = "El campo {1} es requerido")]
        [HoraValido]
        public required string Hora { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public List<TurnoAlumno> TurnoAlumnos { get; set; } = [];
        [Range(0, 30)]
        public int Cupos { get; set; }
    }

}
