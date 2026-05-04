using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Opportunities;

public class CambiarEstadoOportunidadRequest
{
    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;
}