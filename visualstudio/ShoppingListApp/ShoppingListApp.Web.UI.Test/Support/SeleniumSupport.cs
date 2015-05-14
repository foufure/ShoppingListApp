using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Selenium_SpecFlow.Support
{
    [Binding]
    public static class SeleniumSupport
    {
        private static bool ReuseWebSession
        {
            get { return ConfigurationManager.AppSettings["ReuseWebSession"] == "true"; }
        }

        [BeforeScenario("web")]
        public static void BeforeWebScenario()
        {
            SeleniumController.Instance.Start();
        }

        [AfterScenario("web")]
        public static void AfterWebScenario()
        {
            if (!ReuseWebSession)
                SeleniumController.Instance.Stop();
        }

        //[AfterFeature]
        [AfterTestRun]
        public static void AfterWebFeature()
        {
            SeleniumController.Instance.Stop();
        }
    }
}
