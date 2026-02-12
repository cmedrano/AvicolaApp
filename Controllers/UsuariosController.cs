using AvicolaApp.Models;
using AvicolaApp.Services;
using AvicolaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvicolaApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly IClienteService _clienteService;
        private readonly IAutenticacionService _autenticacionService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IUsuarioService usuarioService,
            IRolService rolService,
            IClienteService clienteService,
            IAutenticacionService autenticacionService,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _clienteService = clienteService;
            _autenticacionService = autenticacionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var listaUsuarios = await _usuarioService.ObtenerTodosAsync();
            ViewBag.TotalUsuarios = await _usuarioService.ObtenerTotalAsync();
            ViewBag.TotalClientes = await _clienteService.ObtenerTotalAsync();
            ViewBag.Roles = await _rolService.ObtenerTodosAsync();

            return View(listaUsuarios);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarUsuario(string UserName, string UserEmail, string Password, int RolId)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(UserEmail) ||
                string.IsNullOrWhiteSpace(Password) || RolId <= 0)
            {
                TempData["Error"] = "Todos los campos son obligatorios";
                return RedirectToAction("Index");
            }

            try
            {
                // Verificar que no exista otro usuario con el mismo nombre o email
                var usuarioExistente = await _autenticacionService.ObtenerUsuarioPorNombreOEmailAsync(UserName);
                if (usuarioExistente != null)
                {
                    TempData["Error"] = "El usuario o email ya existe";
                    return RedirectToAction("Index");
                }

                var nuevoUsuario = new Usuario
                {
                    UserName = UserName,
                    UserEmail = UserEmail,
                    Password = _autenticacionService.HashearPassword(Password),
                    RolId = RolId
                };

                await _usuarioService.GuardarAsync(nuevoUsuario);
                TempData["Exito"] = "Usuario creado exitosamente";
                _logger.LogInformation($"Usuario {UserName} creado por {User.Identity?.Name}");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                TempData["Error"] = "Error al crear el usuario";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GenerarHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                return BadRequest("Password requerido");

            var hash = _autenticacionService.HashearPassword(password);
            return Ok(new { password = password, hash = hash });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult VerificarHash(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return BadRequest("Password y hash son requeridos");

            var esValido = _autenticacionService.VerificarPassword(password, hash);
            return Ok(new { password = password, hash = hash, esValido = esValido });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(int Id, string UserName, string UserEmail, int RolId, string Password)
        {
            _logger.LogInformation($"=== INICIO EditarUsuario === Id: {Id}, UserName: {UserName}, UserEmail: {UserEmail}, RolId: {RolId}, Password recibida: {(string.IsNullOrWhiteSpace(Password) ? "VACIA/NULA" : $"PRESENTE ({Password.Length} caracteres)")}");

            if (Id <= 0)
            {
                _logger.LogWarning("EditarUsuario: Id inválido");
                TempData["Error"] = "ID de usuario inválido";
                return RedirectToAction("Index");
            }

            var usuarioExistente = await _usuarioService.ObtenerPorIdAsync(Id);

            if (usuarioExistente == null)
            {
                _logger.LogWarning($"EditarUsuario: Usuario con ID {Id} no encontrado");
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction("Index");
            }

            try
            {
                bool emailCambiado = usuarioExistente.UserEmail != UserEmail;
                bool rolCambiado = usuarioExistente.RolId != RolId;
                
                // Validar que el email sea válido si se proporcionó
                if (string.IsNullOrWhiteSpace(UserEmail))
                {
                    TempData["Error"] = "El correo electrónico no puede estar vacío";
                    return RedirectToAction("Index");
                }

                // Validar que el rol sea válido
                if (RolId <= 0)
                {
                    TempData["Error"] = "Debes seleccionar un rol válido";
                    return RedirectToAction("Index");
                }

                // Si el email cambió, verificar que no exista otro usuario con ese email
                if (emailCambiado)
                {
                    var usuarioConMismoEmail = await _autenticacionService.ObtenerUsuarioPorNombreOEmailAsync(UserEmail);
                    if (usuarioConMismoEmail != null && usuarioConMismoEmail.Id != Id)
                    {
                        TempData["Error"] = "Ya existe otro usuario con este correo electrónico";
                        return RedirectToAction("Index");
                    }
                }

                // Actualizar email si cambió
                if (emailCambiado)
                {
                    usuarioExistente.UserEmail = UserEmail;
                    _logger.LogInformation($">>> Email actualizado de {usuarioExistente.UserEmail} a {UserEmail}");
                }

                // Actualizar rol si cambió
                if (rolCambiado)
                {
                    usuarioExistente.RolId = RolId;
                    _logger.LogInformation($">>> Rol actualizado a {RolId}");
                }

                // Solo actualizar la contraseña si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    _logger.LogInformation($">>> Generando nuevo hash para usuario {usuarioExistente.UserName} con contraseña de {Password.Length} caracteres");
                    
                    var nuevoHash = _autenticacionService.HashearPassword(Password);
                    _logger.LogInformation($">>> Hash generado: {nuevoHash.Substring(0, Math.Min(20, nuevoHash.Length))}... (longitud: {nuevoHash.Length})");
                    
                    // Verificar que el nuevo hash sea válido
                    if (string.IsNullOrEmpty(nuevoHash) || nuevoHash.Length < 60)
                    {
                        throw new Exception($"Hash generado inválido. Longitud: {nuevoHash?.Length ?? 0}");
                    }
                    
                    var hashAnterior = usuarioExistente.Password;
                    usuarioExistente.Password = nuevoHash;
                    _logger.LogInformation($">>> Contraseña asignada a la entidad. Hash anterior: {hashAnterior?.Substring(0, 20)}...");
                }

                // Guardar los cambios
                await _usuarioService.ActualizarAsync(usuarioExistente);
                _logger.LogInformation($">>> Cambios guardados en BD");

                // Si se cambió la contraseña, verificar que se guardó correctamente
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    var usuarioVerificado = await _usuarioService.ObtenerPorIdAsync(Id);
                    var esValida = _autenticacionService.VerificarPassword(Password, usuarioVerificado.Password);
                    _logger.LogInformation($">>> Verificación post-guardado: Hash en BD = {usuarioVerificado.Password?.Substring(0, 20)}..., ¿Válida? {esValida}");
                    TempData["Exito"] = "Usuario actualizado exitosamente";
                }
                else if (emailCambiado || rolCambiado)
                {
                    TempData["Exito"] = "Usuario actualizado exitosamente";
                }
                else
                {
                    TempData["Aviso"] = "No se realizaron cambios";
                }

                _logger.LogInformation($"=== ÉXITO === Usuario {usuarioExistente.UserName} actualizado por {User.Identity?.Name}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "!!! ERROR al actualizar usuario ID: {UserId} | Exception: {ExceptionMessage}", Id, ex.Message);
                TempData["Error"] = $"Error al actualizar el usuario: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Index");
                }

                await _usuarioService.EliminarLogicamenteAsync(id);
                TempData["Exito"] = "Usuario eliminado exitosamente";
                _logger.LogInformation($"Usuario {usuario.UserName} eliminado lógicamente por {User.Identity?.Name}");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                TempData["Error"] = "Error al eliminar el usuario";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DiagnosticoPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest("Username y password son requeridos");

            try
            {
                var usuario = await _usuarioService.ObtenerPorNombreOEmailAsync(username);
                
                if (usuario == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                var hashAlmacenado = usuario.Password;
                var esValido = _autenticacionService.VerificarPassword(password, hashAlmacenado);
                var nuevoHash = _autenticacionService.HashearPassword(password);

                return Ok(new
                {
                    usuario = usuario.UserName,
                    passwordIngresado = password,
                    hashAlmacenado = hashAlmacenado,
                    longitudHashAlmacenado = hashAlmacenado?.Length ?? 0,
                    esValido = esValido,
                    nuevoHash = nuevoHash,
                    longitudNuevoHash = nuevoHash.Length,
                    esHashBCryptValido = hashAlmacenado?.StartsWith("$2") ?? false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}