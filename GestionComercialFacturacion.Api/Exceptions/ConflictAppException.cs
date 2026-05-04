namespace GestionComercialFacturacion.Api.Exceptions;

public class ConflictAppException : ApiException
{
    public ConflictAppException(string error, string message)
        : base(StatusCodes.Status409Conflict, error, message)
    {
    }
}