using AvicolaApp.Data;
using AvicolaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AvicolaApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
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

            return View();
        }
        public IActionResult Privacy() => View();
        public IActionResult Clientes() => View();


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
