namespace AmoPilates.DTOs
{
    public class TarifaDTO
    {
        public int Id { get; set; }
        public decimal ValorClaseIndividual { get; set; }
        public decimal Valor2Turnos { get; set; }
        public decimal Valor3Turnos { get; set; }
        public decimal GananciaTotal { get; set; }
        public string FechaCalculo { get; set; } = DateTime.UtcNow.ToString("dd/MM/yyyy");
    }
}
