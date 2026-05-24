using Microsoft.EntityFrameworkCore;
using Reservas.DataAccess;
using Reservas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservas.Business
{
    public class SedeService
    {
        private readonly FondoDbContext _context;

        public SedeService(FondoDbContext context)
        {
            _context = context;
        }

        //Obtiene las sedes
        public async Task<List<Sede>> ObtenerSedesActivasAsync()
        {
            return await _context.Sedes
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        //Obtiene detalles de una sede incluyendo espacios, tarifas y servicios para mostrarlo en los detalles
        public async Task<Sede?> ObtenerSedeDetalleAsync(int sedeId)
        {
            return await _context.Sedes
                .Include(s => s.Espacios)
                    .ThenInclude(e => e.Caracteristicas)
                .Include(s => s.TarifaConfigs)
                .Include(s => s.ServicioExtras)
                .FirstOrDefaultAsync(s => s.SedeId == sedeId);
        }

    }
}
