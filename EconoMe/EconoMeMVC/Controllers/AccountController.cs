using EconoMeMVC.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EconoMeMVC.Controllers
{
    // ********** CONTROLADOR ORIGINALMENTE CREADO PARA RECUPERAR DATOS DE USER CON JS ASÍNCRONO **********
    // ********** MÁS EFICIENTE, PERO MÁS QUEBRADEROS DE CABEZA **********
    // ********** ACTUALMENTE EN DESUSO, SE GESTIONA MEDIANTE EL HELPER UserSessionHelper **********
    public class AccountController : Controller
    {

        private readonly string _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

        
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
                var response = await client.GetAsync($"{_apiBaseUrl}/api/Auth/GetUserInfo");
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


    public class UserInfo
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}