using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Contacts;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestionComercialFacturacion.Api.Services;

public class ContactoService : IContactoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ContactoService> _logger;

    public ContactoService(AppDbContext context, ILogger<ContactoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ContactoResponse> CrearAsync(int clienteId, CrearContactoRequest request)
    {
        ValidarContacto(request);

        bool existeCliente = await _context.Clientes
            .AnyAsync(x => x.Id == clienteId);

        if (!existeCliente)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        if (request.EsPrincipal)
        {
            var contactoPrincipalActual = await _context.Contactos
                .FirstOrDefaultAsync(x => x.ClienteId == clienteId && x.EsPrincipal);

            if (contactoPrincipalActual is not null)
            {
                contactoPrincipalActual.EsPrincipal = false;
                contactoPrincipalActual.FechaActualizacion = DateTime.UtcNow;
            }
        }

        var contacto = new Contacto
        {
            ClienteId = clienteId,
            Nombre = request.Nombre.Trim(),
            CorreoElectronico = request.CorreoElectronico.Trim(),
            Telefono = request.Telefono,
            Cargo = request.Cargo,
            EsPrincipal = request.EsPrincipal,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Contactos.Add(contacto);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Contacto creado correctamente. ClienteId: {ClienteId}, ContactoId: {ContactoId}",
            clienteId,
            contacto.Id);

        return MapearContacto(contacto);
    }

    public async Task<IReadOnlyList<ContactoResponse>> ObtenerPorClienteAsync(int clienteId)
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

        return await _context.Contactos
            .AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.EsPrincipal)
            .ThenBy(x => x.Nombre)
            .Select(x => MapearContacto(x))
            .ToListAsync();
    }

    private static void ValidarContacto(CrearContactoRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            errores.Add("El nombre del contacto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.CorreoElectronico))
        {
            errores.Add("El email del contacto es obligatorio.");
        }
        else if (!new EmailAddressAttribute().IsValid(request.CorreoElectronico))
        {
            errores.Add("El email del contacto no tiene un formato válido.");
        }

        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }
    }

    private static ContactoResponse MapearContacto(Contacto contacto)
    {
        return new ContactoResponse
        {
            Id = contacto.Id,
            ClienteId = contacto.ClienteId,
            Nombre = contacto.Nombre,
            CorreoElectronico = contacto.CorreoElectronico,
            Telefono = contacto.Telefono,
            Cargo = contacto.Cargo,
            EsPrincipal = contacto.EsPrincipal,
            FechaCreacion = contacto.FechaCreacion,
            FechaActualizacion = contacto.FechaActualizacion
        };
    }
}