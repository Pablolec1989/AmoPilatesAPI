namespace AmoPilates.DTOs
{
    public class TurnoPostGetDTO
    {
        public List<AlumnoDTO> alumnos { get; set; } = [];
        public List<InstructorDTO> Instructores { get; set; } = null!;
    }
}
