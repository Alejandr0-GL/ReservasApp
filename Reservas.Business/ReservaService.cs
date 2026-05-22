using Reservas.DataAccess;
using Reservas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservas.Business
{
    public class ReservaService
    {
        private readonly FondoDbContext _context;

        // Inyección directa del DbContext en la capa de Negocio
        public ReservaService(FondoDbContext context)
        {
            _context = context;
        }

        // 1. Consultar disponibilidad usando el SP 1 mapeado en el DbContext
        public async Task<List<ResultadoDisponibilidad>> ConsultarDisponibilidadAsync(int sedeId, DateTime inicio, DateTime fin)
        {
            return await _context.ObtenerHabitacionesDisponiblesAsync(sedeId, inicio, fin);
        }

        // 2. Consultar disponibilidad por cupo usando el SP 2 mapeado en el DbContext
        public async Task<List<ResultadoDisponibilidad>> ConsultarDisponibilidadPorCupoAsync(int sedeId, DateTime inicio, DateTime fin, int personas)
        {
            return await _context.ObtenerHabitacionesDisponiblesPorCupoAsync(sedeId, inicio, fin, personas);
        }


    }
}
