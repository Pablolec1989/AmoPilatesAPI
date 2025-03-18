using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AmoPilates.Validaciones
{
    public class HoraValidoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string hora && Regex.IsMatch(hora, @"^([01]\d|2[0-3]):([0-5]\d)$"))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("La hora ingresada no es válida. Debe estar en el formato HH:mm y ser un valor entre 00:00 y 23:59.");
        }
    }
}
