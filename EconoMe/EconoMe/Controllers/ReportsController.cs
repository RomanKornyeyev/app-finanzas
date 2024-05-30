using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EconoMe.Models;
using EconoMe.Models.DTOs;

namespace EconoMe.Controllers
{
    // *************** REPORTS CONTROLLER (ALL MOVEMENTS, REPORTS, ETC.) ***************
    public class ReportsController : BaseController
    {
        // Obtener todas las transacciones de un user en específico
        [HttpGet]
        [Route("api/Reports/GetUserGlobalTransactions")]
        public IHttpActionResult GetUserGlobalTransactions()
        {
            // Leer el token de acceso del HEADER
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

            // Comprobamos si el token es válido
            if (!ValidateToken(token))
            {
                return Unauthorized();
            }

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

        // Obtener todas las transacciones de un user en específico (POR AÑO)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByYear")]
        public IHttpActionResult GetUserTransactionsByYear(int year)
        {
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

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

        // Obtener todas las transacciones de un user en específico (POR MES)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByMonth")]
        public IHttpActionResult GetUserTransactionsByMonth(int year, int month)
        {
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

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

        // Obtener todas las transacciones de un user en específico (POR DÍA)
        [HttpGet]
        [Route("api/Reports/GetUserTransactionsByDay")]
        public IHttpActionResult GetUserTransactionsByDay(int year, int month, int day)
        {
            var headers = Request.Headers;
            if (!headers.Contains("Authorization"))
            {
                return BadRequest("Faltan encabezados de autenticación.");
            }

            string token = headers.GetValues("Authorization").FirstOrDefault();

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

        // **************** SUMMARY JSON FOR VIEWS ****************

    }
}
