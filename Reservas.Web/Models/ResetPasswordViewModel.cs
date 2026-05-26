using System.ComponentModel.DataAnnotations;

namespace Reservas.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "La clave debe tener 4 dígitos.")]
        public string NuevaClave { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NuevaClave), ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmarClave { get; set; } = string.Empty;
    }
}