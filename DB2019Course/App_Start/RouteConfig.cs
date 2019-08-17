using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DB2019Course
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Groups", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Articles",
                url: "{controller}/{action}/{author}/{id}",
                defaults: new { controller = "Articles", action = "Index", author = UrlParameter.Optional,id = UrlParameter.Optional }
            ); //дополнительный роут для двух параметров
        }
    }
}
