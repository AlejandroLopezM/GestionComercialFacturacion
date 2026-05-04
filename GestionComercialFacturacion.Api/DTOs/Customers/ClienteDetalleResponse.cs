using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Customers;

public class ClienteDetalleResponse : ClienteResponse
{
    [JsonPropertyName("contacts")]
    public List<ContactoClienteDetalleResponse> Contactos { get; set; } = new();

    [JsonPropertyName("opportunities")]
    public List<OportunidadClienteDetalleResponse> Oportunidades { get; set; } = new();

    [JsonPropertyName("invoices")]
    public List<FacturaClienteDetalleResponse> Facturas { get; set; } = new();
}

public class ContactoClienteDetalleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

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

public class OportunidadClienteDetalleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("estimatedAmount")]
    public decimal MontoEstimado { get; set; }

    [JsonPropertyName("expectedCloseDate")]
    public DateTime FechaEstimadaCierre { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("closedAt")]
    public DateTime? FechaCierre { get; set; }
}

public class FacturaClienteDetalleResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public string NumeroFactura { get; set; } = string.Empty;

    [JsonPropertyName("opportunityId")]
    public int? OportunidadId { get; set; }

    [JsonPropertyName("issueDate")]
    public DateTime FechaEmision { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime FechaVencimiento { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("taxAmount")]
    public decimal ValorImpuesto { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;
}