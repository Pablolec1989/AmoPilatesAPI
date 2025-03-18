using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Servicios
{
    public class InstructoresService
    {
        private readonly ApplicationDbContext context;

        public InstructoresService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> InstructorExiste(int instructorId)
        {
            return await context.Instructores.AnyAsync(i => i.Id == instructorId);

        }
    }

}
