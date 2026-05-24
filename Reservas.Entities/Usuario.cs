using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string NroDocumento { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? Celular { get; set; }

    public string DireccionEmail { get; set; } = null!;

    public int MunicipioId { get; set; }

    public string Barrio { get; set; } = null!;

    public string DireccionResidencia { get; set; } = null!;

    public string TelefonoResidencia { get; set; } = null!;

    public string PreguntaSecreta { get; set; } = null!;

    public string RespuestaSecreta { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public bool AutorizaCorreo { get; set; }

    public bool AutorizaCelular { get; set; }

    public bool Activo { get; set; }

    public virtual Municipio? Municipio { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
