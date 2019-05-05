using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppASV.Startup))]
namespace AppASV
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
