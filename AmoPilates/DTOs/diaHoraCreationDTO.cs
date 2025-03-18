using AmoPilates.Validaciones;

namespace AmoPilates.DTOs
{
    public class diaHoraCreationDTO
    {
        [DiaValido]
        public required string Dia { get; set; }
        [HoraValido]
        public required string Hora { get; set; }
    }
}
