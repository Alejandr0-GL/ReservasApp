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
            var noches = Math.Max(1, (fechaFin.Date - fechaInicio.Date).Days);
            var numHabitaciones = espacioId.HasValue ? 1 : 0;

            var tipoTemporada = await ObtenerTipoTemporadaAsync(fechaInicio, fechaFin);

            var total = await _context.CalcularTarifaReservaAsync(
                sedeId,
                espacioId,
                tipoTemporada,
                numHabitaciones,
                personas,
                noches,
                tipoReserva,
                fechaInicio,
                fechaFin);

            if (incluyeLavanderia)
            {
                var precioLav = await _context.ServicioExtras
                    .Where(s => s.SedeId == sedeId && s.Nombre == "Lavandería")
                    .Select(s => s.Precio)
                    .FirstOrDefaultAsync();

                total += precioLav;
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

        public async Task<string> ObtenerTipoTemporadaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = DateOnly.FromDateTime(fechaInicio);
            var fin = DateOnly.FromDateTime(fechaFin);

            var tipos = await _context.Temporada
                .Where(t => t.FechaInicio <= fin && t.FechaFin >= inicio)
                .Select(t => t.Tipo)
                .ToListAsync();

            if (tipos.Count == 0)
            {
                return "Baja";
            }

            if (tipos.Contains("Alta"))
            {
                return "Alta";
            }

            return tipos.First();
        }

        public async Task<List<ResultadoTarifa>> ObtenerTarifasAsync(int sedeId, string tipoTemporada, int personas)
        {
            return await _context.ObtenerTarifasAsync(sedeId, tipoTemporada, personas, null);
        }

        public async Task<decimal> CalcularTotalAsync(
            int sedeId,
            int? espacioId,
            string tipoReserva,
            DateTime fechaInicio,
            DateTime fechaFin,
            int personas,
            bool incluyeLavanderia)
        {
            var noches = Math.Max(1, (fechaFin.Date - fechaInicio.Date).Days);
            var numHabitaciones = espacioId.HasValue ? 1 : 0;

            var tipoTemporada = await ObtenerTipoTemporadaAsync(fechaInicio, fechaFin);

            var total = await _context.CalcularTarifaReservaAsync(
                sedeId,
                espacioId,
                tipoTemporada,
                numHabitaciones,
                personas,
                noches,
                tipoReserva,
                fechaInicio,
                fechaFin);

            if (incluyeLavanderia)
            {
                var precioLav = await _context.ServicioExtras
                    .Where(s => s.SedeId == sedeId && s.Nombre == "Lavandería")
                    .Select(s => s.Precio)
                    .FirstOrDefaultAsync();

                total += precioLav;
            }

            return total;
        }
    }
}
