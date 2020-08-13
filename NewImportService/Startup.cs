using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NewImportService.Startup))]
namespace NewImportService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
