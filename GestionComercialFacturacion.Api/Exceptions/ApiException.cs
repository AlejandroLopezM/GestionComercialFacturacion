namespace GestionComercialFacturacion.Api.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string Error { get; }
    public IReadOnlyList<string> Details { get; }

    public ApiException(
        int statusCode,
        string error,
        string message,
        IReadOnlyList<string>? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Error = error;
        Details = details ?? Array.Empty<string>();
    }
}