using AvicolaApp.Models;
using AvicolaApp.Models.DTOs;
using AvicolaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvicolaApp.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;
        private const int PageSize = 10;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index(int page = 1, string? searchNombre = null, string? searchFantasia = null)
        {
            if (page < 1)
                page = 1;

            var resultado = await _clienteService.ObtenerPaginadosAsync(page, PageSize, searchNombre, searchFantasia);

            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = PageSize;
            ViewData["SearchNombre"] = searchNombre ?? "";
            ViewData["SearchFantasia"] = searchFantasia ?? "";

            return View(resultado);
        }

        [HttpPost(Name = "GuardarCliente")]
        public async Task<IActionResult> GuardarCliente(
            string Nombre,
            string? Telefono,
            string? Domicilio,
            string? Localidad,
            string? Provincia,
            string? CodigoPostal,
            string? Email,
            string? Celular,
            string? DNI,
            string? CUIT,
            string? Fantasia,
            string? Categoria,
            bool OperacionesContado,
            bool InhabilitadoFacturar)
        {
            // Validar que el Nombre no esté vacío
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Error"] = "El nombre es obligatorio";
                return RedirectToAction("Index");
            }

            // Validar email duplicado si se proporciona
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var clienteExistente = await _clienteService.ObtenerPorEmailAsync(Email);
                if (clienteExistente != null)
                {
                    TempData["Error"] = "El email ya está registrado en otro cliente activo";
                    return RedirectToAction("Index");
                }
            }

            var nuevoCliente = new Cliente
            {
                Nombre = Nombre,
                Telefono = Telefono,
                Domicilio = Domicilio,
                Localidad = Localidad,
                Provincia = Provincia,
                CodigoPostal = CodigoPostal,
                Email = Email,
                Celular = Celular,
                DNI = DNI,
                CUIT = CUIT,
                Fantasia = Fantasia,
                Categoria = Categoria,
                OperacionesContado = OperacionesContado,
                InhabilitadoFacturar = InhabilitadoFacturar
            };

            await _clienteService.GuardarAsync(nuevoCliente);
            TempData["Exito"] = "Cliente registrado exitosamente";

            return RedirectToAction("Index");
        }

        [HttpPost(Name = "EditarCliente")]
        public async Task<IActionResult> EditarCliente(
            int Id,
            string Nombre,
            string? Telefono,
            string? Domicilio,
            string? Localidad,
            string? Provincia,
            string? CodigoPostal,
            string? Email,
            string? Celular,
            string? DNI,
            string? CUIT,
            string? Fantasia,
            string? Categoria,
            bool OperacionesContado,
            bool InhabilitadoFacturar)
        {
            var clienteExistente = await _clienteService.ObtenerPorIdAsync(Id);

            if (clienteExistente == null)
            {
                TempData["Error"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            // Validar email duplicado si el email cambió y no está vacío
            if (!string.IsNullOrWhiteSpace(Email) && Email != clienteExistente.Email)
            {
                var clienteConEmailDuplicado = await _clienteService.ObtenerPorEmailAsync(Email);
                if (clienteConEmailDuplicado != null)
                {
                    TempData["Error"] = "El email ya está registrado en otro cliente activo";
                    return RedirectToAction("Index");
                }
            }

            clienteExistente.Nombre = Nombre;
            clienteExistente.Telefono = Telefono;
            clienteExistente.Domicilio = Domicilio;
            clienteExistente.Localidad = Localidad;
            clienteExistente.Provincia = Provincia;
            clienteExistente.CodigoPostal = CodigoPostal;
            clienteExistente.Email = Email;
            clienteExistente.Celular = Celular;
            clienteExistente.DNI = DNI;
            clienteExistente.CUIT = CUIT;
            clienteExistente.Fantasia = Fantasia;
            clienteExistente.Categoria = Categoria;
            clienteExistente.OperacionesContado = OperacionesContado;
            clienteExistente.InhabilitadoFacturar = InhabilitadoFacturar;

            await _clienteService.ActualizarAsync(clienteExistente);
            TempData["Exito"] = "Cliente actualizado exitosamente";

            return RedirectToAction("Index");
        }

        [HttpPost(Name = "EliminarCliente")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);

            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado";
                return RedirectToAction("Index");
            }

            await _clienteService.EliminarAsync(id);
            TempData["Exito"] = "Cliente eliminado exitosamente";

            return RedirectToAction("Index");
        }

        [HttpGet(Name = "ValidarEmail")]
        public async Task<IActionResult> ValidarEmail(string email, int? clienteId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { valido = true });
            }

            var clienteExistente = await _clienteService.ObtenerPorEmailAsync(email);
            
            // Si es para editar, verificar que el email no pertenezca a otro cliente
            if (clienteId.HasValue)
            {
                var clienteActual = await _clienteService.ObtenerPorIdAsync(clienteId.Value);
                if (clienteExistente != null && clienteExistente.Id != clienteId.Value)
                {
                    return Json(new { valido = false, mensaje = "Este email ya está registrado en otro cliente activo" });
                }
            }
            else
            {
                // Si es para crear, verificar que no exista
                if (clienteExistente != null)
                {
                    return Json(new { valido = false, mensaje = "Este email ya está registrado en otro cliente activo" });
                }
            }

            return Json(new { valido = true });
        }
    }
}