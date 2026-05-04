using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Activities;

public class CrearActividadRequest
{
    [JsonPropertyName("type")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("activityDate")]
    public DateTime FechaActividad { get; set; }
}