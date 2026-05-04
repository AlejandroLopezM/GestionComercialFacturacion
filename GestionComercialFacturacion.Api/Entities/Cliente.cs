using GestionComercialFacturacion.Api.Enums;
using System.Diagnostics.Contracts;

namespace GestionComercialFacturacion.Api.Entities;

public class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string IdentificacionFiscal { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Pais { get; set; }

    public EstadoCliente Estado { get; set; } = EstadoCliente.Activo;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
    public ICollection<Oportunidad> Oportunidades { get; set; } = new List<Oportunidad>();
    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}