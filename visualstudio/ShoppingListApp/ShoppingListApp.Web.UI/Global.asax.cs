using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using NLog;
using ShoppingListApp.I18N.Utils;
using ShoppingListApp.JobsScheduler;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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

            ConfigureAndStartCronJobs();

            AppStartLogger.Trace("App Cron JobScheduler Started: " + DateTime.Now.ToString());
        }

        private void ConfigureAndStartCronJobs()
        {
            CronJobsScheduler cronJobsScheduler = new CronJobsScheduler(DependencyResolver.Current.GetService<ISchedulerFactory>(), DependencyResolver.Current.GetService<IJobFactory>());
            cronJobsScheduler.StartJobScheduler();

            // @"0 5 8,10,12,18,20 * * ?" - every day at 8:05, 10:05 ... + @"0 * * ? * *" - every minute
            cronJobsScheduler.AddJob(@"0 5 8,10,12,18,20 * * ?", JobBuilder.Create<BackupAllJob>().Build());
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
