using System.Web.Mvc;
using System.Web.Routing;

namespace proyectoACME
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "LlenarEncuesta",
                url: "Publico/Llenar/{token}",
                defaults: new { controller = "Publico", action = "Llenar", token = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}