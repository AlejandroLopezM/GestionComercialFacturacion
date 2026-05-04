using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Activities;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Enums;
using GestionComercialFacturacion.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GestionComercialFacturacion.Api.Services;

public class ActividadService : IActividadService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ActividadService> _logger;

    public ActividadService(AppDbContext context, ILogger<ActividadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ActividadResponse> CrearAsync(int oportunidadId, CrearActividadRequest request)
    {
        ValidarActividad(request);

        var oportunidadExiste = await _context.Oportunidades
            .AnyAsync(x => x.Id == oportunidadId);

        if (!oportunidadExiste)
        {
            throw new NotFoundAppException(
                "OpportunityNotFound",
                "No se ha encontrado la oportunidad solicitada.");
        }

        var actividad = new ActividadComercial
        {
            OportunidadId = oportunidadId,
            Tipo = ConvertirTipo(request.Tipo),
            Descripcion = request.Descripcion.Trim(),
            FechaActividad = request.FechaActividad.Date,
            FechaCreacion = DateTime.UtcNow
        };

        _context.ActividadesComerciales.Add(actividad);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Actividad comercial registrada. OportunidadId: {OportunidadId}, ActividadId: {ActividadId}",
            oportunidadId,
            actividad.Id);

        return MapearActividad(actividad);
    }

    public async Task<IReadOnlyList<ActividadResponse>> ObtenerPorOportunidadAsync(int oportunidadId)
    {
        var oportunidadExiste = await _context.Oportunidades
            .AsNoTracking()
            .AnyAsync(x => x.Id == oportunidadId);

        if (!oportunidadExiste)
        {
            throw new NotFoundAppException(
                "OpportunityNotFound",
                "No se ha encontrado la oportunidad solicitada.");
        }

        return await _context.ActividadesComerciales
            .AsNoTracking()
            .Where(x => x.OportunidadId == oportunidadId)
            .OrderByDescending(x => x.FechaActividad)
            .Select(x => MapearActividad(x))
            .ToListAsync();
    }

    private static void ValidarActividad(CrearActividadRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Tipo))
        {
            errores.Add("El tipo de actividad es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Descripcion))
        {
            errores.Add("La descripción de la actividad es obligatoria.");
        }

        if (request.FechaActividad.Date > DateTime.UtcNow.Date)
        {
            errores.Add("La fecha de actividad no puede ser futura.");
        }

        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }
    }

    private static TipoActividadComercial ConvertirTipo(string tipo)
    {
        return tipo.Trim().ToLowerInvariant() switch
        {
            "call" or "llamada" => TipoActividadComercial.Llamada,
            "email" or "correo" => TipoActividadComercial.Correo,
            "meeting" or "reunion" or "reunión" => TipoActividadComercial.Reunion,
            "note" or "nota" => TipoActividadComercial.Nota,
            "task" or "tarea" => TipoActividadComercial.Tarea,
            _ => throw new ValidationAppException(new[]
            {
                "El tipo de actividad no es válido. Valores permitidos: Call, Email, Meeting, Note, Task."
            })
        };
    }

    private static string ConvertirTipoRespuesta(TipoActividadComercial tipo)
    {
        return tipo switch
        {
            TipoActividadComercial.Llamada => "Call",
            TipoActividadComercial.Correo => "Email",
            TipoActividadComercial.Reunion => "Meeting",
            TipoActividadComercial.Nota => "Note",
            TipoActividadComercial.Tarea => "Task",
            _ => tipo.ToString()
        };
    }

    private static ActividadResponse MapearActividad(ActividadComercial actividad)
    {
        return new ActividadResponse
        {
            Id = actividad.Id,
            OportunidadId = actividad.OportunidadId,
            Tipo = ConvertirTipoRespuesta(actividad.Tipo),
            Descripcion = actividad.Descripcion,
            FechaActividad = actividad.FechaActividad,
            FechaCreacion = actividad.FechaCreacion
        };
    }
}