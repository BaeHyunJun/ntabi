using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NDayTrip.Startup))]
namespace NDayTrip
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
