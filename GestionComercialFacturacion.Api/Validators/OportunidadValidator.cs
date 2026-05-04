using GestionComercialFacturacion.Api.DTOs.Opportunities;

namespace GestionComercialFacturacion.Api.Validators;

public static class OportunidadValidator
{
    public static List<string> ValidarCrear(CrearOportunidadRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Titulo))
            errores.Add("El título de la oportunidad es obligatorio.");

        if (request.MontoEstimado <= 0)
            errores.Add("El importe estimado debe ser mayor que 0.");

        if (request.FechaEstimadaCierre.Date < DateTime.UtcNow.Date)
            errores.Add("La fecha prevista de cierre no puede ser anterior a la fecha actual.");

        return errores;
    }
}