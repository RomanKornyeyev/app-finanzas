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
using EconoMeMVC.Models;
using EconoMeMVC.Helpers;


namespace EconoMeMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly string apiUrl = "https://localhost:44386/api/Auth/Login";

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

        // ================== LOGIN Y AUTH ==================
        public ActionResult Login()
        {
            // Verificar si el usuario ya está logueado
            if (UserSessionHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);

                    // Store the token in a cookie
                    HttpCookie authCookie = new HttpCookie("AuthToken", tokenResponse.Token);
                    authCookie.HttpOnly = true; // Optional: make it HTTP-only for security
                    authCookie.Secure = Request.IsSecureConnection; // Only send via HTTPS
                    authCookie.SameSite = SameSiteMode.Lax; // Prevent CSRF
                    Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Credenciales incorrectas.");
                    //ViewBag.ErrorMessage = "Credenciales incorrectas. VIEWBAG";
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error al intentar iniciar sesión.");
                    //ViewBag.ErrorMessage = "Ha ocurrido un error al intentar iniciar sesión.";
                }
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            // Borrar la cookie de autenticación
            if (Request.Cookies["AuthToken"] != null)
            {
                var cookie = new HttpCookie("AuthToken");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            // Redirigir al usuario a la página de inicio
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            // Verificar si el usuario ya está logueado
            if (UserSessionHelper.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"]);

                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/Auth/Register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseString);

                        // Store the token in a cookie
                        HttpCookie authCookie = new HttpCookie("AuthToken", tokenResponse.Token);
                        authCookie.HttpOnly = true; // Optional: make it HTTP-only for security
                        authCookie.Secure = Request.IsSecureConnection; // Only send via HTTPS
                        authCookie.SameSite = SameSiteMode.Lax; // Prevent CSRF
                        Response.Cookies.Add(authCookie);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", errorResponse);
                    }
                }
            }

            return View(model);
        }


        // ================== MATH (TEST) ==================
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

    public class TokenResponse
    {
        public string Token { get; set; }
    }
}