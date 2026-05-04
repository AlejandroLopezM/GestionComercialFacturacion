using GestionComercialFacturacion.Api.DTOs.Activities;

namespace GestionComercialFacturacion.Api.Services;

public interface IActividadService
{
    Task<ActividadResponse> CrearAsync(int oportunidadId, CrearActividadRequest request);
    Task<IReadOnlyList<ActividadResponse>> ObtenerPorOportunidadAsync(int oportunidadId);
}