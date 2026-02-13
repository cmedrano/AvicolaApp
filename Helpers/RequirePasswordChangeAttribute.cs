using AvicolaApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AvicolaApp.Helpers
{
    /// <summary>
    /// Atributo que verifica si el usuario debe cambiar contraseña
    /// Si es así, lo redirige a la pantalla de cambio obligatorio
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePasswordChangeAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Obtener el ID del usuario desde los claims
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Si el usuario no está autenticado, dejar que continúe
            if (string.IsNullOrEmpty(userIdClaim))
            {
                await next();
                return;
            }

            // Permitir acceso a las acciones de cambio de contraseña y logout
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
            var action = context.RouteData.Values["action"]?.ToString() ?? "";

            if (controller.Equals("Acceso", StringComparison.OrdinalIgnoreCase) && 
                (action.Equals("CambiarPasswordObligatorio", StringComparison.OrdinalIgnoreCase) || 
                 action.Equals("Salir", StringComparison.OrdinalIgnoreCase)))
            {
                await next();
                return;
            }

            // Obtener el DbContext
            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();
            if (dbContext == null || !int.TryParse(userIdClaim, out int userId))
            {
                await next();
                return;
            }

            // Verificar si el usuario debe cambiar contraseña
            var usuario = await dbContext.Usuarios.FindAsync(userId);
            if (usuario != null && usuario.DebeCambiarPassword)
            {
                // Redirigir a cambio de contraseña obligatorio
                context.Result = new RedirectToActionResult(
                    "CambiarPasswordObligatorio",
                    "Acceso",
                    null);
                return;
            }

            await next();
        }
    }
}
