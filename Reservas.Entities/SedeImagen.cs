using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class SedeImagen
{
    public int SedeImagenId { get; set; }

    public int SedeId { get; set; }

    public string ImagenUrl { get; set; } = null!;

    public virtual Sede Sede { get; set; } = null!;
}
