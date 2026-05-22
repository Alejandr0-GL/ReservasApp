using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Departamento
{
    public int DepartamentoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
