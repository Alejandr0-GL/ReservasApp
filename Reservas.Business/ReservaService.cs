using Microsoft.EntityFrameworkCore;
using Reservas.DataAccess;
using Reservas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<Reserva> CrearReservaAsync(
            int usuarioId,
            int sedeId,
            int? espacioId,
            int? reservaPadreId,
            string tipoReserva,
            DateTime fechaInicio,
            DateTime fechaFin,
            int personas,
            bool incluyeLavanderia)
        {
            decimal total;

            if (tipoReserva == "VisitaDia")
            {
                var tarifaVisita = await _context.TarifaConfigs
                    .Where(t => t.SedeId == sedeId && t.EsVisitaDia)
                    .FirstOrDefaultAsync();

                var minAcomp = tarifaVisita?.MinAcompanantesTarifaEspecial ?? 5;
                var precioExtra = tarifaVisita?.PrecioAcompananteAdicional ?? 5500m;

                var extras = Math.Max(0, personas - (minAcomp - 1));
                total = extras * precioExtra;
            }
            else
            {
                var tarifa = await _context.TarifaConfigs
                    .Where(t => t.EspacioId == espacioId && t.TipoTemporada == "Baja")
                    .Select(t => t.PrecioOrdinario)
                    .FirstOrDefaultAsync() ?? 0m;

                if (tarifa == 0)
                {
                    tarifa = await _context.TarifaConfigs
                        .Where(t => t.SedeId == sedeId && t.EspacioId == null && t.TipoTemporada == "Baja")
                        .Select(t => t.PrecioOrdinario)
                        .FirstOrDefaultAsync() ?? 0m;
                }

                var noches = Math.Max(1, (fechaFin.Date - fechaInicio.Date).Days);
                total = tarifa * noches;

                if (incluyeLavanderia)
                {
                    var precioLav = await _context.ServicioExtras
                        .Where(s => s.SedeId == sedeId && s.Nombre == "Lavandería")
                        .Select(s => s.Precio)
                        .FirstOrDefaultAsync();

                    total += precioLav;
                }
            }

            var reserva = new Reserva
            {
                UsuarioId = usuarioId,
                SedeId = sedeId,
                EspacioId = espacioId,
                ReservaPadreId = reservaPadreId,
                TipoReserva = tipoReserva,
                FechaInicio = DateOnly.FromDateTime(fechaInicio),
                FechaFin = DateOnly.FromDateTime(fechaFin),
                Personas = personas,
                IncluyeLavanderia = incluyeLavanderia,
                ValorTotal = total,
                Estado = "Pendiente",
                CreadoEn = DateTime.UtcNow
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return reserva;
        }

        public async Task<List<Reserva>> ObtenerReservasUsuarioAsync(int usuarioId)
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Sede)
                .Include(r => r.Espacio)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.CreadoEn)
                .ToListAsync();
        }
    }
}
