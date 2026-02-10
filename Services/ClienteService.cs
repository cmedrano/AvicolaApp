using AvicolaApp.Models;
using AvicolaApp.Models.DTOs;
using AvicolaApp.Repository;
using AvicolaApp.Services.Interfaces;

namespace AvicolaApp.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public Task<List<Cliente>> ObtenerTodosAsync() => _clienteRepository.ObtenerTodosAsync();

        public Task<Cliente?> ObtenerPorIdAsync(int id) => _clienteRepository.ObtenerPorIdAsync(id);

        public Task<Cliente?> ObtenerPorCuitAsync(string cuit) => _clienteRepository.ObtenerPorCuitAsync(cuit);

        public Task<Cliente?> ObtenerPorEmailAsync(string email) => _clienteRepository.ObtenerPorEmailAsync(email);

        public Task GuardarAsync(Cliente cliente) => _clienteRepository.GuardarAsync(cliente);

        public Task ActualizarAsync(Cliente cliente) => _clienteRepository.ActualizarAsync(cliente);

        public Task EliminarAsync(int id) => _clienteRepository.EliminarAsync(id);

        public Task<int> ObtenerTotalAsync() => _clienteRepository.ObtenerTotalAsync();

        public Task<PaginatedResult<Cliente>> ObtenerPaginadosAsync(int pageNumber, int pageSize, string? searchNombre = null, string? searchFantasia = null)
            => _clienteRepository.ObtenerPaginadosAsync(pageNumber, pageSize, searchNombre, searchFantasia);
    }
}