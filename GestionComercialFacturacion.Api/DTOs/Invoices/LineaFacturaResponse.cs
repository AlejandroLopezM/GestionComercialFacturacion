using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Invoices;

public class LineaFacturaResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Cantidad { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal PrecioUnitario { get; set; }

    [JsonPropertyName("taxPercentage")]
    public decimal PorcentajeImpuesto { get; set; }

    [JsonPropertyName("lineSubtotal")]
    public decimal SubtotalLinea { get; set; }

    [JsonPropertyName("lineTaxAmount")]
    public decimal ValorImpuestoLinea { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal TotalLinea { get; set; }
}