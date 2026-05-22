using System;
using System.Collections.Generic;

namespace Reservas.Entities;

public partial class Municipio
{
    public int MunicipioId { get; set; }

    public int DepartamentoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual Departamento Departamento { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
