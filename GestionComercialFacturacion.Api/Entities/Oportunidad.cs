namespace GestionComercialFacturacion.Api.Entities;

public class LineaFactura
{
    public int Id { get; set; }

    public int FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PorcentajeImpuesto { get; set; }

    public decimal SubtotalLinea { get; set; }
    public decimal ValorImpuestoLinea { get; set; }
    public decimal TotalLinea { get; set; }
}