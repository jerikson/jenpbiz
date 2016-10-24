using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jenpbiz.Startup))]
namespace Jenpbiz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
        }
    }
}
