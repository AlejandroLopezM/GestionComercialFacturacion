using GestionComercialFacturacion.Api.DTOs.Customers;
using GestionComercialFacturacion.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComercialFacturacion.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public CustomersController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Crear([FromBody] CrearClienteRequest request)
    {
        var cliente = await _clienteService.CrearAsync(request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = cliente.Id },
            cliente);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> Obtener([FromQuery] string? search)
    {
        var clientes = await _clienteService.ObtenerAsync(search);

        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDetalleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDetalleResponse>> ObtenerPorId([FromRoute] int id)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id);

        return Ok(cliente);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Actualizar(
        [FromRoute] int id,
        [FromBody] ActualizarClienteRequest request)
    {
        var cliente = await _clienteService.ActualizarAsync(id, request);

        return Ok(cliente);
    }

    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desactivar([FromRoute] int id)
    {
        await _clienteService.DesactivarAsync(id);

        return NoContent();
    }
}