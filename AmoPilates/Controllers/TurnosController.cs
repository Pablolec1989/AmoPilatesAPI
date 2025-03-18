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

            var turnos = await queryable
                .ProjectTo<TurnoDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            return turnos;
        }


        [HttpGet("{id:int}", Name = "ObtenerTurnoPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<TurnoDTO>> Get(int id)
        {
            var turno = await context.Turnos
                .ProjectTo<TurnoDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (turno is null)
            {
                return BadRequest();
            }
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return turno;
        }


        [HttpPost]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Post([FromForm] TurnoCreationDTO turnoCreationDTO)
        {

            //Validar que no haya otro turno el mismo dia y misma hora
            var turnoExiste = await context.Turnos
                .AnyAsync(t => t.DiaHoraId == turnoCreationDTO.DiaHoraId);

            if (turnoExiste)
            {
                return BadRequest("Ya existe un turno en ese horario");
            }

            //Validar que dia y horario existan
            var diaHora = await context.DiaHoras
                .FirstOrDefaultAsync(dh => dh.Id == turnoCreationDTO.DiaHoraId);

            if (diaHora is null)
            {
                return BadRequest("El dia o/u horario no existen");
            }

            //Validar que el instructor exista
            if (await instructorService.InstructorExiste(turnoCreationDTO.InstructorId))
            {
                return BadRequest("El instructor no existe");
            }

            //Validar que los alumnos existan
            var alumnosIds = turnoCreationDTO.AlumnosIds;

            await alumnosService.ValidarAlumnos(alumnosIds);

            //Crear el turno
            var turno = mapper.Map<Turno>(turnoCreationDTO);
            context.Add(turno);
            await context.SaveChangesAsync(); //Guardamos para obtener el Id del turno.

            //Crear la relacion TurnosAlumnos
            foreach (var alumnoId in turnoCreationDTO.AlumnosIds)
            {
                // Verificar si el registro ya existe antes de agregarlo
                if (!context.TurnosAlumnos.Any(ta => ta.TurnoId == turno.Id && ta.AlumnoId == alumnoId))
                {
                    context.TurnosAlumnos.Add(new TurnoAlumno { TurnoId = turno.Id, AlumnoId = alumnoId });
                }
            }

            //Cargar los alumnos relacionados y el instructor
            turno = await context.Turnos
                .Include(t => t.TurnosAlumnos)
                .ThenInclude(ta => ta.Alumnos)
                .Include(t => t.Instructor)
                .FirstOrDefaultAsync(t => t.Id == turno.Id);

            //Actualizar los cupos de acuerdo a la cantidad de alumnos que hayan quedado en el turno
            diaHora.Cupos -= turno!.TurnosAlumnos.Count;

            await context.SaveChangesAsync();

            var turnoDTO = mapper.Map<TurnoDTO>(turno);
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return CreatedAtRoute("ObtenerTurnoPorId", new { id = turnoDTO.Id }, turnoDTO);

        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] TurnoCreationDTO turnoCreationDTO)
        {

            //Busco el turno con ese Id.
            var turno = await context.Turnos
                .Include(t => t.TurnosAlumnos)
                .FirstOrDefaultAsync(t => t.Id == id);

            //Validar que el id es el mismo del turno a modificar
            if (id != turnoCreationDTO.Id)
            {
                return BadRequest("El id del turno no coincide con el id de la URL");
            }

            //Validar que no haya otro turno el mismo dia y misma hora
            var turnoExiste = await context.Turnos
                .AnyAsync(t => t.DiaHoraId == turnoCreationDTO.DiaHoraId && t.Id != id);

            if (turnoExiste)
            {
                return BadRequest("Ya existe un turno en ese horario");
            }

            //Buscar el diaHora vinculado al turno y validar que dia y horario existan
            var diaHora = await context.DiaHoras
                .FirstOrDefaultAsync(dh => dh.Id == turnoCreationDTO.DiaHoraId);

            if (diaHora is null)
            {
                return BadRequest("El dia o el horario no existen");
            }

            //Validar que el instructor exista
            if (!await instructorService.InstructorExiste(turnoCreationDTO.InstructorId))
            {
                return BadRequest("El instructor no existe");
            }

            // Validar que los alumnos existan
            var resultadoValidacionAlumnos = await alumnosService.ValidarAlumnos(turnoCreationDTO.AlumnosIds);
            if (resultadoValidacionAlumnos != null)
            {
                return resultadoValidacionAlumnos; // Devuelve el BadRequest si hay errores
            }

            // Eliminar las relaciones antiguas de TurnoAlumno
            var turnosAlumnosAntiguos = await context.TurnosAlumnos
                .Where(ta => ta.TurnoId == id)
                .ToListAsync();

            context.TurnosAlumnos.RemoveRange(turnosAlumnosAntiguos);

            // Crear las nuevas relaciones de TurnoAlumno
            var alumnosIds = turnoCreationDTO.AlumnosIds;
            var nuevosTurnosAlumnos = alumnosIds
                .Select(alumnoId => new TurnoAlumno
                {
                    AlumnoId = alumnoId,
                    TurnoId = id
                })
                .ToList();

            await context.TurnosAlumnos.AddRangeAsync(nuevosTurnosAlumnos);

            // Actualizar los cupos
            diaHora.Cupos -= nuevosTurnosAlumnos.Count - turnosAlumnosAntiguos.Count;

            // Actualizar el turno
            turno!.DiaHoraId = turnoCreationDTO.DiaHoraId;
            turno.InstructorId = turnoCreationDTO.InstructorId;

            //Actualizar el turno
            turno = mapper.Map(turnoCreationDTO, turno);
            context.Update(turno!);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var turno = await context.Turnos.FirstOrDefaultAsync(t => t.Id == id);

            if (turno is null)
            {
                return NotFound();
            }

            //Busco el diaHora vinculado al turno recibido
            var diaHora = await context.DiaHoras
                .FirstOrDefaultAsync(dh => dh.Id == turno.DiaHoraId);

            //Contabilizar todos los alumnos vinculados al turno
            var alumnosEnTurno = await context.TurnosAlumnos
                .Where(ta => ta.TurnoId == id)
                .Select(ta => ta.AlumnoId)
                .ToListAsync();

            //actualizar los cupos 
            diaHora!.Cupos += alumnosEnTurno.Count;

            //Eliminar el turno
            context.Remove(turno);
            await context.SaveChangesAsync();
            return NoContent();
        }



    }
}
