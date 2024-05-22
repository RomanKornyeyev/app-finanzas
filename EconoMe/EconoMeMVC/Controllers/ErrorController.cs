using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EconoMeMVC.Controllers
{
    public class ErrorController : Controller
    {
        // 404 NOT FOUND
        public ActionResult NotFound()
        {
            return View();
        }
    }
}