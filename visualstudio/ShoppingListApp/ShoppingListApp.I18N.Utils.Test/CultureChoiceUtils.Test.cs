using System;
using System.Web;
using Moq;
using NUnit.Framework;

namespace ShoppingListApp.I18N.Utils.Test
{
    [TestFixture]
    public class CultureChoiceUtilsTest
    {
        private const string UserCultureCookieName = "UserCultureCookie";
        private const string DefaultCulture = "en-US";
        private string userCultureInCookie;
        private string userCulture;
        private Mock<HttpRequestBase> httpRequestBase;
        private HttpCookie userCultureCookie;
        private HttpCookieCollection cookies;

        [SetUp]
        public void Init()
        {
            httpRequestBase = new Mock<HttpRequestBase>();
            cookies = new HttpCookieCollection();
        }

        [Test]
        public void CultureChosenByUserOnTheWebsiteIsSaved_WhenUserSelectsASupportedCultureFromTheWebsite()
        {
            // Arrange
            userCulture = "fr-FR";
            
            // Act
            CultureChoiceUtils.SaveSupportedCultureChosenByUserInACookie(cookies, userCulture);

            // Assert
            Assert.AreEqual(userCulture, cookies[UserCultureCookieName].Value);
            Assert.AreEqual(DateTime.Now.AddYears(10).Date, cookies[UserCultureCookieName].Expires.Date);
        }

        [Test]
        public void CultureChosenByUserOnTheWebsiteIsNotSaved_WhenCookieCollectionDoesNotExists()
        {
            // Arrange
            userCulture = "fr-FR";

            // Act
            
            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => CultureChoiceUtils.SaveSupportedCultureChosenByUserInACookie(null, userCulture));
        }

        [Test]
        public void CultureChosenByUserOnTheWebsiteIsNotSaved_WhenUserCultureIsInvalidNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => CultureChoiceUtils.SaveSupportedCultureChosenByUserInACookie(cookies, null));
        }

        [Test]
        public void CultureChosenByUserOnTheWebsiteIsNotSaved_WhenUserCultureIsInvalidEmpty()
        {
            // Arrange
            userCulture = string.Empty;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => CultureChoiceUtils.SaveSupportedCultureChosenByUserInACookie(cookies, userCulture));
        }

        [Test]
        public static void NoMatch_WhenInvalidRequest()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(null));
        }

        [Test]
        public void BestMatchIsUserChoice_WhenUserHasChosenASupportedCulture()
        {
            // Arrange
            userCultureInCookie = "de-DE"; 
            userCultureCookie = new HttpCookie(UserCultureCookieName, userCultureInCookie);
            cookies.Add(userCultureCookie);
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(userCultureInCookie, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsDefaultCulture_WhenUserHasChosenAnEmptyCulture()
        {
            // Arrange
            userCultureInCookie = string.Empty;
            userCultureCookie = new HttpCookie(UserCultureCookieName, userCultureInCookie);
            cookies.Add(userCultureCookie);
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsDefaultCulture_WhenUserHasChosenACultureNotSupported()
        {
            // Arrange
            userCultureInCookie = "pl-PL";
            userCultureCookie = new HttpCookie(UserCultureCookieName, userCultureInCookie);
            cookies.Add(userCultureCookie);
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsDefaultCulture_WhenUserHasChosenNoCultureAndBrowserListIsNull()
        {
            // Arrange
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);
            httpRequestBase.Setup(request => request.UserLanguages).Returns((string[])null);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsDefaultCulture_WhenUserHasChosenNoCultureAndBrowserListIsEmpty()
        {
            // Arrange
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);
            string[] userLanguages = { };
            httpRequestBase.Setup(request => request.UserLanguages).Returns(userLanguages);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsDefaultCulture_WhenUserHasChosenNoCultureAndBrowserListHasANoMatch()
        {
            // Arrange
            const string NoMatch = "pl-PL";
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);
            string[] userLanguages = { NoMatch };
            httpRequestBase.Setup(request => request.UserLanguages).Returns(userLanguages);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsPerfectMatch_WhenUserHasChosenNoCultureAndBrowserListHasAPerfectMatchInFirstPosition()
        {
            // Arrange
            const string PerfectMatch = "fr-FR";
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);
            string[] userLanguages = { PerfectMatch };
            httpRequestBase.Setup(request => request.UserLanguages).Returns(userLanguages);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(PerfectMatch, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }

        [Test]
        public void BestMatchIsInvariantMatch_WhenUserHasChosenNoCultureAndBrowserListHasAnInvariantMatchInFirstPosition()
        {
            // Arrange
            const string InvariantMatch = "de-CH";
            const string InvariantMatchsupported = "de-DE";
            const string PerfectMatch = "fr-FR";
            httpRequestBase.Setup(request => request.Cookies).Returns(cookies);
            string[] userLanguages = { InvariantMatch, PerfectMatch };
            httpRequestBase.Setup(request => request.UserLanguages).Returns(userLanguages);

            // Act
            CultureChoiceUtils.SetBestSupportedCultureMatchBetweenUserBrowserAndDefault(httpRequestBase.Object);

            // Assert
            Assert.AreEqual(InvariantMatchsupported, CurrentCultureConfiguration.GetCurrentUICulture.ToString());
            Assert.AreEqual(DefaultCulture, CurrentCultureConfiguration.GetCurrentCulture.ToString());
        }
    }
}
