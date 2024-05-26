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
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> GetUserInfo()
        {
            var userInfo = await UserSessionHelper.GetUserInfoAsync(Request);

            if (userInfo == null)
            {
                return Json(new { success = false, message = "No se pudo obtener la información del usuario." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = userInfo }, JsonRequestBehavior.AllowGet);
        }

        private readonly string _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
    }

}