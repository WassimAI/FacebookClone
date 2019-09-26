﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FacebookClone
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute("default", "", new { controller = "Account", action = "Index" });

            routes.MapRoute("CreateAccount", "Account/CreateAccount", new { controller = "Account", action = "CreateAccount" });

            routes.MapRoute("Account", "{Username}", new { controller = "Account", action = "Username" });
        }
    }
}
