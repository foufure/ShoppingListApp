using System;
using NUnit.Framework;
using Selenium_SpecFlow.Support;
using TechTalk.SpecFlow;

namespace $rootnamespace$.Steps
{
    [Binding]
    public class US001_GoogleSearchSteps : SeleniumStepsBase
    {
        [Given(@"I have '(.*)' page opened")]
        public void GivenIHavePageOpened(string p0)
        {
            selenium.NavigateTo(string.Empty);
        }
        
        [Given(@"I have entered '(.*)' into the text box of which id is '(.*)'")]
        public void GivenIHaveEnteredIntoTheTextBoxOfWhichIdIs(string p0, string p1)
        {
            selenium.SetTextBoxValue(p1,p0);
        }
        
        [When(@"I press button of which id is '(.*)'")]
        public void WhenIPressButtonOfWhichIdIs(string p0)
        {
            selenium.ClickButton("gbqfb");
        }

        
        [Then(@"I should be navigated to search result page with '(.*)' on it")]
        public void ThenIShouldBeNavigatedToSearchResultPageWithOnIt(string p0)
        {
            Assert.Greater(selenium.FindElements(OpenQA.Selenium.By.PartialLinkText(p0)).Count,0);
        }
    }
}
