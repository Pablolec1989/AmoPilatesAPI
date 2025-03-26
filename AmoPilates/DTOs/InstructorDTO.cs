namespace AmoPilates.DTOs
{
    public class InstructorDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public int PorcentajeDeGanancia { get; set; }
        public decimal Ganancia { get; set; }
    }
}
