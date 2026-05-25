using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Business;
using Reservas.Entities;
using Reservas.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reservas.Web.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly SedeService _sedeService;
        private readonly ReservaService _reservaService;

        public ReservaController(SedeService sedeService, ReservaService reservaService)
        {
            _sedeService = sedeService;
            _reservaService = reservaService;
        }

        [HttpGet]
        public async Task<IActionResult> SelectLocation(int sedeId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var sede = await _sedeService.ObtenerSedeDetalleAsync(sedeId);
            if (sede == null) return NotFound();

            var disponibilidades = new List<ResultadoDisponibilidad>();

            if (fechaInicio.HasValue && fechaFin.HasValue && fechaFin.Value.Date >= fechaInicio.Value.Date)
            {
                disponibilidades = await _reservaService.ConsultarDisponibilidadAsync(sedeId, fechaInicio.Value, fechaFin.Value);
            }

            var vm = new SelectLocationViewModel
            {
                Sede = sede,
                Espacios = sede.Espacios.OrderBy(e => e.NumeroAlojamiento).ToList(),
                TarifaConfigs = sede.TarifaConfigs.ToList(),
                ServicioExtras = sede.ServicioExtras.ToList(),
                Disponibilidades = disponibilidades,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Disponibilidad(int sedeId, DateTime fechaInicio, DateTime fechaFin, int? personas)
        {
            if (fechaFin.Date < fechaInicio.Date)
            {
                return BadRequest();
            }

            List<ResultadoDisponibilidad> disponibilidades;

            if (personas.HasValue && personas.Value > 0)
            {
                disponibilidades = await _reservaService.ConsultarDisponibilidadPorCupoAsync(sedeId, fechaInicio, fechaFin, personas.Value);
            }
            else
            {
                disponibilidades = await _reservaService.ConsultarDisponibilidadAsync(sedeId, fechaInicio, fechaFin);
            }

            var espacioIds = disponibilidades.Select(d => d.EspacioId).ToList();
            return Json(new { espacioIds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(ReservaSolicitudViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TipoReserva))
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar el tipo de reserva.");
            }

            if (model.TipoReserva == "Hospedaje" && model.EspacioId == null)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un alojamiento para hospedaje.");
            }

            if (model.TipoReserva == "Hospedaje" && model.FechaFin <= model.FechaInicio)
            {
                ModelState.AddModelError(string.Empty, "La fecha de salida debe ser mayor a la fecha de llegada.");
            }

            if (model.TipoReserva == "VisitaDia" && model.FechaFin < model.FechaInicio)
            {
                ModelState.AddModelError(string.Empty, "La fecha de visita no puede ser anterior a la de llegada.");
            }

            if (!ModelState.IsValid)
            {
                var sede = await _sedeService.ObtenerSedeDetalleAsync(model.SedeId);
                if (sede == null) return NotFound();

                var vm = new SelectLocationViewModel
                {
                    Sede = sede,
                    Espacios = sede.Espacios.OrderBy(e => e.NumeroAlojamiento).ToList(),
                    TarifaConfigs = sede.TarifaConfigs.ToList(),
                    ServicioExtras = sede.ServicioExtras.ToList()
                };

                return View("SelectLocation", vm);
            }

            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            if (userId == 0) return RedirectToAction("Login", "Account");

            await _reservaService.CrearReservaAsync(
                userId,
                model.SedeId,
                model.EspacioId,
                model.ReservaPadreId,
                model.TipoReserva,
                model.FechaInicio,
                model.FechaFin,
                model.Personas,
                model.IncluyeLavanderia);

            TempData["MensajeExito"] = "Reserva creada correctamente.";
            return RedirectToAction("SelectLocation", new { sedeId = model.SedeId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MisReservas()
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            if (userId == 0) return RedirectToAction("Login", "Account");

            var reservas = await _reservaService.ObtenerReservasUsuarioAsync(userId);
            return View(reservas);
        }
    }
}