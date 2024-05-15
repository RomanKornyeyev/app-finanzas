using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using EconoMe.Models;

namespace EconoMe.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ControlFinanzasEntities _context;

        public AuthController()
        {
            _context = new ControlFinanzasEntities();
        }

        // POST: api/Auth/Register
        [HttpPost]
        [Route("api/Auth/Register")]
        public IHttpActionResult Register(RegistroUsuarioRequest request)
        {
            // Verificar si el usuario ya existe
            if (_context.Usuarios.Any(u => u.NombreUsuario == request.NombreUsuario))
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Verificar si el correo electrónico ya está en uso
            if (_context.Usuarios.Any(u => u.Email == request.Email))
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            // Crear un nuevo usuario
            var nuevoUsuario = new Usuarios
            {
                NombreUsuario = request.NombreUsuario,
                Clave = request.Clave, // La contraseña ya llega hasheada
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Email = request.Email
                // El campo Foto es opcional y se dejará como nulo por ahora
            };

            // Agregar el nuevo usuario a la base de datos
            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            return Ok("Usuario registrado exitosamente.");
        }

        // POST: api/Auth/Login
        [HttpPost]
        [Route("api/Auth/Login")]
        public IHttpActionResult Login(LoginUsuarioRequest request)
        {
            // Buscar el usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == request.Email);

            if (usuario == null || !PasswordHasher.VerifyPassword(request.Clave, usuario.Clave))
            {
                return Unauthorized();
            }

            // Aquí podrías generar y devolver un token de acceso si lo necesitas

            return Ok("Inicio de sesión exitoso.");
        }
    }

    // Clase para recibir los datos de registro de usuario
    public class RegistroUsuarioRequest
    {
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        // El campo Foto es opcional y se dejará como nulo por ahora
    }

    // Clase para recibir los datos de inicio de sesión de usuario
    public class LoginUsuarioRequest
    {
        public string Email { get; set; }
        public string Clave { get; set; }
    }

    // Clase para hashear y verificar contraseñas
    public class PasswordHasher
    {
        // Método para hashear una contraseña
        public static string HashPassword(string password)
        {
            // Generar una sal aleatoria
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Generar el hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combinar la sal y el hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convertir el hash a string
            return Convert.ToBase64String(hashBytes);
        }

        // Método para verificar una contraseña
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Obtener la sal y el hash desde el hash almacenado
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Comparar los hashes
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}