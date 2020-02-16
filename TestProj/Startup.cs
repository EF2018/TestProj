using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestProj.Startup))]
namespace TestProj
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
