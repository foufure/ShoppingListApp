using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium_SpecFlow.Support
{
    public abstract class SeleniumStepsBase
    {
        protected IWebDriver selenium
        {
            get { return SeleniumController.Instance.Selenium; }
        }
    }
}
