using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShoppingListApp.Web.UI.Startup))]

namespace ShoppingListApp.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}