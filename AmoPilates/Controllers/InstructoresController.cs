using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AmoPilates.Servicios;
using AmoPilates.Utilidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/instructores")]
    public class InstructoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "instructores";

        public InstructoresController(ApplicationDbContext context, IMapper mapper, 
            IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }


        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<InstructorDTO>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Instructores;
            await HttpContext.insertarParametrosPaginacionEnCabecera(queryable);

            return await queryable
                .OrderBy(i => i.Id)
                .Paginar(paginacion)
                .ProjectTo<InstructorDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerInstructorPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<InstructorDTO>> Get(int id)
        {
            //Validar existencia del instructor
            var instructor = await context.Instructores
                .ProjectTo<InstructorDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            //Identificar los alumnos con el mismo turno que el instructor
            var alumnos = await context.Turnos
                .Where(t => t.InstructorId == id)
                .SelectMany(t => t.TurnoAlumnos.Select(ta => ta.Alumnos))
                .ToListAsync();

            if(alumnos is null)
            {
                return NotFound("No existen alumnos");
            }

            //Calcular la ganancia del instructor de acuerdo a la frecuencia del alumno
            foreach (var alumno in alumnos)
            {
                var ganancias = await context.Tarifas.FirstOrDefaultAsync();

                //Validar que existan las tarifas
                if(ganancias is null)
                {
                    return NotFound("Los valores de tarifas no han sido asignados");
                }

                decimal gananciaAlumnos = 0;
                decimal gananciaInstructor = 0;

                if (alumno.FrecuenciaTurno == 0)
                {
                    return NotFound("El alumno no asiste a ningun turno");
                }
                else if(alumno.FrecuenciaTurno == 2)
                {
                    gananciaAlumnos = ganancias.Valor2Turnos;
                }
                else if (alumno.FrecuenciaTurno == 3)
                {
                    gananciaAlumnos = ganancias.Valor3Turnos;
                }
                else
                {
                    gananciaAlumnos = ganancias.ValorClaseIndividual;
                }

                gananciaInstructor = gananciaAlumnos * instructor.PorcentajeDeGanancia/100;
                instructor.Ganancia = gananciaInstructor;
            }

            await context.SaveChangesAsync();


            return Ok(instructor);


        }



        [HttpPost]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Post([FromForm] InstructorCreationDTO instructorCreationDTO)
        {
            //Validar que no haya otro instructor con el mismo nombre y apellido sin importar mayusculas
            var instructorExiste = await context.Instructores
                .AnyAsync(i => i.Nombre.ToLower() == instructorCreationDTO.Nombre.ToLower() && 
                i.Apellido.ToLower() == instructorCreationDTO.Apellido.ToLower());

            var instructor = mapper.Map<Instructor>(instructorCreationDTO);
            context.Add(instructor);
            await context.SaveChangesAsync();

            var instructorDTO = mapper.Map<InstructorDTO>(instructor);
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return CreatedAtRoute("ObtenerInstructorPorId", new { id = instructor.Id }, instructorCreationDTO);
        }


        [HttpPut("{id:int}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Put(int id,[FromForm] InstructorCreationDTO instructorCreationDTO)
        {
            var instructor = await context.Instructores.FirstOrDefaultAsync(i => i.Id == id);

            if(instructor is null)
            {
                return NotFound();
            }
            
            instructor = mapper.Map(instructorCreationDTO, instructor);

            context.Update(instructor);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existenInstructores = await context.Instructores.Where(i => i.Id == id).ExecuteDeleteAsync();

            if(existenInstructores == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
