using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Servicios
{
    public class AlumnosService : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AlumnosService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult> ValidarAlumnos(List<int> alumnosIds)
        {
            var alumnosExistentes = await context.Alumnos
                .Where(a => alumnosIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            if (alumnosExistentes.Count != alumnosIds.Count)
            {
                var alumnosNoExistentes = alumnosIds.Except(alumnosExistentes).ToList();
                return BadRequest($"El/Los alumnos de id: {string.Join(", ", alumnosNoExistentes)}");
            }

            return null; // Indica que la validación fue exitosa
        }
    }
}
