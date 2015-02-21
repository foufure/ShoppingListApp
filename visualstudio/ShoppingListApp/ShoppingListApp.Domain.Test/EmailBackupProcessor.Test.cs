using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.Domain.Abstract;
using Moq;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public class EmailBackupProcessorTest
    {
        public Mock<IEmailSettings> emailSettingsMock;

        [SetUp]
        public void Init()
        {
            emailSettingsMock = new Mock<IEmailSettings>();
            emailSettingsMock.Setup(x => x.UseSsl).Returns(true);
            emailSettingsMock.Setup(x => x.MailFromAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.MailToAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.Password).Returns("InvalidPassword");
            emailSettingsMock.Setup(x => x.ServerName).Returns("smtp.gmail.com");
            emailSettingsMock.Setup(x => x.ServerPort).Returns(587);
            emailSettingsMock.Setup(x => x.UserName).Returns("shoppinglistappharbor@gmail.com");
        }

        [Test]
        [ExpectedException(typeof(System.FormatException))]
        public void ItemRepositoryContainsItemsFromPersistentRepository_WhenReadFromXmlFileRepository()
        {
            // Arrange
            emailSettingsMock.Setup(x => x.MailFromAddress).Returns("InvalidEmail");
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act
            testee.ProcessBackup(@"./ItemRepository.example.xml");

            // Assert
        }
    }
}
