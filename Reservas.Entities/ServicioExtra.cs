using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class ServicioExtra
{
    public int ServicioId { get; set; }

    public int SedeId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public virtual Sede Sede { get; set; } = null!;
}
