using System.Web;
using System.Web.Optimization;

namespace EconoMeMVC
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/Custom_CSS/js-global").Include(
                      "~/Content/Custom_JS/header.js"));

            bundles.Add(new StyleBundle("~/Content/Custom_CSS/css").Include(
                      "~/Content/Custom_CSS/buttons.css",
                      "~/Content/Custom_CSS/global.css"));

            bundles.Add(new StyleBundle("~/Content/Custom_CSS/css-index").Include(
                      "~/Content/Custom_CSS/index.css"));
        }
    }
}
