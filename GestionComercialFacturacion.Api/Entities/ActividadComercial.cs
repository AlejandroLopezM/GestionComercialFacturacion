using GestionComercialFacturacion.Api.Enums;

namespace GestionComercialFacturacion.Api.Entities;

public class ActividadComercial
{
    public int Id { get; set; }

    public int OportunidadId { get; set; }
    public Oportunidad Oportunidad { get; set; } = null!;

    public TipoActividadComercial Tipo { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaActividad { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}