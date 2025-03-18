using System.ComponentModel.DataAnnotations;

namespace AmoPilates.Validaciones
{
    public class DiaValidoAttribute : ValidationAttribute
    {
        private readonly string[] diasValidos = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string dia && diasValidos.Contains(dia))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("El día ingresado no es válido. " +
                "Debe ser uno de los siguientes: Lunes, Martes, Miercoles, Jueves, Viernes, Sabado (Primera letra en mayúscula)");
        }
    }
}
