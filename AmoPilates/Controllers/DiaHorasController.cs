using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/diashoras")]
    public class DiaHorasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "diashoras";

        public DiaHorasController(ApplicationDbContext context, 
            IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }


        [HttpGet]
        public async Task<ActionResult<List<diaHoraDTO>>> Get()
        {
            return await context.DiaHoras
                .ProjectTo<diaHoraDTO>(mapper.ConfigurationProvider)
                .OrderBy(d => d.Dia)
                .ToListAsync();
        }


        [HttpGet("{id:int}", Name = "obtenerDiaHoraPorId")]
        public async Task<ActionResult<diaHoraDTO>> Get(int id)
        {
            var diaHora = await context.DiaHoras.FirstOrDefaultAsync(d => d.Id == id);

            if (diaHora == null)
            {
                return NotFound("No encontrado");
            }

            var diaHoraDTO = mapper.Map<diaHoraDTO>(diaHora);

            return diaHoraDTO;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromForm] diaHoraCreationDTO diaHoraCreationDTO)
        {

            var diaHoraExiste = await context.DiaHoras
                .AnyAsync(d => d.Dia == diaHoraCreationDTO.Dia && d.Hora == diaHoraCreationDTO.Hora);

            if (diaHoraExiste)
            {
                return BadRequest("Este día y hora ya existe");
            }
            
            var diaHora = mapper.Map<DiaHoraDTO>(diaHoraCreationDTO);
            
            context.Add(diaHora);
            await context.SaveChangesAsync();

            var diaHoraDTO = mapper.Map<diaHoraDTO>(diaHora);

            return CreatedAtRoute("obtenerDiaHoraPorId", new { id = diaHoraDTO.Id }, diaHoraDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] diaHoraCreationDTO diaHoraCreationDTO)
        {
            var diaHora = await context.DiaHoras.FirstOrDefaultAsync(d => d.Id == id);

            if (diaHora == null)
            {
                return NotFound();
            }

            diaHora = mapper.Map(diaHoraCreationDTO, diaHora);
            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var diaHora = await context.DiaHoras.FirstOrDefaultAsync(d => d.Id == id);

            if (diaHora == null)
            {
                return NotFound();
            }

            context.Remove(diaHora);
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
