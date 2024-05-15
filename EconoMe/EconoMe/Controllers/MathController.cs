using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EconoMe.Controllers
{
    public class MathController : ApiController
    {
        // GET: api/Math/Sumar
        [HttpGet]
        [Route("api/Math/Sumar")]
        public IHttpActionResult Sumar(int a, int b)
        {
            int resultado = a + b;
            return Ok(resultado);
        }
    }
}
