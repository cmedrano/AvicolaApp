using AvicolaApp.Data;
using AvicolaApp.Models;
using AvicolaApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AvicolaApp.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .AsNoTracking()
                .OrderByDescending(c => c.FechaRegistro)
                .ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> ObtenerPorCuitAsync(string cuit)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nombre == cuit);
        }

        public async Task<Cliente?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Activo && c.Email == email);
        }

        public async Task<PaginatedResult<Cliente>> ObtenerPaginadosAsync(int pageNumber, int pageSize, string? searchNombre = null, string? searchFantasia = null)
        {
            var query = _context.Clientes
                .Where(c => c.Activo)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchNombre))
            {
                var lowerSearchNombre = searchNombre.ToLower().Trim();
                query = query.Where(c => c.Nombre.ToLower().StartsWith(lowerSearchNombre));
            }

            if (!string.IsNullOrWhiteSpace(searchFantasia))
            {
                var lowerSearchFantasia = searchFantasia.ToLower().Trim();
                query = query.Where(c => c.Fantasia != null && c.Fantasia.ToLower().StartsWith(lowerSearchFantasia));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.FechaRegistro)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Cliente>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task GuardarAsync(Cliente cliente)
        {
            cliente.FechaRegistro = DateTime.UtcNow;
            cliente.Activo = true;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            var clienteExistente = await _context.Clientes.FindAsync(cliente.Id);

            if (clienteExistente != null)
            {
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Telefono = cliente.Telefono;
                clienteExistente.Domicilio = cliente.Domicilio;
                clienteExistente.Localidad = cliente.Localidad;
                clienteExistente.Provincia = cliente.Provincia;
                clienteExistente.CodigoPostal = cliente.CodigoPostal;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Celular = cliente.Celular;
                clienteExistente.DNI = cliente.DNI;
                clienteExistente.CUIT = cliente.CUIT;
                clienteExistente.Fantasia = cliente.Fantasia;
                clienteExistente.Categoria = cliente.Categoria;
                clienteExistente.OperacionesContado = cliente.OperacionesContado;
                clienteExistente.InhabilitadoFacturar = cliente.InhabilitadoFacturar;

                _context.Clientes.Update(clienteExistente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ObtenerTotalAsync()
        {
            return await _context.Clientes.Where(c => c.Activo).CountAsync();
        }
    }
}