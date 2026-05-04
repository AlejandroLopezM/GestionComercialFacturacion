using GestionComercialFacturacion.Api.DTOs.Opportunities;

namespace GestionComercialFacturacion.Api.Services;

public interface IOportunidadService
{
    Task<OportunidadResponse> CrearAsync(int clienteId, CrearOportunidadRequest request);
    Task<IReadOnlyList<OportunidadResponse>> ObtenerPorClienteAsync(int clienteId);
    Task<OportunidadResponse> ObtenerPorIdAsync(int id);
    Task<OportunidadResponse> CambiarEstadoAsync(int id, CambiarEstadoOportunidadRequest request);
}