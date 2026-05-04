using GestionComercialFacturacion.Api.DTOs.Opportunities;
using GestionComercialFacturacion.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComercialFacturacion.Api.Controllers;

[ApiController]
public class OpportunitiesController : ControllerBase
{
    private readonly IOportunidadService _oportunidadService;

    public OpportunitiesController(IOportunidadService oportunidadService)
    {
        _oportunidadService = oportunidadService;
    }

    [HttpPost("api/customers/{customerId:int}/opportunities")]
    [ProducesResponseType(typeof(OportunidadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OportunidadResponse>> Crear(
        [FromRoute] int customerId,
        [FromBody] CrearOportunidadRequest request)
    {
        var oportunidad = await _oportunidadService.CrearAsync(customerId, request);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = oportunidad.Id },
            oportunidad);
    }

    [HttpGet("api/customers/{customerId:int}/opportunities")]
    [ProducesResponseType(typeof(IReadOnlyList<OportunidadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<OportunidadResponse>>> ObtenerPorCliente(
        [FromRoute] int customerId)
    {
        var oportunidades = await _oportunidadService.ObtenerPorClienteAsync(customerId);

        return Ok(oportunidades);
    }

    [HttpGet("api/opportunities/{id:int}", Name = "GetOpportunityById")]
    [ProducesResponseType(typeof(OportunidadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OportunidadResponse>> ObtenerPorId([FromRoute] int id)
    {
        var oportunidad = await _oportunidadService.ObtenerPorIdAsync(id);

        return Ok(oportunidad);
    }

    [HttpPatch("api/opportunities/{id:int}/status")]
    [ProducesResponseType(typeof(OportunidadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OportunidadResponse>> CambiarEstado(
        [FromRoute] int id,
        [FromBody] CambiarEstadoOportunidadRequest request)
    {
        var oportunidad = await _oportunidadService.CambiarEstadoAsync(id, request);

        return Ok(oportunidad);
    }
}