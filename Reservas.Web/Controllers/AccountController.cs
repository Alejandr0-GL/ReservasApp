using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservas.Business;
using Reservas.Entities;
using System.Security.Claims;

namespace Reservas.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioService _usuarioService;
        public AccountController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nroDocumento, string clave)
        {
            if (string.IsNullOrEmpty(nroDocumento) || string.IsNullOrEmpty(clave))
            {
                ModelState.AddModelError(string.Empty, "Número de documento y clave son requeridos.");
                return View();
            }

            //Llama a UsuarioService para validar login
            var usuario = await _usuarioService.LoginAsync(nroDocumento, clave);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.DireccionEmail)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Número de documento o clave incorrectos.");
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            var departamentos = await _usuarioService.ObtenerDepartamentosAsync();
            ViewBag.Departamentos = new SelectList(departamentos, "DepartamentoId", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro (Usuario usuario, string clave, string respuestaSecreta)
        {
            if (ModelState.IsValid)
            {
                var result = await _usuarioService.RegistrarUsuarioAsync(usuario, clave, respuestaSecreta);
                if (result)
                {
                    TempData["MensajeExito"] = "Usuario registrado exitosamente.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar el usuario. Intente nuevamente.");
                }
            }

            var departamentos = await _usuarioService.ObtenerDepartamentosAsync();
            ViewBag.Departamentos = new SelectList(departamentos, "DepartamentoId", "Nombre");
            return View(usuario);
        }


        //Carga municipios dinámicamente
        [HttpGet]
        public async Task<JsonResult> GetMunicipios(int departamentoId)
        {
            var municipios = await _usuarioService.ObtenerMunicipiosPorDepartamentoAsync(departamentoId);
            
            var resultado = municipios.Select(m => new { id = m.MunicipioId, nombre = m.Nombre }).ToList();
            return Json(resultado);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
