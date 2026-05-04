using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Opportunities;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Enums;
using GestionComercialFacturacion.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GestionComercialFacturacion.Api.Services;

public class OportunidadService : IOportunidadService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OportunidadService> _logger;

    public OportunidadService(AppDbContext context, ILogger<OportunidadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OportunidadResponse> CrearAsync(int clienteId, CrearOportunidadRequest request)
    {
        ValidarCrearOportunidad(request);

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == clienteId);

        if (cliente is null)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        if (cliente.Estado == EstadoCliente.Inactivo)
        {
            throw new ConflictAppException(
                "InactiveCustomer",
                "Un cliente inactivo no puede recibir nuevas oportunidades.");
        }

        var oportunidad = new Oportunidad
        {
            ClienteId = clienteId,
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion,
            MontoEstimado = request.MontoEstimado,
            FechaEstimadaCierre = request.FechaEstimadaCierre.Date,
            Estado = EstadoOportunidad.Abierta,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Oportunidades.Add(oportunidad);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Oportunidad creada correctamente. ClienteId: {ClienteId}, OportunidadId: {OportunidadId}",
            clienteId,
            oportunidad.Id);

        oportunidad.Cliente = cliente;

        return MapearOportunidad(oportunidad);
    }

    public async Task<IReadOnlyList<OportunidadResponse>> ObtenerPorClienteAsync(int clienteId)
    {
        bool existeCliente = await _context.Clientes
            .AsNoTracking()
            .AnyAsync(x => x.Id == clienteId);

        if (!existeCliente)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        return await _context.Oportunidades
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.FechaCreacion)
            .Select(x => MapearOportunidad(x))
            .ToListAsync();
    }

    public async Task<OportunidadResponse> ObtenerPorIdAsync(int id)
    {
        var oportunidad = await _context.Oportunidades
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Actividades)
            .Include(x => x.Factura)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (oportunidad is null)
        {
            throw new NotFoundAppException(
                "OpportunityNotFound",
                "No se ha encontrado la oportunidad solicitada.");
        }

        return MapearOportunidad(oportunidad);
    }

    public async Task<OportunidadResponse> CambiarEstadoAsync(int id, CambiarEstadoOportunidadRequest request)
    {
        var nuevoEstado = ConvertirEstado(request.Estado);

        var oportunidad = await _context.Oportunidades
            .Include(x => x.Cliente)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (oportunidad is null)
        {
            throw new NotFoundAppException(
                "OpportunityNotFound",
                "No se ha encontrado la oportunidad solicitada.");
        }

        if (EstaCerrada(oportunidad.Estado))
        {
            throw new ConflictAppException(
                "ClosedOpportunity",
                "No se puede cambiar el estado de una oportunidad ya cerrada.");
        }

        oportunidad.Estado = nuevoEstado;
        oportunidad.FechaActualizacion = DateTime.UtcNow;

        if (EstaCerrada(nuevoEstado))
        {
            oportunidad.FechaCierre = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Estado de oportunidad actualizado. OportunidadId: {OportunidadId}, Estado: {Estado}",
            oportunidad.Id,
            oportunidad.Estado);

        return MapearOportunidad(oportunidad);
    }

    private static void ValidarCrearOportunidad(CrearOportunidadRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            errores.Add("El título de la oportunidad es obligatorio.");
        }

        if (request.MontoEstimado <= 0)
        {
            errores.Add("El importe estimado debe ser mayor que 0.");
        }

        if (request.FechaEstimadaCierre.Date < DateTime.UtcNow.Date)
        {
            errores.Add("La fecha prevista de cierre no puede ser anterior a la fecha actual.");
        }

        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }
    }

    private static EstadoOportunidad ConvertirEstado(string estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
        {
            throw new ValidationAppException(new[]
            {
                "El estado de la oportunidad es obligatorio."
            });
        }

        return estado.Trim().ToLowerInvariant() switch
        {
            "open" or "abierta" => EstadoOportunidad.Abierta,
            "inprogress" or "in_progress" or "enprogreso" or "en progreso" => EstadoOportunidad.EnProgreso,
            "won" or "ganada" => EstadoOportunidad.Ganada,
            "lost" or "perdida" => EstadoOportunidad.Perdida,
            "cancelled" or "canceled" or "cancelada" => EstadoOportunidad.Cancelada,
            _ => throw new ValidationAppException(new[]
            {
                "El estado de la oportunidad no es válido. Valores permitidos: Open, InProgress, Won, Lost, Cancelled."
            })
        };
    }

    private static bool EstaCerrada(EstadoOportunidad estado)
    {
        return estado is EstadoOportunidad.Ganada
            or EstadoOportunidad.Perdida
            or EstadoOportunidad.Cancelada;
    }

    private static string ConvertirEstadoRespuesta(EstadoOportunidad estado)
    {
        return estado switch
        {
            EstadoOportunidad.Abierta => "Open",
            EstadoOportunidad.EnProgreso => "InProgress",
            EstadoOportunidad.Ganada => "Won",
            EstadoOportunidad.Perdida => "Lost",
            EstadoOportunidad.Cancelada => "Cancelled",
            _ => estado.ToString()
        };
    }

    private static OportunidadResponse MapearOportunidad(Oportunidad oportunidad)
    {
        return new OportunidadResponse
        {
            Id = oportunidad.Id,
            ClienteId = oportunidad.ClienteId,
            NombreCliente = oportunidad.Cliente?.Nombre,
            Titulo = oportunidad.Titulo,
            Descripcion = oportunidad.Descripcion,
            MontoEstimado = oportunidad.MontoEstimado,
            FechaEstimadaCierre = oportunidad.FechaEstimadaCierre,
            Estado = ConvertirEstadoRespuesta(oportunidad.Estado),
            FechaCreacion = oportunidad.FechaCreacion,
            FechaActualizacion = oportunidad.FechaActualizacion,
            FechaCierre = oportunidad.FechaCierre
        };
    }
}