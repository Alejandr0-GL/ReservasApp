using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Reserva
{
    public int ReservaId { get; set; }

    public int UsuarioId { get; set; }

    public int SedeId { get; set; }

    public int? EspacioId { get; set; }

    public string TipoReserva { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int Personas { get; set; }

    public bool IncluyeLavanderia { get; set; }

    public decimal ValorTotal { get; set; }

    public string Estado { get; set; } = null!;

    public string? ComprobantePagoUrl { get; set; }

    public DateTime CreadoEn { get; set; }

    public virtual Espacio? Espacio { get; set; }

    public virtual Sede Sede { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
