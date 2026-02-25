using AvicolaApp.Models;

namespace AvicolaApp.Repository
{
    public interface ILocalidadPostalRepository
    {
        Task<List<LocalidadPostal>> ObtenerPorCodigoPostalAsync(string codigoPostal);
        Task<LocalidadPostal?> ObtenerPorCodigoPostalPrimeroAsync(string codigoPostal);
    }
}
