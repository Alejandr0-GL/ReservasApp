using Reservas.Entities;
using System.Collections.Generic;

namespace Reservas.Web.Models
{
    public class SelectLocationViewModel
    {
        public Sede? Sede { get; set; }

        public List<Espacio> Espacios { get; set; } = new List<Espacio>();

        public List<TarifaConfig> TarifaConfigs { get; set; } = new List<TarifaConfig>();

        public List<ServicioExtra> ServicioExtras { get; set; } = new List<ServicioExtra>();
    }
}