using AmoPilates.DTOs;
using AmoPilates.Entidades;
using AmoPilates.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Controllers
{
    [ApiController]
    [Route("api/tarifas")]
    public class TarifasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;

        public TarifasController(ApplicationDbContext applicationDbContext, 
            IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            this.context = applicationDbContext;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }


        [HttpGet]
        public async Task<ActionResult<TarifaDTO>> Get()
        {
            //Traer alumnos y Tarifas
            var alumnos = await context.Alumnos.FirstOrDefaultAsync();
            var tarifas = await context.Tarifas.FirstOrDefaultAsync();

            //Traer datos de ganancias
            var alumnosIndividual = await context.Alumnos.Where(a => a.FrecuenciaTurno < 1 && a.FrecuenciaTurno >3).ToListAsync();
            var alumnos2Turnos = await context.Alumnos.Where(a => a.FrecuenciaTurno == 2).ToListAsync();
            var alumnos3Turnos = await context.Alumnos.Where(a => a.FrecuenciaTurno == 3).ToListAsync();

            decimal gananciaTotal = (alumnosIndividual.Count() * tarifas.ValorClaseIndividual) + (alumnos2Turnos.Count() * tarifas.Valor2Turnos) + (alumnos3Turnos.Count() * tarifas.Valor3Turnos);

            tarifas.GananciaTotal = gananciaTotal;

            await context.SaveChangesAsync();

            var tarifasDTO = mapper.Map<TarifaDTO>(tarifas);

            return Ok(tarifasDTO);

        }


        [HttpPost]
        public async Task<ActionResult> Post([FromForm] TarifaCreationDTO gananciaCreationDTO)
        {
            var ganancia = mapper.Map<Tarifas>(gananciaCreationDTO);
            await context.Tarifas.AddAsync(ganancia);
            await context.SaveChangesAsync();
            return NoContent();
        }



    }
}
