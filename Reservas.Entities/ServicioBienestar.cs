using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class ServicioBienestar
{
    public int ServicioBienestarId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Sede> Sedes { get; set; } = new List<Sede>();
}
