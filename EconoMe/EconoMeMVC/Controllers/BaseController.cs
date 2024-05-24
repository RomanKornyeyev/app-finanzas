using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace EconoMeMVC.Controllers
{
    public class BaseController : Controller
    {
        private readonly string ApiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

        protected bool IsUserLoggedIn()
        {
            HttpCookie authCookie = Request.Cookies["AuthToken"];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiBaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authCookie.Value);

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