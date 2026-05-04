using GestionComercialFacturacion.Api.DTOs.Invoices;
using GestionComercialFacturacion.Api.Pdf;
using GestionComercialFacturacion.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComercialFacturacion.Api.Controllers;

[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IFacturaService _facturaService;
    private readonly IFacturaPdfService _facturaPdfService;

    public InvoicesController(
        IFacturaService facturaService,
        IFacturaPdfService facturaPdfService)
    {
        _facturaService = facturaService;
        _facturaPdfService = facturaPdfService;
    }

    [HttpPost("api/opportunities/{id:int}/invoice")]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaResponse>> CrearDesdeOportunidad(
        [FromRoute] int id,
        [FromBody] CrearFacturaRequest request)
    {
        var factura = await _facturaService.CrearDesdeOportunidadAsync(id, request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = factura.Id },
            factura);
    }

    [HttpGet("api/invoices")]
    [ProducesResponseType(typeof(IReadOnlyList<FacturaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FacturaResponse>>> Obtener(
        [FromQuery] int? customerId,
        [FromQuery] string? status)
    {
        var facturas = await _facturaService.ObtenerAsync(customerId, status);

        return Ok(facturas);
    }

    [HttpGet("api/invoices/{id:int}")]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FacturaResponse>> ObtenerPorId([FromRoute] int id)
    {
        var factura = await _facturaService.ObtenerPorIdAsync(id);

        return Ok(factura);
    }

    [HttpGet("api/invoices/{id:int}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DescargarPdf([FromRoute] int id)
    {
        var factura = await _facturaService.ObtenerPorIdAsync(id);

        var pdfBytes = _facturaPdfService.Generar(factura);

        return File(
            pdfBytes,
            "application/pdf",
            $"{factura.NumeroFactura}.pdf");
    }

    [HttpPatch("api/invoices/{id:int}/status")]
    [ProducesResponseType(typeof(FacturaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaResponse>> CambiarEstado(
        [FromRoute] int id,
        [FromBody] CambiarEstadoFacturaRequest request)
    {
        var factura = await _facturaService.CambiarEstadoAsync(id, request);

        return Ok(factura);
    }
}