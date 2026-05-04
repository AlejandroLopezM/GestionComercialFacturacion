using GestionComercialFacturacion.Api.DTOs.Contacts;

namespace GestionComercialFacturacion.Api.Services;

public interface IContactoService
{
    Task<ContactoResponse> CrearAsync(int clienteId, CrearContactoRequest request);
    Task<IReadOnlyList<ContactoResponse>> ObtenerPorClienteAsync(int clienteId);
}