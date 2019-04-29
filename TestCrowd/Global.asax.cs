using System.Web.Mvc;
using System.Web.Routing;

namespace TestCrowd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DataTables.AspNet.Mvc5.Configuration.RegisterDataTables();
        }
    }
}
