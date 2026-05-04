using GestionComercialFacturacion.Api.DTOs.Invoices;

namespace GestionComercialFacturacion.Api.Services;

public interface IFacturaService
{
    Task<FacturaResponse> CrearDesdeOportunidadAsync(int oportunidadId, CrearFacturaRequest request);
    Task<IReadOnlyList<FacturaResponse>> ObtenerAsync(int? customerId, string? status);
    Task<FacturaResponse> ObtenerPorIdAsync(int id);
    Task<FacturaResponse> CambiarEstadoAsync(int id, CambiarEstadoFacturaRequest request);
}