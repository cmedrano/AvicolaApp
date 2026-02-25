using AvicolaApp.Data;
using AvicolaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AvicolaApp.Repository
{
    public class LocalidadPostalRepository : ILocalidadPostalRepository
    {
        private readonly ApplicationDbContext _context;

        public LocalidadPostalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocalidadPostal>> ObtenerPorCodigoPostalAsync(string codigoPostal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal))
                return new List<LocalidadPostal>();

            return await _context.localidades_postales
                     .Where(x => x.CodigoPostal.Trim() == codigoPostal.Trim()) // Limpia ambos lados
                     .OrderBy(x => x.Localidad)
                     .ToListAsync(); ;
        }

        public async Task<LocalidadPostal?> ObtenerPorCodigoPostalPrimeroAsync(string codigoPostal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal))
                return null;

            return await _context.localidades_postales
                .Where(x => x.CodigoPostal == codigoPostal)
                .FirstOrDefaultAsync();
        }
    }
}
