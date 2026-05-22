using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Sede
{
    public int SedeId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public string TipoSede { get; set; } = null!;

    public string DescripcionCorta { get; set; } = null!;

    public string DescripcionLarga { get; set; } = null!;

    public string? RutaMapaUrl { get; set; }

    public string? ImagenPrincipalUrl { get; set; }

    public virtual ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<SedeImagen> SedeImagens { get; set; } = new List<SedeImagen>();

    public virtual ICollection<ServicioExtra> ServicioExtras { get; set; } = new List<ServicioExtra>();

    public virtual ICollection<TarifaConfig> TarifaConfigs { get; set; } = new List<TarifaConfig>();

    public virtual ICollection<ServicioBienestar> ServicioBienestars { get; set; } = new List<ServicioBienestar>();
}
