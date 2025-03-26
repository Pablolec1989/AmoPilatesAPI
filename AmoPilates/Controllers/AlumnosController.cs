using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AmoPilates.Utilidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/alumnos")]
    public class AlumnosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "alumnos";

        public AlumnosController(ApplicationDbContext context, 
            IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }


        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<AlumnoDTO>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Alumnos;
            await HttpContext.insertarParametrosPaginacionEnCabecera(queryable);

            return await queryable
                .OrderBy(a => a.Id)
                .Paginar(paginacion)
                .ProjectTo<AlumnoDTO>(mapper.ConfigurationProvider).
                ToListAsync();
        }


        [HttpGet("{id:int}", Name = "ObtenerAlumnoPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<AlumnoDTO>> Get(int id)
        {
            var alumno = await context.Alumnos
                .ProjectTo<AlumnoDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            if(alumno is null)
            {
                return NotFound();
            }

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return alumno;
        }


        [HttpPost]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Post([FromForm] AlumnoCreationDTO alumnoCreationDTO)
        {
            var alumno = mapper.Map<Alumno>(alumnoCreationDTO);

            //Validar que no exista mismo alumno con el mismo nombre y apellido
            var alumnoExiste = await context.Alumnos
                .AnyAsync(a => a.Nombre == alumnoCreationDTO.Nombre && a.Apellido == alumnoCreationDTO.Apellido);

            if (alumnoExiste)
            {
                return BadRequest("El alumno ya existe");
            }


            context.Add(alumno);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return CreatedAtRoute("ObtenerAlumnoPorId", new { id = alumno.Id }, alumnoCreationDTO);
        }


        [HttpPut("{id:int}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Put(int id, [FromForm] AlumnoCreationDTO alumnoCreationDTO)
        {
            var alumno = await context.Alumnos.FirstOrDefaultAsync(a => a.Id == id);

            if (alumno is null)
            {
                return NotFound();
            }

            alumno = mapper.Map(alumnoCreationDTO, alumno); //Piso el dato alumno
            context.Update(alumno);
            await context.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var alumnoBorrado = await context.Alumnos.Where(a => a.Id == id).ExecuteDeleteAsync();

            if(alumnoBorrado == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
