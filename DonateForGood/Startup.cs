using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DonateForGood.Startup))]
namespace DonateForGood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
