using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvicolaApp.Models
{
    [Table("localidades_postal")]
    public class LocalidadPostal
    {
        [Key]
        [Column("id_cod_postal")]
        public int IdCodPostal { get; set; }

        [Column("codigo_postal")]
        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        [Column("localidad")]
        [StringLength(100)]
        public string? Localidad { get; set; }

        [Column("id_provincia")]
        public int IdProvincia { get; set; }
    }
}
