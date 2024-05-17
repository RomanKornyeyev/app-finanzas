using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;


namespace EconoMeMVC.Controllers
{
    public class HomeController : Controller
    {
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

        public async Task<ActionResult> Math()
        {
            ViewBag.Message = "Your math page.";

            string apiUrl = "https://localhost:44386/api/Math/Sumar?a=5&b=20";

            // Realizar una solicitud GET a la API y obtener el resultado
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string resultado = await response.Content.ReadAsStringAsync();

                    // Pasar el resultado a la vista
                    ViewBag.Resultado = resultado;
                }
                else
                {
                    // Manejar el error de la solicitud
                    ViewBag.Error = "Error al realizar la solicitud a la API";
                }
            }

            return View();
        }
    }
}