using System.Web.Mvc;
using System.Web.Routing;

namespace WebchatBuilder
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "WebChat",
                url: "webchat.js",
                defaults: new { controller = "Chat", action = "RenderJavascript" }
            );

            routes.MapRoute(
                name: "Profiles",
                url: "Profiles/{action}",
                defaults: new { controller = "Profiles", action = "GetProfiles" }
            );

            routes.MapRoute(
                name: "Chat",
                url: "Chat/{action}",
                defaults: new { controller = "Chat", action = "StandardChat" }
            );

            routes.MapRoute(
                name: "Settings",
                url: "Settings/{action}",
                defaults: new { controller = "Settings", action = "Settings" }
            );

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Account_Default",
                url: "Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Edit",
                url: "Account/Edit",
                defaults: new { controller = "Account", action = "Edit" }
            );

            routes.MapRoute(
                name: "Dashboard",
                url: "Dashboard",
                defaults: new { controller = "Home", action = "Dashboard" }
            );

            routes.MapRoute(
                name: "Users",
                url: "Users",
                defaults: new { controller = "Home", action = "Users" }
            );

            routes.MapRoute(
                name: "AboutWcb",
                url: "AboutWcb",
                defaults: new { controller = "Home", action = "AboutWcb" }
            );

            routes.MapRoute(
                name: "Default_Action",
                url: "{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
