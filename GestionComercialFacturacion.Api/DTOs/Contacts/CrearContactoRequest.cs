using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Contacts;

public class CrearContactoRequest
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Telefono { get; set; }

    [JsonPropertyName("position")]
    public string? Cargo { get; set; }

    [JsonPropertyName("isPrimary")]
    public bool EsPrincipal { get; set; }
}
