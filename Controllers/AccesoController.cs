using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AvicolaApp.Data;
using AvicolaApp.Models;

namespace AvicolaApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string user, string password)
        {
            var usuario = await _context.Usuarios
                                 .Include(u => u.Rol)
                                        .FirstOrDefaultAsync(u => u.UserName == user || u.UserEmail == user);

            if (usuario == null || usuario.Password != password)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim(ClaimTypes.Email, usuario.UserEmail),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Denegado()
        {
            return View();
        }

        //[HttpGet]
        //public IActionResult Register()
        //{
        //    return View("Views/Register/Register.cshtml");
        //}

        public async Task<IActionResult> Salir()
        {
            // Borra la cookie y te manda al login
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}