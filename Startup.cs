using NISA.DSSelfAPI.WebApi.Handlers;
using Owin;
using System;
using System.Web.Http;
using System.Threading.Tasks;

namespace NISA.DSSelfAPI
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
            config.MessageHandlers.Add(new CorsHandler());
            appBuilder.UseWebApi(config);

            
        }
    }


}
