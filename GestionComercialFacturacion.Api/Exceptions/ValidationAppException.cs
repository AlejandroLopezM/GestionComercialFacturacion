namespace GestionComercialFacturacion.Api.Exceptions;

public class ValidationAppException : ApiException
{
    public ValidationAppException(IReadOnlyList<string> details)
        : base(
            StatusCodes.Status400BadRequest,
            "ValidationError",
            "Los datos enviados no son válidos.",
            details)
    {
    }
}