using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Invoices;

public class FacturaResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public string NumeroFactura { get; set; } = string.Empty;

    [JsonPropertyName("customerId")]
    public int ClienteId { get; set; }

    [JsonPropertyName("customerName")]
    public string NombreCliente { get; set; } = string.Empty;

    [JsonPropertyName("customerTaxId")]
    public string IdentificacionFiscalCliente { get; set; } = string.Empty;

    [JsonPropertyName("customerAddress")]
    public string? DireccionCliente { get; set; }

    [JsonPropertyName("customerCity")]
    public string? CiudadCliente { get; set; }

    [JsonPropertyName("customerPostalCode")]
    public string? CodigoPostalCliente { get; set; }

    [JsonPropertyName("customerCountry")]
    public string? PaisCliente { get; set; }

    [JsonPropertyName("opportunityId")]
    public int? OportunidadId { get; set; }

    [JsonPropertyName("issueDate")]
    public DateTime FechaEmision { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime FechaVencimiento { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("taxPercentage")]
    public decimal PorcentajeImpuesto { get; set; }

    [JsonPropertyName("taxAmount")]
    public decimal ValorImpuesto { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("status")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime FechaCreacion { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? FechaActualizacion { get; set; }

    [JsonPropertyName("lines")]
    public List<LineaFacturaResponse> Lineas { get; set; } = new();
}