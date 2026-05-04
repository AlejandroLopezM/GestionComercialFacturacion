using System.ComponentModel.DataAnnotations;
using GestionComercialFacturacion.Api.DTOs.Contacts;

namespace GestionComercialFacturacion.Api.Validators;

public static class ContactoValidator
{
    public static List<string> Validar(CrearContactoRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Nombre))
            errores.Add("El nombre del contacto es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.CorreoElectronico))
            errores.Add("El email del contacto es obligatorio.");
        else if (!new EmailAddressAttribute().IsValid(request.CorreoElectronico))
            errores.Add("El email del contacto no tiene un formato válido.");

        return errores;
    }
}