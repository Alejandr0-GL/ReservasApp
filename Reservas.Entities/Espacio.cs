using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Espacio
{
    public int EspacioId { get; set; }

    public int SedeId { get; set; }

    public string NumeroAlojamiento { get; set; } = null!;

    public int Capacidad { get; set; }

    public bool Activo { get; set; }
    public string? Descripcion { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Sede Sede { get; set; } = null!;

    public virtual ICollection<TarifaConfig> TarifaConfigs { get; set; } = new List<TarifaConfig>();

    public virtual ICollection<Caracteristica> Caracteristicas { get; set; } = new List<Caracteristica>();
}
