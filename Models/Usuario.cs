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
        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; } = null!;
    }
}