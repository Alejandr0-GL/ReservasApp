using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Web.Models;
using System.Diagnostics;
using Reservas.Business;

namespace Reservas.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SedeService _sedeService;

        public HomeController(SedeService sedeService)
        {
            _sedeService = sedeService;
        }

        public async Task<IActionResult> Index()
        {
            var sedes = await _sedeService.ObtenerSedesActivasAsync();
            return View(sedes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
