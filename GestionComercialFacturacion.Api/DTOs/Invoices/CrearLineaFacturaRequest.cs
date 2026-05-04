using System.Text.Json.Serialization;

namespace GestionComercialFacturacion.Api.DTOs.Invoices;

public class CrearLineaFacturaRequest
{
    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Cantidad { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal PrecioUnitario { get; set; }

    [JsonPropertyName("taxPercentage")]
    public decimal PorcentajeImpuesto { get; set; }
}