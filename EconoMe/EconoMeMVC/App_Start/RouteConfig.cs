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

            // Ruta personalizada (MES)
            routes.MapRoute(
                name: "DashboardByYearAndMonth",
                url: "Home/Dashboard/{year}/{month}",
                defaults: new { controller = "Home", action = "Dashboard", year = UrlParameter.Optional, month = UrlParameter.Optional },
                constraints: new { year = @"\d{4}", month = @"\d{1,2}" } // Ensure year is 4 digits and month is 1 or 2 digits
            );

            // Ruta personalizada (AÑO)
            routes.MapRoute(
                name: "DashboardByYear",
                url: "Home/Dashboard/{year}",
                defaults: new { controller = "Home", action = "Dashboard", year = UrlParameter.Optional },
                constraints: new { year = @"\d{4}" } // Ensure year is 4 digits
            );

            // Ruta personalizada (GLOBAL)
            routes.MapRoute(
                name: "Dashboard",
                url: "Home/Dashboard",
                defaults: new { controller = "Home", action = "Dashboard" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Ruta de fallback para manejar 404
            routes.MapRoute(
                name: "Error",
                url: "{*url}",
                defaults: new { controller = "Error", action = "NotFound" }
            );
        }
    }
}
