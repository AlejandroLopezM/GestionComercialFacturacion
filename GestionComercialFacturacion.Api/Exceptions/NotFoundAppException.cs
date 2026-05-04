namespace GestionComercialFacturacion.Api.Exceptions;

public class NotFoundAppException : ApiException
{
    public NotFoundAppException(string error, string message)
        : base(StatusCodes.Status404NotFound, error, message)
    {
    }
}