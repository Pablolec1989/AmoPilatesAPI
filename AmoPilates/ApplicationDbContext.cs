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
                .WithMany(a => a.Turnos)
                .HasForeignKey(ta => ta.AlumnoId)
                .OnDelete(DeleteBehavior.Cascade); // O DeleteBehavior.Cascade, según tus necesidades

            // Configuración de la relación con Turno
            modelBuilder.Entity<TurnoAlumno>()
                .HasOne(ta => ta.Turnos)
                .WithMany(t => t.TurnoAlumnos)
                .HasForeignKey(ta => ta.TurnoId)
                .OnDelete(DeleteBehavior.Cascade);

            //Config relacion con instructor
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Instructor)
                .WithMany(i => i.Turnos) // Especifica la propiedad de navegación en Instructor
                .HasForeignKey(t => t.InstructorId);


            //Config relacion Ganancias
            modelBuilder.Entity<Tarifas>(entity =>
            {
                entity.Property(e => e.Valor2Turnos)
                    .HasColumnType("decimal(18,2)");
            });
            
            modelBuilder.Entity<Tarifas>(entity =>
            {
                entity.Property(e => e.Valor3Turnos)
                    .HasColumnType("decimal(18,2)"); 
            });

            modelBuilder.Entity<Tarifas>(entity =>
            {
                entity.Property(e => e.ValorClaseIndividual)
                    .HasColumnType("decimal(18,2)"); 
            });

            modelBuilder.Entity<Tarifas>(entity =>
            {
                entity.Property(e => e.GananciaTotal)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Tarifas>(entity =>
            {
                entity.Property(e => e.FechaCalculo)
                    .HasColumnType("datetime");
            });
        }



        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Instructor> Instructores { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<TurnoAlumno> TurnosAlumnos { get; set; }
        public DbSet<Tarifas> Tarifas { get; set; }

    }
}
