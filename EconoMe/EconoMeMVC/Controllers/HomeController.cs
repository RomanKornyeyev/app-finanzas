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
using EconoMeMVC.Helpers;


namespace EconoMeMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Movements()
        {
            return View();
        }

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
    }
}