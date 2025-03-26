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

        public async Task<bool> ValidarAlumnos(List<int> alumnosIds)
        {
            var alumnosExistentes = await context.Alumnos
                .Where(a => alumnosIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            if (alumnosExistentes.Count != alumnosIds.Count)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ValidarAlumnosDuplicados(List<int> alumnoIds, int turnoId = 0)
        {
            // Verificar si algún alumno ya está asignado al turno
            var alumnosDuplicados = await context.TurnosAlumnos
                .Where(ta => ta.TurnoId == turnoId && alumnoIds.Contains(ta.AlumnoId))
                .ToListAsync();

            return alumnosDuplicados.Any(); // Retorna true si hay duplicados
        }
    }
}
