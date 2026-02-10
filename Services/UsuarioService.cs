using AvicolaApp.Models;
using AvicolaApp.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvicolaApp.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuariosRepository _usuariosRepository;

        public UsuarioService(IUsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        public Task<List<Usuario>> ObtenerTodosAsync() => _usuariosRepository.ObtenerTodosAsync();

        public Task<Usuario?> ObtenerPorIdAsync(int id) => _usuariosRepository.ObtenerPorIdAsync(id);

        public Task<Usuario?> ObtenerPorNombreOEmailAsync(string nombreOEmail) => _usuariosRepository.ObtenerPorNombreOEmailAsync(nombreOEmail);

        public Task GuardarAsync(Usuario usuario) => _usuariosRepository.GuardarAsync(usuario);

        public Task ActualizarAsync(Usuario usuario) => _usuariosRepository.ActualizarAsync(usuario);

        public Task EliminarLogicamenteAsync(int id) => _usuariosRepository.EliminarLogicamenteAsync(id);

        public Task<int> ObtenerTotalAsync() => _usuariosRepository.ObtenerTotalAsync();
    }
}