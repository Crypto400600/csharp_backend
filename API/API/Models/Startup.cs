using System;
using System.Threading.Tasks;
using Owin;
using Engine;
using Hangfire;
using Hangfire.MemoryStorage;

namespace API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Connector.GenerateDb();
            Console.WriteLine("db initialized");

            GlobalConfiguration.Configuration.UseMemoryStorage();
            
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}