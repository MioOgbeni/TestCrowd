using System.Web.Mvc;
using System.Web.Routing;

namespace TestCrowd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Public", action = "Index", id = UrlParameter.Optional },
                namespaces: new []{"TestCrowd.Controllers"}
            );

            routes.MapRoute(
                name: "Profile",
                url: "{controller}/{action}/{role}/{username}",
                defaults: new { controller = "Public", action = "Index", role = UrlParameter.Optional, username = UrlParameter.Optional },
                namespaces: new[] { "TestCrowd.Areas.Testing.Controllers" }
            );
        }
    }
}
