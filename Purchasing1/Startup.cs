using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Purchasing1.Startup))]
namespace Purchasing1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
