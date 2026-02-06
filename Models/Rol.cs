using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvicolaApp.Models
{
    [Table("Roles")]
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
