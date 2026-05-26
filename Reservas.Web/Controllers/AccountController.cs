using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservas.Business;
using Reservas.Entities;
using Reservas.Web.Models;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ActualizarDatos()
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            if (userId == 0) return RedirectToAction("Login");

            var usuario = await _usuarioService.ObtenerUsuarioConMunicipioAsync(userId);
            if (usuario == null) return NotFound();

            var departamentos = await _usuarioService.ObtenerDepartamentosAsync();
            var municipios = await _usuarioService.ObtenerMunicipiosPorDepartamentoAsync(usuario.Municipio.DepartamentoId);

            ViewBag.Departamentos = new SelectList(departamentos, "DepartamentoId", "Nombre", usuario.Municipio.DepartamentoId);
            ViewBag.Municipios = new SelectList(municipios, "MunicipioId", "Nombre", usuario.MunicipioId);

            var model = new ActualizarUsuarioViewModel
            {
                UsuarioId = usuario.UsuarioId,
                NroDocumento = usuario.NroDocumento,
                NombreCompleto = usuario.NombreCompleto,
                FechaNacimiento = usuario.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
                Celular = usuario.Celular,
                DireccionEmail = usuario.DireccionEmail,
                DepartamentoId = usuario.Municipio.DepartamentoId,
                MunicipioId = usuario.MunicipioId,
                Barrio = usuario.Barrio,
                DireccionResidencia = usuario.DireccionResidencia,
                TelefonoResidencia = usuario.TelefonoResidencia,
                AutorizaCorreo = usuario.AutorizaCorreo,
                AutorizaCelular = usuario.AutorizaCelular
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarDatos(ActualizarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var departamentos = await _usuarioService.ObtenerDepartamentosAsync();
                var municipios = await _usuarioService.ObtenerMunicipiosPorDepartamentoAsync(model.DepartamentoId);

                ViewBag.Departamentos = new SelectList(departamentos, "DepartamentoId", "Nombre", model.DepartamentoId);
                ViewBag.Municipios = new SelectList(municipios, "MunicipioId", "Nombre", model.MunicipioId);

                return View(model);
            }

            var datos = new Usuario
            {
                UsuarioId = model.UsuarioId,
                NombreCompleto = model.NombreCompleto,
                FechaNacimiento = DateOnly.FromDateTime(model.FechaNacimiento),
                Celular = model.Celular,
                DireccionEmail = model.DireccionEmail,
                MunicipioId = model.MunicipioId,
                Barrio = model.Barrio,
                DireccionResidencia = model.DireccionResidencia,
                TelefonoResidencia = model.TelefonoResidencia,
                AutorizaCorreo = model.AutorizaCorreo,
                AutorizaCelular = model.AutorizaCelular
            };

            var actualizado = await _usuarioService.ActualizarUsuarioAsync(datos);

            var deps = await _usuarioService.ObtenerDepartamentosAsync();
            var mun = await _usuarioService.ObtenerMunicipiosPorDepartamentoAsync(model.DepartamentoId);

            ViewBag.Departamentos = new SelectList(deps, "DepartamentoId", "Nombre", model.DepartamentoId);
            ViewBag.Municipios = new SelectList(mun, "MunicipioId", "Nombre", model.MunicipioId);

            if (!actualizado)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar. Verifica que el correo no esté en uso.");
                return View(model);
            }

            ViewData["MensajeExito"] = "Datos actualizados correctamente.";
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCuenta()
        {
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            if (userId == 0)
            {
                return RedirectToAction("Login");
            }

            var eliminado = await _usuarioService.DesactivarUsuarioAsync(userId);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!eliminado)
            {
                TempData["MensajeError"] = "No se pudo eliminar la cuenta.";
                return RedirectToAction("ActualizarDatos");
            }

            TempData["MensajeExito"] = "Cuenta eliminada correctamente.";
            return RedirectToAction("Login");
        }
    }
}
