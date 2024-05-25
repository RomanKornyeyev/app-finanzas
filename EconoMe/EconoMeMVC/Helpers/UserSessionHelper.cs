using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using static System.Net.WebRequestMethods;

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
    }
}
