using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using Moq;
using NUnit.Framework;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public class EmailSettingsTest
    {
        private GoogleUserInformation googleUserInformation;
        private GoogleEmailSettings googleEmailSettings;

        [SetUp]
        public void Init()
        {
            googleUserInformation = new GoogleUserInformation();
            googleEmailSettings = new GoogleEmailSettings(googleUserInformation);
        }

        [TearDown]
        public void Dispose()
        {

        }

        [Test]
        public void UserNameIsNull_WhenNotLoggedOn()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(googleUserInformation.UserName == null);   
        }

        [Test]
        public void UserEmailIsNull_WhenNotLoggedOn()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(googleUserInformation.UserEmail == null);
        }

        [Test]
        public void MailToAddressIsDefault_WhenNotLoggedOn()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(googleEmailSettings.MailToAddress == "shoppinglistappharbor@gmail.com");
        }

        [Test]
        public void MailToAddressIsUserEmail_WhenLoggedOn()
        {
            // Arrange
            Mock<IUserInformation> userInformation = new Mock<IUserInformation>();
            userInformation.Setup(x => x.UserEmail).Returns("test@test.com");
            GoogleEmailSettings googleEmailSettingsWithCustomUserInformation = new GoogleEmailSettings(userInformation.Object);
            // Act

            // Assert
            Assert.True(googleEmailSettingsWithCustomUserInformation.MailToAddress == "test@test.com");
        }
    }
}
