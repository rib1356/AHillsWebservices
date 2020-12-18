using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Purchasing.Startup))]
namespace Purchasing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
