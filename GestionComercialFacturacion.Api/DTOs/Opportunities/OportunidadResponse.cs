using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Opportunities;

public class OportunidadResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("customerId")]
    public int ClienteId { get; set; }

    [JsonPropertyName("customerName")]
    public string? NombreCliente { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Descripcion { get; set; }

    [JsonPropertyName("estimatedAmount")]
    public decimal MontoEstimado { get; set; }

    [JsonPropertyName("expectedCloseDate")]
    public DateTime FechaEstimadaCierre { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime FechaCreacion { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? FechaActualizacion { get; set; }

    [JsonPropertyName("closedAt")]
    public DateTime? FechaCierre { get; set; }
}