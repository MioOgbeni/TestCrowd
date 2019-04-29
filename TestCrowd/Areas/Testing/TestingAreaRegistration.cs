using System.Web.Mvc;

namespace TestCrowd.Areas.Testing
{
    public class TestingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Testing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Testing_default",
                "Testing/{controller}/{action}/{id}/{attachment}",
                new { controler="Home", action = "Index", id = UrlParameter.Optional, attachment = UrlParameter.Optional },
                namespaces: new[] {"TestCrowd.Areas.Testing.Controllers"}
            );
        }
    }
}