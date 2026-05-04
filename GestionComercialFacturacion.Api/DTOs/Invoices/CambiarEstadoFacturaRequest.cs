using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Invoices;

public class CambiarEstadoFacturaRequest
{
    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;
}