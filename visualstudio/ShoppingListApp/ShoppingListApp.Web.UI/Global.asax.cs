using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ShoppingListApp.I18N.Utils;
using ShoppingListApp.JobsScheduler;
using NLog;

namespace ShoppingListApp.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Logger AppStartLogger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AppStartLogger.Trace("Start App Cron JobScheduler: " + DateTime.Now.ToString());

            CronJobsScheduler.InitializeJobScheduler();

            AppStartLogger.Trace("App Cron JobScheduler Started: " + DateTime.Now.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed.")]
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(((HttpApplication)source).Context.Request);
        }
    }
}
