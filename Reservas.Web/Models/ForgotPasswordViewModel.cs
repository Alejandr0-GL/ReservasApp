using System.ComponentModel.DataAnnotations;

namespace Reservas.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string NroDocumento { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string DireccionEmail { get; set; } = string.Empty;
    }
}