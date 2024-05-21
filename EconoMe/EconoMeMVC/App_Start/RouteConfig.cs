using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EconoMeMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ruta personalizada
            routes.MapRoute(
                name: "DashboardByYear",
                url: "Home/Dashboard/{year}",
                defaults: new { controller = "Home", action = "Dashboard" },
                constraints: new { year = @"\d{4}" } // Solo acepta años de 4 dígitos
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
