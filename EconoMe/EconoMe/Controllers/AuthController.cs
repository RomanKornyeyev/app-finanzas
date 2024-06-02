using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using EconoMe.Models;
using EconoMe.Models.DTOs;

namespace EconoMe.Controllers
{
    // *************** AUTH CONTROLLER (LOGIN, REGISTER, ETC) ***************
    public class AuthController : BaseController
    {
        // ***** REGISTRO *****
        [HttpPost]
        [Route("api/Auth/Register")]
        public IHttpActionResult Register(RegistroUsuarioRequest request)
        {
            // Verificar si el user ya existe
            if (_context.Usuarios.Any(u => u.NombreUsuario == request.NombreUsuario))
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Verificar si el email está en uso
            if (_context.Usuarios.Any(u => u.Email == request.Email))
            {
                return BadRequest("El correo electrónico ya está en uso.");
            }

            // Crear nuevo user (si las validaciones han ido bien)
            var nuevoUsuario = new Usuarios
            {
                NombreUsuario = request.NombreUsuario,
                Clave = PasswordHasher.HashPassword(request.Clave),
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Email = request.Email,
                Token = GenerateToken(),
                TokenGenerated = DateTime.UtcNow
            };

            // Agregar el nuevo usuario a la base de datos
            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            // Retornamos token y mensaje de ok
            return Ok(new { Token = nuevoUsuario.Token, Message = "Usuario registrado exitosamente." });
        }

        // ***** LOGIN *****
        [HttpPost]
        [Route("api/Auth/Login")]
        public IHttpActionResult Login(LoginUsuarioRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == request.Email);

            // Credenciales incorrectas, NO AUTORIZADO
            if (usuario == null || !PasswordHasher.VerifyPassword(request.Clave, usuario.Clave))
            {
                return Unauthorized();
            }

            bool existeTokenValido = ValidateToken(usuario.Token);

            // Se comprueba si existe token válido
            if (existeTokenValido)
            {
                // Si existe, se renueva la duración
                usuario.TokenGenerated = DateTime.UtcNow;
            }
            else
            {
                // Si no existe, se genera uno nuevo
                usuario.Token = GenerateToken();
                usuario.TokenGenerated = DateTime.UtcNow;
            }

            _context.SaveChanges();

            // Se devuelve el token
            return Ok(new { Token = usuario.Token });
        }

        // ***** OBTENER DATOS DE USER *****
        [HttpGet]
        [Route("api/Auth/GetUserInfo")]
        public IHttpActionResult GetUserInfo()
        {
            string authToken = Request.Headers.GetValues("Authorization").First();

            var user = _context.Usuarios.FirstOrDefault(u => u.Token == authToken);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                user.id,
                user.NombreUsuario,
                user.Email,
                user.Nombres,
                user.Apellidos
            });
        }

        // ***** VALIDAR TOKEN *****
        [HttpGet]
        [Route("api/Auth/ValidateToken")]
        public IHttpActionResult ValidateToken()
        {
            var headers = Request.Headers;
            if (headers.Contains("Authorization"))
            {
                var token = headers.GetValues("Authorization").First();
                if (ValidateToken(token))
                {
                    // Token válido, se renueva y se devuelve un ok
                    RenewToken(token);
                    return Ok();
                }
            }

            // Token no válido, no autorizado
            return Unauthorized();
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