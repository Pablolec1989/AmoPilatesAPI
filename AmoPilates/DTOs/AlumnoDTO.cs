namespace AmoPilates.DTOs
{
    public class AlumnoDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string NroTelefono { get; set; }
        public required string Observaciones { get; set; }
        public int FrecuenciaTurno { get; set; }
    }
}
