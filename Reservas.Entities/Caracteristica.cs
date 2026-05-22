using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Caracteristica
{
    public int CaracteristicaId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();
}
