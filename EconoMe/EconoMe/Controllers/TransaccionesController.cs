using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using EconoMe.Controllers;
using EconoMe.Models;
using EconoMe.Models.DTOs;
using Newtonsoft.Json;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Util;

namespace EconoMe.API.Controllers
{
    [RoutePrefix("api/Transacciones")]
    public class TransaccionesController : BaseController
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> RegisterTransaccion([FromBody] TransaccionesDTO transaccionDto)
        {
            // VALIDACIÓN DEL MODELO DTO NO NECESARIA, AL ASIGNAR ID_USER Y FECHA AUTOMÁTICAMENTE EN ESTA FUNCT
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            // Validar que los campos necesarios lleguen en el DTO
            //if (transaccionDto == null || transaccionDto.Importe <= 0 || string.IsNullOrEmpty(transaccionDto.Concepto))
            //{
            //    return BadRequest("Datos de la transacción incompletos o inválidos.");
            //}


            //LOCALIZAR AL USER
            string authToken = Request.Headers.GetValues("Authorization").First();
            var user = _context.Usuarios.FirstOrDefault(u => u.Token == authToken);
            if (user == null)
            {
                return Unauthorized();
            }

            var userInfo = new UserInfo
            {
                id = user.id,
                NombreUsuario = user.NombreUsuario,
                Email = user.Email,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos
            };

            //ASIGNAMOS FECHA Y USUARIOID AUTOMÁTICAMENTE
            transaccionDto.Fecha = DateTime.Now;
            transaccionDto.UsuarioId = userInfo.id;


            //METER LA TRANSACCIÓN AL USER
            try
            {
                var transaccion = transaccionDto.ToEntity();
                _context.Transaciones.Add(transaccion);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Transacción registrada exitosamente" });
            }
            catch (Exception ex)
            {
                // Log the error (ex) if needed
                return InternalServerError(ex);
            }
        }
    }

    public class UserInfo
    {
        public int id { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
    }

}
