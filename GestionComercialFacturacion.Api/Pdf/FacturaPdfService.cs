using GestionComercialFacturacion.Api.DTOs.Invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionComercialFacturacion.Api.Pdf;

public class FacturaPdfService : IFacturaPdfService
{
    private readonly ILogger<FacturaPdfService> _logger;

    public FacturaPdfService(ILogger<FacturaPdfService> logger)
    {
        _logger = logger;
    }

    public byte[] Generar(FacturaResponse factura)
    {
        try
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("FACTURA")
                            .FontSize(22)
                            .Bold();

                        column.Item().Text(factura.NumeroFactura)
                            .FontSize(14)
                            .SemiBold();

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(15);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("Datos del cliente").Bold();
                                left.Item().Text(factura.NombreCliente);
                                left.Item().Text($"CIF/NIF: {factura.IdentificacionFiscalCliente}");
                                left.Item().Text($"Dirección: {factura.DireccionCliente}");
                                left.Item().Text($"Ciudad: {factura.CiudadCliente}");
                                left.Item().Text($"Código postal: {factura.CodigoPostalCliente}");
                                left.Item().Text($"País: {factura.PaisCliente}");

                                left.Item().PaddingTop(5).Text($"Cliente Id: {factura.ClienteId}");
                                left.Item().Text($"Oportunidad Id: {factura.OportunidadId}");
                            });

                            row.RelativeItem().Column(right =>
                            {
                                right.Item().Text("Datos de factura").Bold();
                                right.Item().Text($"Número: {factura.NumeroFactura}");
                                right.Item().Text($"Fecha emisión: {factura.FechaEmision:yyyy-MM-dd}");
                                right.Item().Text($"Fecha vencimiento: {factura.FechaVencimiento:yyyy-MM-dd}");
                                right.Item().Text($"Estado: {factura.Estado}");
                            });
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Concepto");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Cant.");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Precio");
                                header.Cell().Element(HeaderCell).AlignRight().Text("IVA");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                            });

                            foreach (var linea in factura.Lineas)
                            {
                                table.Cell().Element(BodyCell).Text(linea.Descripcion);
                                table.Cell().Element(BodyCell).AlignRight().Text(linea.Cantidad.ToString("0.##"));
                                table.Cell().Element(BodyCell).AlignRight().Text($"{linea.PrecioUnitario:N2}");
                                table.Cell().Element(BodyCell).AlignRight().Text($"{linea.ValorImpuestoLinea:N2}");
                                table.Cell().Element(BodyCell).AlignRight().Text($"{linea.TotalLinea:N2}");
                            }
                        });

                        column.Item().AlignRight().Width(220).Column(totals =>
                        {
                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Base imponible:");
                                row.ConstantItem(90).AlignRight().Text($"{factura.Subtotal:N2}");
                            });

                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"IVA {factura.PorcentajeImpuesto:0.##}%:");
                                row.ConstantItem(90).AlignRight().Text($"{factura.ValorImpuesto:N2}");
                            });

                            totals.Item().LineHorizontal(1);

                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("TOTAL:").Bold();
                                row.ConstantItem(90).AlignRight().Text($"{factura.Total:N2}").Bold();
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generado automáticamente por GestionComercialFacturacion.Api");
                    });
                });
            }).GeneratePdf();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando PDF de factura {NumeroFactura}", factura.NumeroFactura);
            throw;
        }
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Padding(5)
            .BorderBottom(1);
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .Padding(5)
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2);
    }
}