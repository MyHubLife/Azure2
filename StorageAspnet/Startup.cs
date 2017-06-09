using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StorageAspnet.Startup))]
namespace StorageAspnet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
