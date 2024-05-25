using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EconoMeMVC.Helpers
{
    public static class UserSessionHelper
    {
        private static readonly string ApiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

        public static bool IsUserLoggedIn()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiBaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // CON ESQUEMA (EJ: BEARER), para aplicaciones más complejas
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authCookie.Value);
                    // SIN ESQUEMA
                    // client.DefaultRequestHeaders.Add("Authorization", authCookie.Value);
                    // SIN COMPROBACIÓN DE PARÁMETROS DEL HEADER
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authCookie.Value);

                    HttpResponseMessage response = client.GetAsync("api/Auth/ValidateToken").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task<UserInfo> GetUserInfoAsync(HttpRequestBase request)
        {
            HttpCookie authCookie = request.Cookies["AuthToken"];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authCookie.Value);

                var response = await client.GetAsync("api/Auth/GetUserInfo");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfo>(responseString);
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
