using GestionComercialFacturacion.Api.Enums;

namespace GestionComercialFacturacion.Api.Entities;

public class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int? OportunidadId { get; set; }
    public Oportunidad? Oportunidad { get; set; }

    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }

    public decimal Subtotal { get; set; }
    public decimal PorcentajeImpuesto { get; set; }
    public decimal ValorImpuesto { get; set; }
    public decimal Total { get; set; }

    public EstadoFactura Estado { get; set; } = EstadoFactura.Emitida;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public ICollection<LineaFactura> Lineas { get; set; } = new List<LineaFactura>();
}