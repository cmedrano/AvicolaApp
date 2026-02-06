using AvicolaApp.Data;
using AvicolaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvicolaApp.Controllers
{
    [Authorize(Roles = "Administrador, Operario")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listaClientes = await _context.Clientes.ToListAsync();
            return View(listaClientes);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCliente(string NombreRazonSocial, string Cuit, string Telefono, string Localidad, string TipoCliente)
        {
            var nuevoCliente = new Cliente
            {
                NombreRazonSocial = NombreRazonSocial,
                Cuit = Cuit,
                Telefono = Telefono,
                Localidad = Localidad,
                TipoCliente = TipoCliente,
                SaldoCuentaCorriente = 0,
                FechaRegistro = DateTime.Now
            };

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditarCliente(int Id, string NombreRazonSocial, string Cuit, string Telefono, string Localidad, string TipoCliente)
        {
            var clienteExistente = await _context.Clientes.FindAsync(Id);

            if (clienteExistente != null)
            {
                clienteExistente.NombreRazonSocial = NombreRazonSocial;
                clienteExistente.Cuit = Cuit;
                clienteExistente.Telefono = Telefono;
                clienteExistente.Localidad = Localidad;
                clienteExistente.TipoCliente = TipoCliente;

                _context.Clientes.Update(clienteExistente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
