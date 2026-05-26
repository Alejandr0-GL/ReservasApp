using System;
using System.ComponentModel.DataAnnotations;

namespace Reservas.Web.Models
{
    public class ActualizarUsuarioViewModel
    {
        public int UsuarioId { get; set; }

        [Required]
        public string NroDocumento { get; set; } = string.Empty;

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public string? Celular { get; set; }

        [Required]
        [EmailAddress]
        public string DireccionEmail { get; set; } = string.Empty;

        [Required]
        public int DepartamentoId { get; set; }

        [Required]
        public int MunicipioId { get; set; }

        [Required]
        public string Barrio { get; set; } = string.Empty;

        [Required]
        public string DireccionResidencia { get; set; } = string.Empty;

        [Required]
        public string TelefonoResidencia { get; set; } = string.Empty;

        public bool AutorizaCorreo { get; set; }

        public bool AutorizaCelular { get; set; }
    }
}