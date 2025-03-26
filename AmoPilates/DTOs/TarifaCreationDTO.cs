using AmoPilates.Entidades;

namespace AmoPilates.DTOs
{
    public class TarifaCreationDTO
    {
        public int Id { get; set; }
        public decimal Valor2Turnos { get; set; }
        public decimal Valor3Turnos { get; set; }
        public decimal ValorClaseIndividual { get; set; }
    }
}
