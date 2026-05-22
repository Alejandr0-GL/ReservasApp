using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class TarifaConfig
{
    public int TarifaConfigId { get; set; }

    public int SedeId { get; set; }

    public int? EspacioId { get; set; }

    public int Capacidad { get; set; }

    public string TipoTemporada { get; set; } = null!;

    public decimal PrecioOrdinario { get; set; }

    public decimal PrecioEspecial { get; set; }

    public bool EsVisitaDia { get; set; }

    public int? MinAcompanantesTarifaEspecial { get; set; }

    public decimal? PrecioAcompananteAdicional { get; set; }

    public virtual Espacio? Espacio { get; set; }

    public virtual Sede Sede { get; set; } = null!;
}
