using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ShoppingListApp.i18n.Utils;

namespace ShoppingListApp.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private CultureHelper cultureHelper;

        public MvcApplication()
        {
            cultureHelper = new CultureHelper();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            cultureHelper.ApplyUserCulture(((HttpApplication)source).Context.Request);
        }
    }
}
