using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Festivo
{
    public DateOnly Fecha { get; set; }

    public string Nombre { get; set; } = null!;
}
