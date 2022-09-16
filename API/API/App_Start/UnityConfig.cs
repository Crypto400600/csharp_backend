using System.Web.Http;
using API.Controllers;
using Hangfire;
using Unity;
using Unity.WebApi;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<ApiController, IndexController>();
            container.RegisterType<ApiController, QueryController>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}