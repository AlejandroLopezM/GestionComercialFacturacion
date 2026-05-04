using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Opportunities;

public class CrearOportunidadRequest
{
    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("estimatedAmount")]
    public decimal MontoEstimado { get; set; }

    [JsonPropertyName("expectedCloseDate")]
    public DateTime FechaEstimadaCierre { get; set; }
}