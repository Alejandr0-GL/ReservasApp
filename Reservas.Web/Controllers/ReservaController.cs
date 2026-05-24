using Microsoft.AspNetCore.Mvc;
using Reservas.Business;
using Reservas.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Reservas.Web.Controllers
{
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
        public async Task<IActionResult> SelectLocation(int sedeId)
        {
            var sede = await _sedeService.ObtenerSedeDetalleAsync(sedeId);
            if (sede == null) return NotFound();

            var vm = new SelectLocationViewModel
            {
                Sede = sede,
                Espacios = sede.Espacios.OrderBy(e => e.NumeroAlojamiento).ToList(),
                TarifaConfigs = sede.TarifaConfigs.ToList(),
                ServicioExtras = sede.ServicioExtras.ToList()
            };

            return View(vm);
        }
    }
}