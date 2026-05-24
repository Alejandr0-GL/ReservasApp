using Microsoft.EntityFrameworkCore;
using Reservas.DataAccess;
using Reservas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Reservas.Business
{
    public class UsuarioService
    {
        private readonly FondoDbContext _context;

        public UsuarioService(FondoDbContext context)
        {
            _context = context;
        }

        //Login usuario
        public async Task<Usuario> LoginAsync(string nroDocumento, string clave)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NroDocumento == nroDocumento && u.Activo == true);

            if (usuario == null) return null; //Usuario no encontrado o inactivo

            string claveHash = CalcularHashSHA256(clave);

            if (usuario.Clave == claveHash)
            {
                return usuario; //Login exitoso
            }

            return null; //Clave incorrecta
            
        }

        //Registrar usuario
        public async Task<bool> RegistrarUsuarioAsync(Usuario usuario, string clave, string respuestaSecreta)
        {
            bool existe = await _context.Usuarios.AnyAsync(u => u.NroDocumento == usuario.NroDocumento || u.DireccionEmail == usuario.DireccionEmail);
            if (existe) return false;

            //Hashea clave y respuesta secreta
            usuario.Clave = CalcularHashSHA256(clave);
            usuario.RespuestaSecreta = HashTexto(respuestaSecreta);
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        //Obtener departamentos y municipios con departamentoId
        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            return await _context.Departamentos.ToListAsync();
        }

        public async Task<List<Municipio>> ObtenerMunicipiosPorDepartamentoAsync(int departamentoId)
        {
            return await _context.Municipios
                .Where(m => m.DepartamentoId == departamentoId)
                .ToListAsync();
        }

        //Hash
        private string CalcularHashSHA256(string texto)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //Hash texto
        private string HashTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            return CalcularHashSHA256(texto.Trim().ToLower());
        }
    }
}
