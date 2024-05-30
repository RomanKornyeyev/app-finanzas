using EconoMe.Models;
using EconoMe.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EconoMe.Controllers
{
    // **************** CONTROLLER PARA METER / MANIPULAR MOVIMIENTOS ****************
    [RoutePrefix("api/Transacciones")]
    public class TransaccionesController : BaseController
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> RegisterTransaccion([FromBody] TransaccionesDTO transaccionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var transaccion = transaccionDto.ToEntity();
                _context.Transaciones.Add(transaccion);

                // Verificar y agregar la categoría asociada
                if (transaccionDto.TransaccionesCategorias.Count > 0)
                {
                    var categoriaNombre = transaccionDto.TransaccionesCategorias[0];
                    var categoria = _context.Categorias
                        .FirstOrDefault(c => c.Nombre == categoriaNombre);

                    if (categoria != null)
                    {
                        transaccion.TransaccionesCategorias = new List<TransaccionesCategorias>
                        {
                            new TransaccionesCategorias { CategoriaId = categoria.Id }
                        };
                    }
                }

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
}
