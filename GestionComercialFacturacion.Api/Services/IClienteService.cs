using GestionComercialFacturacion.Api.DTOs.Customers;

namespace GestionComercialFacturacion.Api.Services;

public interface IClienteService
{
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request);
    Task<IReadOnlyList<ClienteResponse>> ObtenerAsync(string? search);
    Task<ClienteDetalleResponse> ObtenerPorIdAsync(int id);
    Task<ClienteResponse> ActualizarAsync(int id, ActualizarClienteRequest request);
    Task DesactivarAsync(int id);
}