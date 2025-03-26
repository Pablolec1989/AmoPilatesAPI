using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AutoMapper;

namespace AmoPilates.Utilidades
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            //Alumnos
            CreateMap<Alumno, AlumnoDTO>();

            CreateMap<Alumno, AlumnoCreationDTO>();

            CreateMap<Alumno, AlumnoTurnoGetDTO>()
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(src => $"{src.Nombre} {src.Apellido}"));

            CreateMap<AlumnoCreationDTO, Alumno>();

            //Instructores
            CreateMap<Instructor, InstructorDTO>();

            CreateMap<InstructorCreationDTO, Instructor>();

            CreateMap<Instructor, InstructorTurnoGetDTO>()
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(src => $"{src.Nombre} {src.Apellido}"));

            //Turnos
            CreateMap<Turno, TurnoDTO>()
                .ForMember(dto => dto.Alumnos, config => config.MapFrom(src => src.TurnoAlumnos.Select(ta => ta.Alumnos)));

            CreateMap<TurnoCreationDTO, Turno>()
                .ForMember(ent => ent.TurnoAlumnos, config => config.MapFrom(dto => dto.AlumnosIds
                .Select(id => new TurnoAlumno { AlumnoId = id })));

            CreateMap<TarifaCreationDTO, Tarifas>();

            CreateMap<Tarifas, TarifaDTO>();


        }
    }
}
