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
    public class AuthController : ApiController
    {
        private readonly ControlFinanzasEntities _context;
        private const int TokenExpirationDays = 7; // Duración del token en días

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
                Clave = PasswordHasher.HashPassword(request.Clave), // La contraseña ya llega hasheada
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

            // Credenciales incorrectas, NO AUTORIZADO
            if (usuario == null || !PasswordHasher.VerifyPassword(request.Clave, usuario.Clave))
            {
                return Unauthorized();
            }

            //quitar el id del login y modificar la función
            // Se comprueba si existe un token y es válido (no está caducado, 7 días)
            bool existeTokenValido = ValidateToken(usuario.Token);

            // Renovar la duración del token o generar uno nuevo si no existe
            if (existeTokenValido)
            {
                // Token existente y vigente, renovar la duración
                usuario.TokenGenerated = DateTime.UtcNow;
            }
            else
            {
                // Generar un nuevo token y sobreescribirlo en la base de datos
                usuario.Token = GenerateToken();
                usuario.TokenGenerated = DateTime.UtcNow;
            }

            _context.SaveChanges();

            return Ok(new { Token = usuario.Token });
        }

        // Obtener todas las transacciones de un usuario específico
        [HttpGet]
        [Route("api/Reports/GetUserGlobalTransactions")]
        public IHttpActionResult GetUserGlobalTransactions()
        {
            // Leer el token de acceso y el ID de usuario de los encabezados de la solicitud
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

            // Comprobar si el token es válido
            if (!ValidateToken(token))
            {
                return Unauthorized();
            }

            // Buscar las transacciones del usuario en la base de datos
            // (normalmente la querys no se hacen en los controladores)
            var userId = _context.Usuarios
                .Where(u => u.Token == token)
                .Select(u => u.id)
                .FirstOrDefault();
            var transactions = _context.Transaciones
                .Where(t => t.UsuarioId == userId)
                .ToList();

            List<TransaccionesDTO> retVal = new List<TransaccionesDTO>();
            foreach (var transaccion in transactions)
            {
                retVal.Add(new TransaccionesDTO(transaccion));
            }

            return Ok(retVal);
        }

        // Obtener todas las transacciones de un usuario específico (por año)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByYear")]
        public IHttpActionResult GetUserTransactionsByYear(int year)
        {
            // Leer el token de acceso y el ID de usuario de los encabezados de la solicitud
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

            // Comprobar si el token es válido
            if (!ValidateToken(token))
            {
                return Unauthorized();
            }

            var userId = _context.Usuarios
                .Where(u => u.Token == token)
                .Select(u => u.id)
                .FirstOrDefault();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var transactions = _context.Transaciones
                .Where(t => t.UsuarioId == userId && t.Fecha.Year == year)
                .ToList();

            var transactionsDto = transactions.Select(t => new TransaccionesDTO(t)).ToList();

            return Ok(transactionsDto);
        }

        // Obtener todas las transacciones de un usuario específico (por mes)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByMonth")]
        public IHttpActionResult GetUserTransactionsByMonth(int year, int month)
        {
            // Leer el token de acceso y el ID de usuario de los encabezados de la solicitud
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

            // Comprobar si el token es válido
            if (!ValidateToken(token))
            {
                return Unauthorized();
            }

            var userId = _context.Usuarios
                .Where(u => u.Token == token)
                .Select(u => u.id)
                .FirstOrDefault();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var transactions = _context.Transaciones
                .Where(t => t.UsuarioId == userId && t.Fecha.Year == year && t.Fecha.Month == month)
                .ToList();

            var transactionsDto = transactions.Select(t => new TransaccionesDTO(t)).ToList();

            return Ok(transactionsDto);
        }

        // Obtener todas las transacciones de un usuario específico (por día)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByDay")]
        public IHttpActionResult GetUserTransactionsByDay(int year, int month, int day)
        {
            // Leer el token de acceso y el ID de usuario de los encabezados de la solicitud
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

            // Comprobar si el token es válido
            if (!ValidateToken(token))
            {
                return Unauthorized();
            }

            var userId = _context.Usuarios
                .Where(u => u.Token == token)
                .Select(u => u.id)
                .FirstOrDefault();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var transactions = _context.Transaciones
                .Where(t => t.UsuarioId == userId && t.Fecha.Year == year && t.Fecha.Month == month && t.Fecha.Day == day)
                .ToList();

            var transactionsDto = transactions.Select(t => new TransaccionesDTO(t)).ToList();

            return Ok(transactionsDto);
        }



        // Método para generar un token aleatorio
        private string GenerateToken()
        {
            byte[] tokenBytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        // cambiar la función para que solo use token (y no id de user) buscar token y mirar que no esté caducado
        // Método para validar un token para un usuario específico
        private bool ValidateToken(string token)
        {
            // Buscar el usuario en la base de datos por su ID
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token);

            // Verificar si el usuario existe y el token coincide
            return usuario != null && usuario.Token == token && usuario.TokenGenerated.HasValue && DateTime.UtcNow.Subtract(usuario.TokenGenerated.Value).TotalDays < TokenExpirationDays;
        }

        // Método para renovar el token de un usuario
        private void RenewToken(int userId, string token)
        {
            // Comprobar si el token es válido
            if (ValidateToken(token))
            {
                // Buscar el usuario en la base de datos por su ID
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token);

                // Renovar la duración del token
                usuario.TokenGenerated = DateTime.UtcNow;

                // Guardar los cambios en la base de datos
                _context.SaveChanges();
            }
            // Si el token no es válido, no se hace nada
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
        //public int Id {  get; set; } //params opcionales para token
        //public string Token { get; set; } //params opcionales para token
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