using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AmoPilates.Utilidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using AmoPilates.Servicios;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/turnos")]
    public class TurnosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly InstructoresService instructorService;
        private readonly AlumnosService alumnosService;
        private const string cacheTag = "turnos";

        public TurnosController(ApplicationDbContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore, InstructoresService instructorService,
            AlumnosService alumnosService)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.instructorService = instructorService;
            this.alumnosService = alumnosService;
        }


        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<List<TurnoDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Turnos;
            await HttpContext.insertarParametrosPaginacionEnCabecera(queryable);

            return await queryable
                .Paginar(paginacion)
                .ProjectTo<TurnoDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

        }


        [HttpGet("{id:int}", Name = "ObtenerTurnoPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<TurnoDTO>> Get(int id)
        {
            var turnoDTO = await context.Turnos
                .Include(t => t.Instructor)
                .Include(t => t.TurnoAlumnos)
                .ProjectTo<TurnoDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (turnoDTO is null)
            {
                return NotFound();
            }

            return turnoDTO;
        }


        [HttpGet("PostGet")]
        public async Task<ActionResult<TurnoPostGetDTO>> PostGet()
        {
            var instructores = await context.Instructores
                .ProjectTo<InstructorDTO>(mapper.ConfigurationProvider).ToListAsync();

            var alumnos = await context.Alumnos
                .ProjectTo<AlumnoDTO>(mapper.ConfigurationProvider).ToListAsync();

            return new TurnoPostGetDTO
            {
                Instructores = instructores,
                alumnos = alumnos
            };
        }

        [HttpPost]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> Post([FromForm] TurnoCreationDTO turnoCreationDTO)
        {
            var turno = mapper.Map<Turno>(turnoCreationDTO);

            //Validar que no exista otro turno el mismo dia, hora y con el mismo instructor
            var turnoExiste = await context.Turnos.AnyAsync(t => t.Dia == turnoCreationDTO.Dia && t.Hora == turnoCreationDTO.Hora
            && t.InstructorId == turnoCreationDTO.InstructorId);

            if (turnoExiste)
            {
                return BadRequest("Ya existe un turno el mismo dia, hora y con el mismo instructor");
            }

            //Validar que el instructor exista
            var instructorExiste = await context.Instructores.AnyAsync(i => i.Id == turnoCreationDTO.InstructorId);

            if (!instructorExiste)
            {
                return BadRequest("El instructor ingresado no existe");
            }

            //Validar que los alumnos existan
            var alumnosExisten = await alumnosService.ValidarAlumnos(turnoCreationDTO.AlumnosIds);

            if (alumnosExisten)
            {
                return BadRequest("Uno o mas alumnos no existen");
            }

            //Validar que no haya alumnos duplicados
            var alumnosDuplicados = await alumnosService.ValidarAlumnosDuplicados(turnoCreationDTO.AlumnosIds, turno.Id);

            if (alumnosDuplicados)
            {
                return BadRequest("Hay un alumno duplicado en el turno");
            }

            //Validar que la cantidad de alumnos no exceda el cupo
            if (turno.Cupos - turnoCreationDTO.AlumnosIds.Count() < 0)
            {
                return BadRequest("No hay suficientes cupos disponibles.");
            }

            turno.Cupos -= turnoCreationDTO.AlumnosIds.Count();

            // Actualizar la frecuencia de los alumnos
            var alumnosIds = turnoCreationDTO.AlumnosIds;
            await context.Alumnos
                .Where(a => alumnosIds.Contains(a.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.FrecuenciaTurno, b => b.FrecuenciaTurno + 1));

            context.Add(turno);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            var turnoDTO = mapper.Map<TurnoDTO>(turno);

            return CreatedAtRoute("ObtenerTurnoPorId", new { id = turno.Id }, turnoDTO);

        }

        [HttpPut("{id:int}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Put(int id, [FromForm] TurnoCreationDTO turnoCreationDTO)
        {
            var turnoId = await context.Turnos.FirstOrDefaultAsync(t => t.Id == id);

            if(turnoId is null)
            {
                return NotFound();
            }

            var turno = mapper.Map<Turno>(turnoCreationDTO);

            //Validar que no exista otro turno el mismo dia, hora y con el mismo instructor
            var turnoExiste = await context.Turnos.AnyAsync(t => t.Dia == turnoCreationDTO.Dia && t.Hora == turnoCreationDTO.Hora
            && t.InstructorId == turnoCreationDTO.InstructorId);

            if (turnoExiste)
            {
                return BadRequest("Ya existe un turno el mismo dia, hora y con el mismo instructor");
            }

            //Validar que el instructor exista
            var instructorExiste = await context.Instructores.AnyAsync(i => i.Id == turnoCreationDTO.InstructorId);

            if (!instructorExiste)
            {
                return BadRequest("El instructor ingresado no existe");
            }

            //Validar que los alumnos existan
            var alumnosExisten = await alumnosService.ValidarAlumnos(turnoCreationDTO.AlumnosIds);

            if (alumnosExisten)
            {
                return BadRequest("Uno o mas alumnos no existen");
            }

            //Validar que no haya alumnos duplicados
            var alumnosDuplicados = await alumnosService.ValidarAlumnosDuplicados(turnoCreationDTO.AlumnosIds, turno.Id);

            if (alumnosDuplicados)
            {
                return BadRequest("Hay un alumno duplicado en el turno");
            }

            //Actualizar el dato de frecuencia de turno para cada alumno


            //Actualizar el valor del cupo
            turno.Cupos -= turnoCreationDTO.AlumnosIds.Count();

            turno = mapper.Map(turnoCreationDTO, turno);

            context.Update(turno);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();



        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Busco si el turno existe con ese id
            var turno = await context.Turnos.FirstOrDefaultAsync(t => t.Id == id);

            if (turno is null)
            {
                return BadRequest("El turno no existe");
            }

            //Elimino las relaciones de TurnoAlumno
            var turnosAlumnos = await context.TurnosAlumnos
                .Where(ta => ta.TurnoId == id)
                .ToListAsync();

            context.TurnosAlumnos.RemoveRange(turnosAlumnos);

            //Elimino el turno
            context.Turnos.Remove(turno);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();

        }
    }
}
