using AvicolaApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AvicolaApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Esta línea le dice a C# que existe una tabla Usuarios
        public DbSet<Usuario> Usuarios { get; set; }
    }
}