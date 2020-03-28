using System.Configuration;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace WebChatBuilderService
{
    public class WebApiStartup
    {
        public void Start()
        {
            var port = ConfigurationManager.AppSettings["ServicePort"];

            var config = new HttpSelfHostConfiguration("http://localhost:" + port);
            config.Routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }
    }
}
