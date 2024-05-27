using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Threading.Tasks;
using EconoMeMVC.Controllers;

namespace EconoMeMVC.Helpers
{
    public static class UserSessionHelper
    {
        private static readonly string API_BASE_URL = ConfigurationManager.AppSettings["ApiBaseUrl"];

        public static bool IsUserLoggedIn()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_BASE_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // CON ESQUEMA (EJ: BEARER), para aplicaciones más complejas
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authCookie.Value);
                    // SIN ESQUEMA
                    // client.DefaultRequestHeaders.Add("Authorization", authCookie.Value);
                    // SIN COMPROBACIÓN DE PARÁMETROS DEL HEADER
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authCookie.Value);

                    HttpResponseMessage response = client.GetAsync("Auth/ValidateToken").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static UserInfo GetUserInfo()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API_BASE_URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authCookie.Value);

                    HttpResponseMessage response = client.GetAsync("Auth/GetUserInfo").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        var userInfo = JsonConvert.DeserializeObject<UserInfo>(responseString);
                        return userInfo;
                    }
                }
            }
            return null;
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
