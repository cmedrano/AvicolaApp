using AvicolaApp.Data;
using AvicolaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AvicolaApp.Repository
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuariosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .Include(u => u.Rol)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .Include(u => u.Rol)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObtenerPorNombreOEmailAsync(string nombreOEmail)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Activo)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UserName == nombreOEmail || u.UserEmail == nombreOEmail);

            if (usuario != null && usuario.Rol == null)
            {
                await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();
            }

            return usuario;
        }

        public async Task GuardarAsync(Usuario usuario)
        {
            usuario.CreateDate = DateTime.UtcNow;
            usuario.Activo = true;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            var usuarioEnBd = await _context.Usuarios.FindAsync(usuario.Id);
            if (usuarioEnBd != null)
            {
                usuarioEnBd.UserName = usuario.UserName;
                usuarioEnBd.UserEmail = usuario.UserEmail;
                usuarioEnBd.Password = usuario.Password;
                usuarioEnBd.RolId = usuario.RolId;
                usuarioEnBd.Activo = usuario.Activo;

                _context.Usuarios.Update(usuarioEnBd);
                await _context.SaveChangesAsync();
                
                await _context.Entry(usuarioEnBd).Reference(u => u.Rol).LoadAsync();
                
                usuario.Rol = usuarioEnBd.Rol;
            }
        }

        public async Task EliminarLogicamenteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ObtenerTotalAsync()
        {
            return await _context.Usuarios.Where(u => u.Activo).CountAsync();
        }
    }
}
