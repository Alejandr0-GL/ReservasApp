using System;

namespace Reservas.Web.Models
{
    public class ReservaSolicitudViewModel
    {
        public int SedeId { get; set; }

        public int? EspacioId { get; set; }

        public string TipoReserva { get; set; } = "Hospedaje";

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int Personas { get; set; }

        public bool IncluyeLavanderia { get; set; }
    }
}