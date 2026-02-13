using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvicolaApp.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        [StringLength(255)]
        [Display(Name = "Domicilio")]
        public string? Domicilio { get; set; }

        [StringLength(100)]
        [Display(Name = "Localidad")]
        public string? Localidad { get; set; }

        [StringLength(100)]
        [Display(Name = "Provincia")]
        public string? Provincia { get; set; }

        [StringLength(20)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        [StringLength(200)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Celular")]
        public string? Celular { get; set; }

        [StringLength(20)]
        [Display(Name = "DNI")]
        public string? DNI { get; set; }

        [StringLength(20)]
        [Display(Name = "CUIT")]
        public string? CUIT { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [StringLength(100)]
        [Display(Name = "Fantasía")]
        public string? Fantasia { get; set; }

        [StringLength(100)]
        [Display(Name = "CondicionDeVenta")]
        public string? CondicionDeVenta { get; set; }

        [StringLength(100)]
        [Display(Name = "Categoría")]
        public string? Categoria { get; set; }

        [Display(Name = "Operaciones al Contado")]
        public bool OperacionesContado { get; set; } = false;

        [Display(Name = "Inhabilitado para Facturar")]
        public bool InhabilitadoFacturar { get; set; } = false;
    }
}