using System;
using NUnit.Framework;
using Selenium_SpecFlow.Support;
using TechTalk.SpecFlow;

namespace ShoppingListApp.Web.UI.Test.Steps
{
    [Binding]
    public class US001_LogInAsUser_Steps : SeleniumStepsBase
    {
        [Given(@"I have '(.*)' page opened")]
        public void GivenIHavePageOpened(string p0)
        {
            selenium.NavigateTo(p0);
        }
        
        [Given(@"I have entered '(.*)' into the text box of which id is '(.*)'")]
        public void GivenIHaveEnteredIntoTheTextBoxOfWhichIdIs(string p0, string p1)
        {
            selenium.SetTextBoxValue(p1,p0);
        }

        [Given(@"I have pressed button '(.*)'")]
        [When(@"I press button of which name is '(.*)'")]
        public void WhenIPressButtonOfWhichNameIs(string p0)
        {
            selenium.ClickButton(p0);
        }
        
        [Then(@"I should be navigated to home page with '(.*)' on it")]
        public void ThenIShouldBeNavigatedToSearchResultPageWithOnIt(string p0)
        {
            Assert.Greater(selenium.FindElements(OpenQA.Selenium.By.PartialLinkText(p0)).Count,0);
        }
    }
}
