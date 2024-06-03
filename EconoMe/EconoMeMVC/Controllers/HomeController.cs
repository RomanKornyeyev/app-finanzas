using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using EconoMeMVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using EconoMeMVC.Helpers;
using EconoMe.Models;


namespace EconoMeMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly string API_BASE_URL = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard(int? year, int? month)
        {
            if (year.HasValue && month.HasValue)
            {
                ViewBag.Message = $"Dashboard for Year: {year}, Month: {month}";
            }
            else if (year.HasValue)
            {
                ViewBag.Message = $"Dashboard for Year: {year}";
            }
            else
            {
                ViewBag.Message = "Global Dashboard";
            }

            ViewBag.Year = year;
            ViewBag.Month = month;

            return View();
        }


        // ************ REGISTER MOVEMENTS ************
        public ActionResult Movements()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterTransaction(RegisterTransaccionesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Movements", model);
            }

            try
            {
                var token = Request.Cookies["AuthToken"]?.Value;

                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError("", "No se encontró el token de autenticación.");
                    return View("Movements", model);
                }

                var transaccionDto = new
                {
                    Importe = model.Importe,
                    Concepto = model.Concepto,
                    Descripcion = model.Descripcion,
                    TipoTransaccionId = model.TipoTransaccionId,
                    TransaccionesCategorias = new List<string> { model.CategoriaId.ToString() }
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);

                    var response = await client.PostAsJsonAsync($"{API_BASE_URL}Transacciones/Register", transaccionDto);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Transacción registrada exitosamente";
                        return RedirectToAction("Movements");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al registrar la transacción: " + response.ReasonPhrase);
                        return View("Movements", model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error interno: " + ex.Message);
                return View("Movements", model);
            }
        }
    }
}