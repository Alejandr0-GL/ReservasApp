using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservas.Entities
{
    public class ResultadoDisponibilidad
    {
        public int EspacioId { get; set; }
        public int SedeId { get; set; }
        public string NumeroAlojamiento { get; set; }
        public int Capacidad { get; set; }
        public bool Activo { get; set; }
    }
}
