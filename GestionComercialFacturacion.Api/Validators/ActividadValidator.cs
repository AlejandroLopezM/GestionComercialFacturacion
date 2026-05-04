using GestionComercialFacturacion.Api.DTOs.Activities;

namespace GestionComercialFacturacion.Api.Validators;

public static class ActividadValidator
{
    public static List<string> Validar(CrearActividadRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Tipo))
            errores.Add("El tipo de actividad es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Descripcion))
            errores.Add("La descripción de la actividad es obligatoria.");

        if (request.FechaActividad.Date > DateTime.UtcNow.Date)
            errores.Add("La fecha de actividad no puede ser futura.");

        return errores;
    }
}