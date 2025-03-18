﻿using AmoPilates.DTOs;
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
                .OrderBy(i => i.Nombre)
                .Paginar(paginacion)
                .ProjectTo<InstructorDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }


        [HttpGet("{id:int}", Name = "ObtenerInstructorPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<InstructorDTO>> Get (int id)
        {
            var instructor = await context.Instructores
                .FirstOrDefaultAsync(i => i.Id == id);

            if(instructor is null)
            {
                return NotFound();
            }
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            var instructorDTO = mapper.Map<InstructorDTO>(instructor);

            return instructorDTO;
        }


        [HttpPost]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult> Post([FromForm] InstructorCreationDTO instructorCreationDTO)
        {

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
