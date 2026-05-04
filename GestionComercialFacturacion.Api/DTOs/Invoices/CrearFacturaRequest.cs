using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Invoices;

public class CrearFacturaRequest
{
    [JsonPropertyName("issueDate")]
    public DateTime FechaEmision { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime FechaVencimiento { get; set; }

    [JsonPropertyName("taxPercentage")]
    public decimal PorcentajeImpuesto { get; set; }

    [JsonPropertyName("lines")]
    public List<CrearLineaFacturaRequest> Lineas { get; set; } = new();
}