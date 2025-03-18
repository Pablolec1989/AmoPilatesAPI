namespace AmoPilates.DTOs
{
    public class TurnosPostGetDTO
    {
        public List<AlumnoDTO> Alumnos { get; set; } = [];
        public List<InstructorDTO> Instructores { get; set; } = [];
    }
}
