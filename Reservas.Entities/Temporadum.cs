using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Temporadum
{
    public int TemporadaId { get; set; }

    public string Tipo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }
}
