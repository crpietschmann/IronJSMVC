// Copyright (c) 2011 Chris Pietschmann - http://pietschsoft.com
// All rights reserved

using System.Web.Mvc;
using System.Web.Routing;
using IronJSMVC.ViewEngine;

namespace IronJSMVCWebsite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*alljs}", new { allaspx = @".*\.js(/.*)?" });
            routes.IgnoreRoute("{*allcss}", new { allcss = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*allpng}", new { allpng = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*allicon}", new { allicon = @".*\.ico(/.*)?" });

            routes.MapRoute(
                "IronJSController", // Route name
                "{controllerName}/{action}/{id}", // URL with parameters
                new { controller = "IronJSMVC", action = "Index", controllerName = "Home", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Add(new JavaScriptViewEngine());
        }
    }
}