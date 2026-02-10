using AvicolaApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvicolaApp.Repository
{
    public interface IUsuariosRepository
    {
        Task<List<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorNombreOEmailAsync(string nombreOEmail);
        Task GuardarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task EliminarLogicamenteAsync(int id);
        Task<int> ObtenerTotalAsync();
    }
}