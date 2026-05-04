using GestionComercialFacturacion.Api.DTOs.Invoices;

namespace GestionComercialFacturacion.Api.Validators;

public static class FacturaValidator
{
    public static List<string> ValidarCrear(CrearFacturaRequest request)
    {
        var errores = new List<string>();

        if (request.FechaEmision == default)
            errores.Add("La fecha de emisión es obligatoria.");

        if (request.FechaVencimiento == default)
            errores.Add("La fecha de vencimiento es obligatoria.");

        if (request.FechaVencimiento.Date < request.FechaEmision.Date)
            errores.Add("La fecha de vencimiento no puede ser anterior a la fecha de emisión.");

        if (request.PorcentajeImpuesto < 0)
            errores.Add("El porcentaje de impuesto no puede ser negativo.");

        if (request.Lineas is null || request.Lineas.Count == 0)
        {
            errores.Add("La factura debe tener al menos una línea.");
            return errores;
        }

        foreach (var linea in request.Lineas)
        {
            if (string.IsNullOrWhiteSpace(linea.Descripcion))
                errores.Add("La descripción de la línea es obligatoria.");

            if (linea.Cantidad <= 0)
                errores.Add("La cantidad debe ser mayor que 0.");

            if (linea.PrecioUnitario <= 0)
                errores.Add("El precio unitario debe ser mayor que 0.");

            if (linea.PorcentajeImpuesto < 0)
                errores.Add("El porcentaje de impuesto de la línea no puede ser negativo.");
        }

        return errores;
    }
}