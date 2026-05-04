using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Customers;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Enums;
using GestionComercialFacturacion.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestionComercialFacturacion.Api.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(AppDbContext context, ILogger<ClienteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request)
    {
        ValidarCliente(request.Nombre, request.IdentificacionFiscal, request.CorreoElectronico);

        bool existeTaxId = await _context.Clientes
            .AnyAsync(x => x.IdentificacionFiscal == request.IdentificacionFiscal);

        if (existeTaxId)
        {
            throw new ConflictAppException(
                "DuplicatedTaxId",
                "Ya existe un cliente con el CIF/NIF indicado.");
        }

        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            IdentificacionFiscal = request.IdentificacionFiscal.Trim(),
            CorreoElectronico = request.CorreoElectronico.Trim(),
            Telefono = request.Telefono,
            Direccion = request.Direccion,
            Ciudad = request.Ciudad,
            CodigoPostal = request.CodigoPostal,
            Pais = request.Pais,
            Estado = EstadoCliente.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente creado correctamente. ClienteId: {ClienteId}", cliente.Id);

        return MapearCliente(cliente);
    }

    public async Task<IReadOnlyList<ClienteResponse>> ObtenerAsync(string? search)
    {
        IQueryable<Cliente> query = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string filtro = search.Trim();

            query = query.Where(x =>
                x.Nombre.Contains(filtro) ||
                x.IdentificacionFiscal.Contains(filtro) ||
                x.CorreoElectronico.Contains(filtro));
        }

        return await query
            .OrderBy(x => x.Nombre)
            .Select(x => MapearCliente(x))
            .ToListAsync();
    }

    public async Task<ClienteDetalleResponse> ObtenerPorIdAsync(int id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Contactos)
            .Include(x => x.Oportunidades)
            .Include(x => x.Facturas)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cliente is null)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        return MapearClienteDetalle(cliente);
    }

    public async Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request)
    {
        ValidarCliente(request.Nombre, request.IdentificacionFiscal, request.CorreoElectronico);

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cliente is null)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        bool existeTaxIdEnOtroCliente = await _context.Clientes
            .AnyAsync(x => x.Id != id && x.IdentificacionFiscal == request.IdentificacionFiscal);

        if (existeTaxIdEnOtroCliente)
        {
            throw new ConflictAppException(
                "DuplicatedTaxId",
                "Ya existe un cliente con el CIF/NIF indicado.");
        }

        cliente.Nombre = request.Nombre.Trim();
        cliente.IdentificacionFiscal = request.IdentificacionFiscal.Trim();
        cliente.CorreoElectronico = request.CorreoElectronico.Trim();
        cliente.Telefono = request.Telefono;
        cliente.Direccion = request.Direccion;
        cliente.Ciudad = request.Ciudad;
        cliente.CodigoPostal = request.CodigoPostal;
        cliente.Pais = request.Pais;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapearCliente(cliente);
    }

    public async Task DesactivarAsync(int id)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cliente is null)
        {
            throw new NotFoundAppException(
                "CustomerNotFound",
                "No se ha encontrado el cliente solicitado.");
        }

        cliente.Estado = EstadoCliente.Inactivo;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Cliente desactivado correctamente. ClienteId: {ClienteId}", cliente.Id);
    }

    private static void ValidarCliente(string nombre, string identificacionFiscal, string correoElectronico)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add("El nombre del cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(identificacionFiscal))
        {
            errores.Add("El CIF/NIF es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(correoElectronico))
        {
            errores.Add("El email es obligatorio.");
        }
        else if (!new EmailAddressAttribute().IsValid(correoElectronico))
        {
            errores.Add("El email no tiene un formato válido.");
        }

        if (errores.Count > 0)
        {
            throw new ValidationAppException(errores);
        }
    }

    private static ClienteResponse MapearCliente(Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            IdentificacionFiscal = cliente.IdentificacionFiscal,
            CorreoElectronico = cliente.CorreoElectronico,
            Telefono = cliente.Telefono,
            Direccion = cliente.Direccion,
            Ciudad = cliente.Ciudad,
            CodigoPostal = cliente.CodigoPostal,
            Pais = cliente.Pais,
            Estado = cliente.Estado,
            FechaCreacion = cliente.FechaCreacion,
            FechaActualizacion = cliente.FechaActualizacion
        };
    }

    private static ClienteDetalleResponse MapearClienteDetalle(Cliente cliente)
    {
        return new ClienteDetalleResponse
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            IdentificacionFiscal = cliente.IdentificacionFiscal,
            CorreoElectronico = cliente.CorreoElectronico,
            Telefono = cliente.Telefono,
            Direccion = cliente.Direccion,
            Ciudad = cliente.Ciudad,
            CodigoPostal = cliente.CodigoPostal,
            Pais = cliente.Pais,
            Estado = cliente.Estado,
            FechaCreacion = cliente.FechaCreacion,
            FechaActualizacion = cliente.FechaActualizacion,

            Contactos = cliente.Contactos
                .OrderByDescending(x => x.EsPrincipal)
                .ThenBy(x => x.Nombre)
                .Select(x => new ContactoClienteDetalleResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    CorreoElectronico = x.CorreoElectronico,
                    Telefono = x.Telefono,
                    Cargo = x.Cargo,
                    EsPrincipal = x.EsPrincipal
                })
                .ToList(),

            Oportunidades = cliente.Oportunidades
                .OrderByDescending(x => x.FechaCreacion)
                .Select(x => new OportunidadClienteDetalleResponse
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    MontoEstimado = x.MontoEstimado,
                    FechaEstimadaCierre = x.FechaEstimadaCierre,
                    Estado = ConvertirEstadoOportunidadRespuesta(x.Estado),
                    FechaCierre = x.FechaCierre
                })
                .ToList(),

            Facturas = cliente.Facturas
                .OrderByDescending(x => x.FechaCreacion)
                .Select(x => new FacturaClienteDetalleResponse
                {
                    Id = x.Id,
                    NumeroFactura = x.NumeroFactura,
                    OportunidadId = x.OportunidadId,
                    FechaEmision = x.FechaEmision,
                    FechaVencimiento = x.FechaVencimiento,
                    Subtotal = x.Subtotal,
                    ValorImpuesto = x.ValorImpuesto,
                    Total = x.Total,
                    Estado = ConvertirEstadoFacturaRespuesta(x.Estado)
                })
                .ToList()
        };
    }

    private static string ConvertirEstadoOportunidadRespuesta(EstadoOportunidad estado)
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

    private static string ConvertirEstadoFacturaRespuesta(EstadoFactura estado)
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
}