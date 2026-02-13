using AvicolaApp.Data;
using AvicolaApp.Models;
using AvicolaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AvicolaApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IAutenticacionService _autenticacionService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccesoController> _logger;

        public AccesoController(
            IAutenticacionService autenticacionService,
            ApplicationDbContext context,
            ILogger<AccesoController> logger)
        {
            _autenticacionService = autenticacionService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Usuario y contraseña son requeridos");
                return View();
            }

            try
            {
                var usuarioBD = await _autenticacionService.ObtenerUsuarioPorNombreOEmailAsync(usuario);

                if (usuarioBD == null || !_autenticacionService.VerificarPassword(password, usuarioBD.Password))
                {
                    _logger.LogWarning($"Intento de login fallido para: {usuario}");
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                    return View();
                }

                // Login exitoso
                await SignInUsuario(usuarioBD);
                _logger.LogInformation($"Usuario {usuarioBD.UserName} ha iniciado sesión");

                // Si el usuario debe cambiar contraseña, redirigir a la página de cambio obligatorio
                if (usuarioBD.DebeCambiarPassword)
                {
                    _logger.LogInformation($"Usuario {usuarioBD.UserName} debe cambiar contraseña en el login");
                    return RedirectToAction("CambiarPasswordObligatorio", "Acceso");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                ModelState.AddModelError(string.Empty, "Error al procesar la solicitud");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            var username = User.Identity?.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation($"Usuario {username} ha cerrado sesión");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult CambiarPasswordObligatorio()
        {
            // Verificar que el usuario está autenticado
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPasswordObligatorio(string passwordActual, string passwordNueva, string passwordConfirmar)
        {
            // Verificar que el usuario está autenticado
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login");
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(passwordActual) || string.IsNullOrWhiteSpace(passwordNueva) || string.IsNullOrWhiteSpace(passwordConfirmar))
            {
                ModelState.AddModelError(string.Empty, "Todos los campos son requeridos");
                return View();
            }

            if (passwordNueva != passwordConfirmar)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden");
                return View();
            }

            if (passwordNueva.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "La contraseña debe tener al menos 6 caracteres");
                return View();
            }

            try
            {
                // Obtener el ID del usuario desde los claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogError("No se pudo obtener el ID del usuario autenticado");
                    ModelState.AddModelError(string.Empty, "Error al obtener datos del usuario");
                    return View();
                }

                // Obtener el usuario de la BD
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario con ID {userId} no encontrado");
                    return RedirectToAction("Login");
                }

                // Verificar que la contraseña actual sea correcta
                if (!_autenticacionService.VerificarPassword(passwordActual, usuario.Password))
                {
                    _logger.LogWarning($"Intento fallido de cambio de contraseña obligatorio para usuario {usuario.UserName}");
                    ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta");
                    return View();
                }

                // Actualizar la contraseña
                usuario.Password = _autenticacionService.HashearPassword(passwordNueva);
                usuario.DebeCambiarPassword = false;

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Usuario {usuario.UserName} cambió su contraseña obligatoria");

                // Redirigir al home
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña obligatoria");
                ModelState.AddModelError(string.Empty, "Error al procesar el cambio de contraseña");
                return View();
            }
        }

        public IActionResult Denegado()
        {
            return View();
        }

        private async Task SignInUsuario(Usuario usuario)
        {
            // Validar que el usuario y su rol existan
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario no puede ser nulo");
            }

            // Si el rol no está cargado, recargar desde la BD
            if (usuario.Rol == null)
            {
                // Recargar el usuario con su rol
                var usuarioReload = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Id == usuario.Id);

                if (usuarioReload?.Rol == null)
                {
                    throw new InvalidOperationException("No se pudo obtener el rol del usuario");
                }

                usuario = usuarioReload;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim(ClaimTypes.Email, usuario.UserEmail),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}