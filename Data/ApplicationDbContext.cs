using AvicolaApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AvicolaApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions
            <ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la longitud de la columna Password para hashes BCrypt (60+ caracteres)
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Password)
                .HasColumnName("UserPasswordHash")
                .HasMaxLength(255);
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<LocalidadPostal> localidades_postales { get; set; }
    }
}