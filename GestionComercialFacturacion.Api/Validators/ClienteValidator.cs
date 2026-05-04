using System.ComponentModel.DataAnnotations;
using GestionComercialFacturacion.Api.DTOs.Customers;

namespace GestionComercialFacturacion.Api.Validators;

public static class ClienteValidator
{
    public static List<string> Validar(CrearClienteRequest request)
    {
        return ValidarDatosBasicos(
            request.Nombre,
            request.IdentificacionFiscal,
            request.CorreoElectronico);
    }

    public static List<string> Validar(ActualizarClienteRequest request)
    {
        return ValidarDatosBasicos(
            request.Nombre,
            request.IdentificacionFiscal,
            request.CorreoElectronico);
    }

    private static List<string> ValidarDatosBasicos(
        string nombre,
        string identificacionFiscal,
        string correoElectronico)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
            errores.Add("El nombre del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(identificacionFiscal))
            errores.Add("El CIF/NIF es obligatorio.");

        if (string.IsNullOrWhiteSpace(correoElectronico))
            errores.Add("El email es obligatorio.");
        else if (!new EmailAddressAttribute().IsValid(correoElectronico))
            errores.Add("El email no tiene un formato válido.");

        return errores;
    }
}