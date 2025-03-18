using AmoPilates.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AmoPilates
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave compuesta para TurnoAlumno
            modelBuilder.Entity<TurnoAlumno>()
                .HasKey(ta => new { ta.AlumnoId, ta.TurnoId });

            // Configuración de la relación con Alumno
            modelBuilder.Entity<TurnoAlumno>()
                .HasOne(ta => ta.Alumnos)
                .WithMany(a => a.TurnosAlumno)
                .HasForeignKey(ta => ta.AlumnoId)
                .OnDelete(DeleteBehavior.Cascade); // O DeleteBehavior.Cascade, según tus necesidades

            // Configuración de la relación con Turno
            modelBuilder.Entity<TurnoAlumno>()
                .HasOne(ta => ta.Turnos)
                .WithMany(t => t.TurnosAlumnos)
                .HasForeignKey(ta => ta.TurnoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Instructor)
                .WithMany(i => i.Turnos) // Especifica la propiedad de navegación en Instructor
                .HasForeignKey(t => t.InstructorId);

            modelBuilder.Entity<Turno>().HasOne(t => t.DiaHora).WithMany()
                .HasForeignKey(t => t.DiaHoraId);
        }



        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Instructor> Instructores { get; set; }
        public DbSet<DiaHoraDTO> DiaHoras { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<TurnoAlumno> TurnosAlumnos { get; set; }

    }
}
