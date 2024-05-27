using EconoMeMVC.Helpers;
using EconoMeMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EconoMeMVC.Controllers
{
    // ********** AUTH CONTROLLER (LOGIN, REGISTER, ETC.) **********
    public class AuthController : Controller
    {

        private readonly string API_BASE_URL = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];


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
                var response = await client.PostAsync(API_BASE_URL + "Auth/Login", content);

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

        // REGISTRO
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

                    var response = await client.PostAsync("Auth/Register", content);

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



        // ********** ¡¡¡IMP: FUNCIÓN EN DESUSO!!! SE GESTIONA MEDIANTE EL HELPER UserSessionHelper **********
        [HttpGet]
        public async Task<ActionResult> GetUserInfo()
        {

            var authCookie = Request.Cookies["AuthToken"];
            if (authCookie == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authCookie.Value);
                var response = await client.GetAsync($"{API_BASE_URL}Auth/GetUserInfo");
                if (!response.IsSuccessStatusCode)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(responseString);
                return Json(userInfo, JsonRequestBehavior.AllowGet);
            }
        }        
    }


    public class TokenResponse
    {
        public string Token { get; set; }
    }

    public class UserInfo
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}