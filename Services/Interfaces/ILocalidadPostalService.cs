using AvicolaApp.Models;

namespace AvicolaApp.Services.Interfaces
{
    public interface ILocalidadPostalService
    {
        Task<List<LocalidadPostal>> ObtenerPorCodigoPostalAsync(string codigoPostal);
        Task<LocalidadPostal?> ObtenerPorCodigoPostalPrimeroAsync(string codigoPostal);
    }
}
