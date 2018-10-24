using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NDayTrip_PC.Startup))]
namespace NDayTrip_PC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
