using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Ionic.Zip;
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
        private Mock<IUserInformation> userInformationMock;
        private Mock<IDataPathProvider> dataPathProviderMock;
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

            userInformationMock = new Mock<IUserInformation>();
            dataPathProviderMock = new Mock<IDataPathProvider>();
        }

        [TearDown]
        public void Dispose()
        {
            Thread.Sleep(10); // otherwise access to filesystem is too fast and creates access denied
            Array.ForEach(Directory.GetFiles(this.directory), File.Delete);
            
            if (File.Exists(@"./backup/admin.backup"))
            {
                File.Delete(@"./backup/admin.backup");
            }

            if (File.Exists(@"./backup/username.backup"))
            {
                File.Delete(@"./backup/username.backup");
            }

            foreach (string fileToDelete in Directory.GetFiles(@"./restore", "*.xml"))
            {
                File.Delete(fileToDelete);
            }
        }

        [Test]
        public void EmailBackupProcessorCreatesUserSpecificBackup_WhenUserIsLoggedInAndIsNotAdministrator()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./backup");
            userInformationMock.Setup(x => x.UserName).Returns("username");
            userInformationMock.Setup(x => x.IsAdmin).Returns(false);
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);

            // Act
            testee.CreateBackup();

            // Assert
            Assert.IsTrue(File.Exists(@"./backup/username.backup"));
        }

        [Test]
        public void EmailBackupProcessorCreatesConsistentUserSpecificBackup_WhenUserIsLoggedInAndIsNotAdministrator()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./backup");
            userInformationMock.Setup(x => x.UserName).Returns("username");
            userInformationMock.Setup(x => x.IsAdmin).Returns(false);
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);

            List<string> listOfExpectedFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(@"./backup", "*username.xml"))
            {
                listOfExpectedFiles.Add(filePath.Replace("./backup\\", string.Empty));
            }

            // Act
            testee.CreateBackup();
            
            // Assert
            using (ZipFile backupZip = ZipFile.Read(@"./backup/username.backup"))
            {
                Assert.AreEqual(listOfExpectedFiles, backupZip.EntryFileNames);
            } 
        }

        [Test]
        public void EmailBackupProcessorCreatesConsistentFullBackup_WhenUserIsLoggedInAndIsAdministrator()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./backup");
            userInformationMock.Setup(x => x.UserName).Returns("admin");
            userInformationMock.Setup(x => x.IsAdmin).Returns(true);
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);

            List<string> listOfExpectedFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(@"./backup", "*.*"))
            {
                listOfExpectedFiles.Add(filePath.Replace("./backup\\", string.Empty));
            }

            // Act
            testee.CreateBackup();

            // Assert
            using (ZipFile backupZip = ZipFile.Read(@"./backup/admin.backup"))
            {
                Assert.AreEqual(listOfExpectedFiles, backupZip.EntryFileNames);
            } 
        }

        [Test]
        public void EmailBackupProcessorRestoresConsistentFullBackup_WhenUserIsLoggedInAndIsAdministrator()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./restore");
            userInformationMock.Setup(x => x.UserName).Returns("admin");
            userInformationMock.Setup(x => x.IsAdmin).Returns(true);
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);

            // Act
            testee.RestoreBackup(@"./restore/admin.backup");

            // Assert
            List<string> listOfActualFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(@"./restore", "*.*"))
            {
                listOfActualFiles.Add(filePath.Replace("./restore\\", string.Empty));
            }

            listOfActualFiles.Remove("admin.backup");

            using (ZipFile backupZip = ZipFile.Read(@"./restore/admin.backup"))
            {
                List<string> entriesOfZipFile = new List<string>(backupZip.EntryFileNames);
                entriesOfZipFile.Remove("mylog.log");

                Assert.AreEqual(entriesOfZipFile, listOfActualFiles);
            }
        }

        [Test]
        public void EmailBackupProcessorRestoresConsistentUserSpecificBackup_WhenUserIsLoggedInAndIsAdministrator()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./restore");
            userInformationMock.Setup(x => x.UserName).Returns("username");
            userInformationMock.Setup(x => x.IsAdmin).Returns(false);
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);
            
            // Act
            testee.RestoreBackup(@"./restore/admin.backup");

            // Assert
            List<string> listOfActualFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(@"./restore", "*.*"))
            {
                listOfActualFiles.Add(filePath.Replace("./restore\\", string.Empty));
            }

            listOfActualFiles.Remove("admin.backup");

            List<string> listOfExpectedFiles = new List<string>();
            listOfExpectedFiles.Add("CategoryRepository.username.xml");
            listOfExpectedFiles.Add("ItemRepository.username.xml");
            listOfExpectedFiles.Add("ShoppingListRepository.username.xml");

            Assert.AreEqual(listOfExpectedFiles, listOfActualFiles);
        }

        [Test]
        public void EmailBackupProcessorThrowsException_WhenFileToBeAttachedDoesNotExists()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./");
            userInformationMock.Setup(x => x.UserName).Returns("username");
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.SecureBackup());
        }

        [Test]
        public void EmailBackupProcessorBackupComplete_WhenEmailSent()
        {
            // Arrange
            dataPathProviderMock.Setup(x => x.DataPath).Returns(@"./");
            userInformationMock.Setup(x => x.UserName).Returns("username");
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object, userInformationMock.Object, dataPathProviderMock.Object);
            testee.CreateBackup();

            // Act
            testee.SecureBackup();
            string[] files = Directory.GetFiles(this.directory, "*.eml");

            // Assert
            Assert.AreEqual(StripUnwantedContentFromEmail(@"./Email_reference.eml"), StripUnwantedContentFromEmail(files[0]));
        }

        private static string StripUnwantedContentFromEmail(string fileToStrip)
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
