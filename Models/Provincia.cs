using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvicolaApp.Models
{
    [Table("provincias")]
    public class Provincia
    {
        [Key]
        [Column("id_provincia")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(100)]
        public string? Nombre { get; set; }
    }
}
