using GestionComercialFacturacion.Api.Enums;

namespace GestionComercialFacturacion.Api.Entities;

public class Oportunidad
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public decimal MontoEstimado { get; set; }
    public DateTime FechaEstimadaCierre { get; set; }

    public EstadoOportunidad Estado { get; set; } = EstadoOportunidad.Abierta;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaCierre { get; set; }

    public ICollection<ActividadComercial> Actividades { get; set; } = new List<ActividadComercial>();

    public Factura? Factura { get; set; }
}