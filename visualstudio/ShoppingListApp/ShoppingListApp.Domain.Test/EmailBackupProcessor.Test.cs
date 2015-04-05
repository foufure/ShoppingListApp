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
    public class EmailBackupProcessorTest
    {
        private Mock<IEmailSettings> emailSettingsMock;
        private string directory;

        [SetUp]
        public void Init()
        {
            this.directory = System.IO.Directory.GetCurrentDirectory() + @"\tests";
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }

            emailSettingsMock = new Mock<IEmailSettings>();
            emailSettingsMock.Setup(x => x.UseSsl).Returns(false);
            emailSettingsMock.Setup(x => x.MailFromAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.MailToAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.Password).Returns("InvalidPassword");
            emailSettingsMock.Setup(x => x.ServerName).Returns("smtp.gmail.com");
            emailSettingsMock.Setup(x => x.ServerPort).Returns(587);
            emailSettingsMock.Setup(x => x.UserName).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.DeliveryMethod).Returns(SmtpDeliveryMethod.SpecifiedPickupDirectory);
            emailSettingsMock.Setup(x => x.PickupDirectoryLocation).Returns(this.directory);
        }

        [TearDown]
        public void Dispose()
        {
            Array.ForEach(Directory.GetFiles(this.directory), File.Delete);
        }

        [Test]
        public void EmailBackupProcessorThrowsException_WhenFileListToBeAttachedIsNull()
        { 
            // Arrange
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act

            // Assert
            Assert.Throws(typeof(NullReferenceException), () => testee.ProcessBackup(null));
        }

        [Test]
        public void EmailBackupProcessorThrowsException_WhenFileToBeAttachedDoesNotExists()
        {
            // Arrange
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.ProcessBackup(new List<string>() { "non_existing_file.xml" }));
        }

        [Test]
        public void EmailBackupProcessorBackupComplete_WhenEmailSent()
        {
            // Arrange
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act
            testee.ProcessBackup(new List<string>() {@"./ItemRepository.example.xml"});
            string[] files = Directory.GetFiles(this.directory, "*.eml");

            // Assert
            Assert.AreEqual(stripUnwantedContentFromEmail(@"./Email_reference.eml"), stripUnwantedContentFromEmail(files[0]));
        }

        private static string stripUnwantedContentFromEmail(string fileToStrip)
        {
            string line;

            StringBuilder referenceString = new StringBuilder();

            using (StreamReader referenceReader = new StreamReader(fileToStrip))
            {
                while ((line = referenceReader.ReadLine()) != null)
                {
                    if (!line.Contains("Date:") && !line.Contains("boundary") && !string.IsNullOrEmpty(line))
                    {
                        referenceString.AppendLine();
                    }
                }
            }

            return referenceString.ToString();
        }
    }
}
