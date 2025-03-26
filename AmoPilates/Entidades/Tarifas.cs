using System.ComponentModel.DataAnnotations.Schema;

namespace AmoPilates.Entidades
{
    public class Tarifas
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorClaseIndividual { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Valor2Turnos { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Valor3Turnos { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GananciaTotal { get; set; }
        public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;
    }
}
