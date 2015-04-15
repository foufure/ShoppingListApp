using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using NLog;
using ShoppingListApp.I18N.Utils;
using ShoppingListApp.JobsScheduler;

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

            CronJobsScheduler cronJobsScheduler = new CronJobsScheduler(DependencyResolver.Current.GetService<IKernel>());
            cronJobsScheduler.InitializeJobScheduler(string.Empty);

            AppStartLogger.Trace("App Cron JobScheduler Started: " + DateTime.Now.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed.")]
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // To be able to mock HttpRequest, etc... the SystemWeb.Abstractions references must be added and the wrappers used.
            // http://www.codemerlin.com/2011/07/mocking-httpcontext-httpresponse-httprequest-httpsessionstate-etc-in-asp-net/
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(new HttpRequestWrapper(((HttpApplication)source).Context.Request));
        }
    }
}
