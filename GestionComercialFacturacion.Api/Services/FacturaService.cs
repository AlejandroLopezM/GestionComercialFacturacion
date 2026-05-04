using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Invoices;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Enums;
using GestionComercialFacturacion.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GestionComercialFacturacion.Api.Services;

public class FacturaService : IFacturaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FacturaService> _logger;

    public FacturaService(AppDbContext context, ILogger<FacturaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FacturaResponse> CrearDesdeOportunidadAsync(int oportunidadId, CrearFacturaRequest request)
    {
        ValidarCrearFactura(request);

        var oportunidad = await _context.Oportunidades
            .Include(x => x.Cliente)
            .Include(x => x.Factura)
            .FirstOrDefaultAsync(x => x.Id == oportunidadId);

        if (oportunidad is null)
        {
            throw new NotFoundAppException(
                "OpportunityNotFound",
                "No se ha encontrado la oportunidad solicitada.");
        }

        if (oportunidad.Estado != EstadoOportunidad.Ganada)
        {
            throw new ConflictAppException(
                "OpportunityNotWon",
                "Solo se puede generar una factura para una oportunidad ganada.");
        }

        if (oportunidad.Factura is not null)
        {
            throw new ConflictAppException(
                "InvoiceAlreadyExists",
                "La oportunidad ya tiene una factura asociada.");
        }

        var factura = new Factura
        {
            NumeroFactura = await GenerarNumeroFacturaAsync(request.FechaEmision),
            ClienteId = oportunidad.ClienteId,
            OportunidadId = oportunidad.Id,
            FechaEmision = request.FechaEmision.Date,
            FechaVencimiento = request.FechaVencimiento.Date,
            PorcentajeImpuesto = request.PorcentajeImpuesto,
            Estado = EstadoFactura.Emitida,
            FechaCreacion = DateTime.UtcNow
        };

        foreach (var lineaRequest in request.Lineas)
        {
            var subtotalLinea = Math.Round(lineaRequest.Cantidad * lineaRequest.PrecioUnitario, 2);
            var valorImpuestoLinea = Math.Round(subtotalLinea * lineaRequest.PorcentajeImpuesto / 100, 2);
            var totalLinea = subtotalLinea + valorImpuestoLinea;

            factura.Lineas.Add(new LineaFactura
            {
                Descripcion = lineaRequest.Descripcion.Trim(),
                Cantidad = lineaRequest.Cantidad,
                PrecioUnitario = lineaRequest.PrecioUnitario,
                PorcentajeImpuesto = lineaRequest.PorcentajeImpuesto,
                SubtotalLinea = subtotalLinea,
                ValorImpuestoLinea = valorImpuestoLinea,
                TotalLinea = totalLinea
            });
        }

        factura.Subtotal = factura.Lineas.Sum(x => x.SubtotalLinea);
        factura.ValorImpuesto = factura.Lineas.Sum(x => x.ValorImpuestoLinea);
        factura.Total = factura.Lineas.Sum(x => x.TotalLinea);

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Factura creada correctamente. FacturaId: {FacturaId}, NumeroFactura: {NumeroFactura}",
            factura.Id,
            factura.NumeroFactura);

        factura.Cliente = oportunidad.Cliente;
        factura.Oportunidad = oportunidad;

        return MapearFactura(factura);
    }

    public async Task<IReadOnlyList<FacturaResponse>> ObtenerAsync(int? customerId, string? status)
    {
        IQueryable<Factura> query = _context.Facturas
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Lineas);

        if (customerId.HasValue)
        {
            query = query.Where(x => x.ClienteId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var estado = ConvertirEstado(status);
            query = query.Where(x => x.Estado == estado);
        }

        return await query
            .OrderByDescending(x => x.FechaCreacion)
            .Select(x => MapearFactura(x))
            .ToListAsync();
    }

    public async Task<FacturaResponse> ObtenerPorIdAsync(int id)
    {
        var factura = await _context.Facturas
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Oportunidad)
            .Include(x => x.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (factura is null)
        {
            throw new NotFoundAppException(
                "InvoiceNotFound",
                "No se ha encontrado la factura solicitada.");
        }

        return MapearFactura(factura);
    }

    public async Task<FacturaResponse> CambiarEstadoAsync(int id, CambiarEstadoFacturaRequest request)
    {
        var nuevoEstado = ConvertirEstado(request.Estado);

        var factura = await _context.Facturas
            .Include(x => x.Cliente)
            .Include(x => x.Lineas)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (factura is null)
        {
            throw new NotFoundAppException(
                "InvoiceNotFound",
                "No se ha encontrado la factura solicitada.");
        }

        if (factura.Estado == EstadoFactura.Cancelada && nuevoEstado == EstadoFactura.Pagada)
        {
            throw new ConflictAppException(
                "CancelledInvoiceCannotBePaid",
                "Una factura cancelada no puede pasar a pagada.");
        }

        if (factura.Estado == EstadoFactura.Pagada && nuevoEstado != EstadoFactura.Pagada)
        {
            throw new ConflictAppException(
                "PaidInvoiceCannotBeModified",
                "Una factura pagada no debería poder modificarse.");
        }

        factura.Estado = nuevoEstado;
        factura.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Estado de factura actualizado. FacturaId: {FacturaId}, Estado: {Estado}",
            factura.Id,
            factura.Estado);

        return MapearFactura(factura);
    }

    private async Task<string> GenerarNumeroFacturaAsync(DateTime fechaEmision)
    {
        var anio = fechaEmision.Year;

        var cantidadFacturasAnio = await _context.Facturas
            .CountAsync(x => x.FechaEmision.Year == anio);

        var consecutivo = cantidadFacturasAnio + 1;

        return $"FAC-{anio}-{consecutivo:0000}";
    }

    private static void ValidarCrearFactura(CrearFacturaRequest request)
    {
        var errores = new List<string>();

        if (request.FechaEmision == default)
        {
            errores.Add("La fecha de emisión es obligatoria.");
        }

        if (request.FechaVencimiento == default)
        {
            errores.Add("La fecha de vencimiento es obligatoria.");
        }

        if (request.FechaVencimiento.Date < request.FechaEmision.Date)
        {
            errores.Add("La fecha de vencimiento no puede ser anterior a la fecha de emisión.");
        }

        if (request.PorcentajeImpuesto < 0)
        {
            errores.Add("El porcentaje de impuesto no puede ser negativo.");
        }

        if (request.Lineas is null || request.Lineas.Count == 0)
        {
            errores.Add("La factura debe tener al menos una línea.");
        }
        else
        {
            foreach (var linea in request.Lineas)
            {
                if (string.IsNullOrWhiteSpace(linea.Descripcion))
                {
                    errores.Add("La descripción de la línea es obligatoria.");
                }

                if (linea.Cantidad <= 0)
                {
                    errores.Add("La cantidad debe ser mayor que 0.");
                }

                if (linea.PrecioUnitario <= 0)
                {
                    errores.Add("El precio unitario debe ser mayor que 0.");
                }

                if (linea.PorcentajeImpuesto < 0)
                {
                    errores.Add("El porcentaje de impuesto de la línea no puede ser negativo.");
                }
            }
        }

        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }
    }

    private static EstadoFactura ConvertirEstado(string estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            throw new ValidationAppException(new[]
            {
                "El estado de la factura es obligatorio."
            });
        }

        return estado.Trim().ToLowerInvariant() switch
        {
            "draft" or "borrador" => EstadoFactura.Borrador,
            "issued" or "emitida" => EstadoFactura.Emitida,
            "paid" or "pagada" => EstadoFactura.Pagada,
            "cancelled" or "canceled" or "cancelada" => EstadoFactura.Cancelada,
            _ => throw new ValidationAppException(new[]
            {
                "El estado de la factura no es válido. Valores permitidos: Draft, Issued, Paid, Cancelled."
            })
        };
    }

    private static string ConvertirEstadoRespuesta(EstadoFactura estado)
    {
        return estado switch
        {
            EstadoFactura.Borrador => "Draft",
            EstadoFactura.Emitida => "Issued",
            EstadoFactura.Pagada => "Paid",
            EstadoFactura.Cancelada => "Cancelled",
            _ => estado.ToString()
        };
    }

    private static FacturaResponse MapearFactura(Factura factura)
    {
        return new FacturaResponse
        {
            Id = factura.Id,
            NumeroFactura = factura.NumeroFactura,
            ClienteId = factura.ClienteId,
            NombreCliente = factura.Cliente?.Nombre ?? string.Empty,
            IdentificacionFiscalCliente = factura.Cliente?.IdentificacionFiscal ?? string.Empty,
            DireccionCliente = factura.Cliente?.Direccion,
            CiudadCliente = factura.Cliente?.Ciudad,
            CodigoPostalCliente = factura.Cliente?.CodigoPostal,
            PaisCliente = factura.Cliente?.Pais,
            OportunidadId = factura.OportunidadId,
            FechaEmision = factura.FechaEmision,
            FechaVencimiento = factura.FechaVencimiento,
            Subtotal = factura.Subtotal,
            PorcentajeImpuesto = factura.PorcentajeImpuesto,
            ValorImpuesto = factura.ValorImpuesto,
            Total = factura.Total,
            Estado = ConvertirEstadoRespuesta(factura.Estado),
            FechaCreacion = factura.FechaCreacion,
            FechaActualizacion = factura.FechaActualizacion,
            Lineas = factura.Lineas.Select(x => new LineaFacturaResponse
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario,
                PorcentajeImpuesto = x.PorcentajeImpuesto,
                SubtotalLinea = x.SubtotalLinea,
                ValorImpuestoLinea = x.ValorImpuestoLinea,
                TotalLinea = x.TotalLinea
            }).ToList()
        };
    }
}