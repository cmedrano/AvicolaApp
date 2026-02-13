namespace AvicolaApp.Models.DTOs
{
    public class ClienteCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Domicilio { get; set; }
        public string? Email { get; set; }
        public string? Celular { get; set; }
        public string? Fax { get; set; }
        public string? Fantasia { get; set; }
        public string? Categoria { get; set; }
        public string? CondicionDeVenta { get; set; }
        public bool OperacionesContado { get; set; } = false;
        public bool InhabilitadoFacturar { get; set; } = false;
    }

    public class ClienteUpdateDto : ClienteCreateDto
    {
        public int Id { get; set; }
    }

    public class ClienteListDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Celular { get; set; }
        public string? Fantasia { get; set; }
        public string? Categoria { get; set; }
        public bool Activo { get; set; }
    }
}