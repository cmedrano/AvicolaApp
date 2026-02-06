using AvicolaApp.Data;
using AvicolaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvicolaApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int totalClientes = await _context.Clientes.CountAsync();
            ViewBag.TotalClientes = totalClientes;

            int totalUsuarios = await _context.Usuarios.CountAsync();
            ViewBag.TotalUsuarios = totalUsuarios;

            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = roles;  

            var listaUsuarios = await _context.Usuarios.Include(u => u.Rol).ToListAsync();

            return View(listaUsuarios);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarUsuario(string UserName, string UserEmail, string Password, int RolId)
        {
            var nuevoUsuario = new Usuario
            {
                UserName = UserName,
                UserEmail = UserEmail,
                Password = Password,
                RolId = RolId,
                CreateDate = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(int Id, string UserName, string UserEmail, int RolId)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(Id);

            if (usuarioExistente != null)
            {
                usuarioExistente.UserName = UserName;
                usuarioExistente.UserEmail = UserEmail;
                usuarioExistente.RolId = RolId;

                _context.Usuarios.Update(usuarioExistente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}