using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvicolaApp.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }    
        public string UserEmail { get; set; }

        [Column("UserPasswordHash")]
        [StringLength(255)] // BCrypt hash requiere al menos 60 caracteres, pero dejamos 255 para seguridad
        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; } = null!;

        // Borrado lógico: 1 = activo, 0 = inactivo (borrado)
        public bool Activo { get; set; } = true;

        // Seguridad: obliga a cambiar contraseña en el primer login
        public bool DebeCambiarPassword { get; set; } = true;
    }
}