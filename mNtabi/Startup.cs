using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mNtabi.Startup))]
namespace mNtabi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
