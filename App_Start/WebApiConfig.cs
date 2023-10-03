using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.Cors;

namespace jbar.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string origin = "https://jbar.iran.liara.run";
            EnableCorsAttribute cors = new EnableCorsAttribute(origin, "*", "GET,POST");
            config.EnableCors(cors);
            
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi0",
                routeTemplate: "api/modelList/{*name}",
                defaults: new { controller = "app", action = "getModel", name = UrlParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           
        }
    }
}