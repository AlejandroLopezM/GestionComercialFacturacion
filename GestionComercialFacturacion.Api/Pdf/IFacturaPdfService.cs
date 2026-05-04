using GestionComercialFacturacion.Api.DTOs.Invoices;

namespace GestionComercialFacturacion.Api.Pdf;

public interface IFacturaPdfService
{
    byte[] Generar(FacturaResponse factura);
}