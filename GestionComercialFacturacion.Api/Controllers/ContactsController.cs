using GestionComercialFacturacion.Api.DTOs.Contacts;
using GestionComercialFacturacion.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComercialFacturacion.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactoService _contactoService;

    public ContactsController(IContactoService contactoService)
    {
        _contactoService = contactoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactoResponse>> Crear(
        [FromRoute] int customerId,
        [FromBody] CrearContactoRequest request)
    {
        var contacto = await _contactoService.CrearAsync(customerId, request);

        return CreatedAtAction(
            nameof(ObtenerPorCliente),
            new { customerId },
            contacto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContactoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ContactoResponse>>> ObtenerPorCliente(
        [FromRoute] int customerId)
    {
        var contactos = await _contactoService.ObtenerPorClienteAsync(customerId);

        return Ok(contactos);
    }
}