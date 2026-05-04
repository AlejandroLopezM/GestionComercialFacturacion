using GestionComercialFacturacion.Api.Data;
using GestionComercialFacturacion.Api.DTOs.Customers;
using GestionComercialFacturacion.Api.DTOs.Invoices;
using GestionComercialFacturacion.Api.Entities;
using GestionComercialFacturacion.Api.Enums;
using GestionComercialFacturacion.Api.Exceptions;
using GestionComercialFacturacion.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace GestionComercialFacturacion.Tests;

public class PruebaTecnicaTests
{
    [Fact]
    public async Task CrearClienteAsync_DatosValidos_CreaClienteActivo()
    {
        var context = CrearContexto();
        var service = new ClienteService(context, NullLogger<ClienteService>.Instance);

        var request = new CrearClienteRequest
        {
            Nombre = "Acme S.L.",
            IdentificacionFiscal = "B12345678",
            CorreoElectronico = "info@acme.com",
            Telefono = "941000000",
            Direccion = "Calle Mayor 1",
            Ciudad = "Logroño",
            CodigoPostal = "26001",
            Pais = "España"
        };

        var resultado = await service.CrearAsync(request);

        Assert.Equal("Acme S.L.", resultado.Nombre);
        Assert.Equal("B12345678", resultado.IdentificacionFiscal);
        Assert.Equal(EstadoCliente.Activo, resultado.Estado);
        Assert.Equal(1, await context.Clientes.CountAsync());
    }

    [Fact]
    public async Task CrearClienteAsync_TaxIdDuplicado_LanzaConflict()
    {
        var context = CrearContexto();
        var service = new ClienteService(context, NullLogger<ClienteService>.Instance);

        var request = new CrearClienteRequest
        {
            Nombre = "Acme S.L.",
            IdentificacionFiscal = "B12345678",
            CorreoElectronico = "info@acme.com"
        };

        await service.CrearAsync(request);

        var duplicado = new CrearClienteRequest
        {
            Nombre = "Otra empresa",
            IdentificacionFiscal = "B12345678",
            CorreoElectronico = "otra@empresa.com"
        };

        var exception = await Assert.ThrowsAsync<ConflictAppException>(
            () => service.CrearAsync(duplicado));

        Assert.Equal("DuplicatedTaxId", exception.Error);
    }

    [Fact]
    public async Task CrearFacturaDesdeOportunidadGanada_CalculaTotalesCorrectamente()
    {
        var context = CrearContexto();

        var cliente = new Cliente
        {
            Nombre = "Acme S.L.",
            IdentificacionFiscal = "B12345678",
            CorreoElectronico = "info@acme.com",
            Estado = EstadoCliente.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        var oportunidad = new Oportunidad
        {
            Cliente = cliente,
            Titulo = "Implantación nuevo sistema de reporting",
            Descripcion = "Proyecto para mejorar el reporting financiero.",
            MontoEstimado = 15000,
            FechaEstimadaCierre = DateTime.UtcNow.AddDays(30),
            Estado = EstadoOportunidad.Ganada,
            FechaCreacion = DateTime.UtcNow,
            FechaCierre = DateTime.UtcNow
        };

        context.Clientes.Add(cliente);
        context.Oportunidades.Add(oportunidad);
        await context.SaveChangesAsync();

        var service = new FacturaService(context, NullLogger<FacturaService>.Instance);

        var request = new CrearFacturaRequest
        {
            FechaEmision = new DateTime(2026, 4, 30),
            FechaVencimiento = new DateTime(2026, 5, 30),
            PorcentajeImpuesto = 21,
            Lineas = new List<CrearLineaFacturaRequest>
            {
                new()
                {
                    Descripcion = "Implantación nuevo sistema de reporting",
                    Cantidad = 1,
                    PrecioUnitario = 15000,
                    PorcentajeImpuesto = 21
                }
            }
        };

        var factura = await service.CrearDesdeOportunidadAsync(oportunidad.Id, request);

        Assert.Equal("FAC-2026-0001", factura.NumeroFactura);
        Assert.Equal(15000, factura.Subtotal);
        Assert.Equal(3150, factura.ValorImpuesto);
        Assert.Equal(18150, factura.Total);
        Assert.Equal("Issued", factura.Estado);
    }

    [Fact]
    public async Task CrearFacturaDesdeMismaOportunidadDosVeces_LanzaConflict()
    {
        var context = CrearContexto();

        var cliente = new Cliente
        {
            Nombre = "Acme S.L.",
            IdentificacionFiscal = "B12345678",
            CorreoElectronico = "info@acme.com",
            Estado = EstadoCliente.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        var oportunidad = new Oportunidad
        {
            Cliente = cliente,
            Titulo = "Implantación nuevo sistema de reporting",
            MontoEstimado = 15000,
            FechaEstimadaCierre = DateTime.UtcNow.AddDays(30),
            Estado = EstadoOportunidad.Ganada,
            FechaCreacion = DateTime.UtcNow,
            FechaCierre = DateTime.UtcNow
        };

        context.Clientes.Add(cliente);
        context.Oportunidades.Add(oportunidad);
        await context.SaveChangesAsync();

        var service = new FacturaService(context, NullLogger<FacturaService>.Instance);

        var request = new CrearFacturaRequest
        {
            FechaEmision = new DateTime(2026, 4, 30),
            FechaVencimiento = new DateTime(2026, 5, 30),
            PorcentajeImpuesto = 21,
            Lineas = new List<CrearLineaFacturaRequest>
            {
                new()
                {
                    Descripcion = "Servicio de implementación",
                    Cantidad = 1,
                    PrecioUnitario = 15000,
                    PorcentajeImpuesto = 21
                }
            }
        };

        await service.CrearDesdeOportunidadAsync(oportunidad.Id, request);

        var exception = await Assert.ThrowsAsync<ConflictAppException>(
            () => service.CrearDesdeOportunidadAsync(oportunidad.Id, request));

        Assert.Equal("InvoiceAlreadyExists", exception.Error);
    }

    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}