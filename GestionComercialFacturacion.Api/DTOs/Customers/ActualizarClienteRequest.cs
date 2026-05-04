using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Customers;

public class ActualizarClienteRequest
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("taxId")]
    public string IdentificacionFiscal { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Telefono { get; set; }

    [JsonPropertyName("address")]
    public string? Direccion { get; set; }

    [JsonPropertyName("city")]
    public string? Ciudad { get; set; }

    [JsonPropertyName("postalCode")]
    public string? CodigoPostal { get; set; }

    [JsonPropertyName("country")]
    public string? Pais { get; set; }
}