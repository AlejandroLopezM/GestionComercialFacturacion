using GestionComercialFacturacion.Api.DTOs.Activities;
using GestionComercialFacturacion.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComercialFacturacion.Api.Controllers;

[ApiController]
[Route("api/opportunities/{opportunityId:int}/activities")]
public class ActivitiesController : ControllerBase
{
    private readonly IActividadService _actividadService;

    public ActivitiesController(IActividadService actividadService)
    {
        _actividadService = actividadService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActividadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActividadResponse>> Crear(
        [FromRoute] int opportunityId,
        [FromBody] CrearActividadRequest request)
    {
        var actividad = await _actividadService.CrearAsync(opportunityId, request);

        return CreatedAtAction(
            nameof(ObtenerPorOportunidad),
            new { opportunityId },
            actividad);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ActividadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ActividadResponse>>> ObtenerPorOportunidad(
        [FromRoute] int opportunityId)
    {
        var actividades = await _actividadService.ObtenerPorOportunidadAsync(opportunityId);

        return Ok(actividades);
    }
}