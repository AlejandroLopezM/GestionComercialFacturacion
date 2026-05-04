using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Activities;

public class ActividadResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("opportunityId")]
    public int OportunidadId { get; set; }

    [JsonPropertyName("type")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("activityDate")]
    public DateTime FechaActividad { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime FechaCreacion { get; set; }
}