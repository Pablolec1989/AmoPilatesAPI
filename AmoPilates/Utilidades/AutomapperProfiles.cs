using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AutoMapper;

namespace AmoPilates.Utilidades
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            construirMapeoAlumnos();
            construirMapeoInstructores();
            construirMapeoDiaHora();
            construirMapeoTurnos();
        }


        private void construirMapeoAlumnos()
        {
            CreateMap<Alumno, AlumnoTurnoGetDTO>()
                .ForMember(dto => dto.NombreCompleto, config => 
                config.MapFrom(a => $"{a.Nombre}{a.Apellido}"));

            CreateMap<Alumno, AlumnoDTO>().ReverseMap();
            CreateMap<AlumnoDTO, AlumnoTurnoGetDTO>().ReverseMap();
            CreateMap<Alumno, AlumnoCreationDTO>().ReverseMap();
        }

        private void construirMapeoInstructores()
        {
            CreateMap<Instructor, InstructorDTO>();
            CreateMap<InstructorCreationDTO, Instructor>();

            CreateMap<Instructor, InstructorTurnoGetDTO>()
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(i => $"{i.Nombre} {i.Apellido}"));
        }

        private void construirMapeoDiaHora()
        {
            CreateMap<DiaHoraDTO, diaHoraDTO>().ReverseMap();
            CreateMap<DiaHoraDTO, diaHoraCreationDTO>().ReverseMap();
        }

        private void construirMapeoTurnos()
        {
            CreateMap<Turno, TurnoCreationDTO>()
                .ForMember(dto => dto.AlumnosIds, config => config.MapFrom(t => t.TurnosAlumnos.Select(t => t.AlumnoId)))
                .ReverseMap()
                .ForMember(t => t.TurnosAlumnos, config => config.MapFrom(dto => 
                dto.AlumnosIds.Select(id => new TurnoAlumno { AlumnoId = id })));

            CreateMap<Turno, TurnoDTO>()
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ForMember(dto => dto.Alumnos, config => config.MapFrom(t => t.TurnosAlumnos.Select(ta => 
                new AlumnoTurnoGetDTO {
                    NombreCompleto = $"{ta.Alumnos.Nombre} {ta.Alumnos.Apellido}"
                })));




        }
    }
}
