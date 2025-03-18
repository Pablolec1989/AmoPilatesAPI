using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AmoPilates.Utilidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/turnos")]
    public class TurnosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "turnos";

        public TurnosController(ApplicationDbContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
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
            var instructorExiste = await context.Instructores
                .AnyAsync(i => i.Id == turnoCreationDTO.InstructorId);

            if (!instructorExiste)
            {
                return BadRequest("El instructor no existe");
            }

            //Validar que los alumnos existan
            var alumnosIds = turnoCreationDTO.AlumnosIds;

            var alumnosExistentes = await context.Alumnos
                .Where(a => alumnosIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            if (alumnosExistentes.Count != alumnosIds.Count)
            {
                var alumnosNoExistentes = alumnosIds.Except(alumnosExistentes).ToList();
                return BadRequest($"Los siguientes alumnos no existen: {string.Join(", ", alumnosNoExistentes)}");
            }

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
                .Include(t => t.Instructor) // Agregar esta línea
                .FirstOrDefaultAsync(t => t.Id == turno.Id);

            var turnoDTO = mapper.Map<TurnoDTO>(turno);
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return CreatedAtRoute("ObtenerTurnoPorId", new { id = turnoDTO.Id }, turnoDTO);

        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] TurnoCreationDTO turnoCreationDTO)
        {
            //Validar que el id es el mismo del turno a modificar
            if (id != turnoCreationDTO.Id)
            {
                return BadRequest("El id del turno no coincide con el id de la URL");
            }

            var turno = await context.Turnos.FirstOrDefaultAsync(t => t.Id == id);

            //Validar que no haya otro turno el mismo dia y misma hora
            var turnoExiste = await context.Turnos
                .AnyAsync(t => t.DiaHoraId == turnoCreationDTO.DiaHoraId && t.Id != id);

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
            var instructorExiste = await context.Instructores
                .AnyAsync(i => i.Id == turnoCreationDTO.InstructorId);
            if (!instructorExiste)
            {
                return BadRequest("El instructor no existe");
            }

            //Validar que los alumnos existan
            var alumnosIds = turnoCreationDTO.AlumnosIds;
            var alumnosExistentes = await context.Alumnos
                .Where(a => alumnosIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            if (alumnosExistentes.Count != alumnosIds.Count)
            {
                var alumnosNoExistentes = alumnosIds.Except(alumnosExistentes).ToList();
                return BadRequest($"Los siguientes alumnos no existen: {string.Join(", ", alumnosNoExistentes)}");
            }

            //Actualizar el turno
            turno = mapper.Map(turnoCreationDTO, turno);
            await context.SaveChangesAsync();
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

            context.Remove(turno);
            await context.SaveChangesAsync();
            return NoContent();

        }

    }
}
