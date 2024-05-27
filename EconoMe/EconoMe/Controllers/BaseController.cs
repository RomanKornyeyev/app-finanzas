using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Http;
using EconoMe.Models;

namespace EconoMe.Controllers
{
    // *************** BASE CONTROLLER (GENERACIÓN TOKENS, RENOVACIÓN TOKENS, ETC.) ***************
    public class BaseController : ApiController
    {
        protected readonly ControlFinanzasEntities _context;
        protected const int TOKEN_EXPIRATION_DAYS = 7; // Duración del token en días

        public BaseController()
        {
            _context = new ControlFinanzasEntities();
        }

        // Método para generar un token aleatorio
        protected string GenerateToken()
        {
            byte[] tokenBytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        // Método para validar un token para un usuario específico
        protected bool ValidateToken(string token)
        {
            //buscar el user y verificar si existe y el token coincide
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token);
            return usuario != null && usuario.Token == token && usuario.TokenGenerated.HasValue && DateTime.UtcNow.Subtract(usuario.TokenGenerated.Value).TotalDays < TOKEN_EXPIRATION_DAYS;
        }

        // Método para renovar el token de un usuario
        protected void RenewToken(string token)
        {
            if (ValidateToken(token))
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token);
                // Renovar la duración del token
                usuario.TokenGenerated = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}