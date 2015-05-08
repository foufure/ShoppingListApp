using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.I18N.Resources.Views.Home;
using ShoppingListApp.Web.UI.Controllers;

namespace ShoppingListApp.Web.Test
{
    [TestFixture]
    public class ControllerTest
    {
        private Mock<IBackupProcessor> mockBackupProcessor;

        private Mock<IDataPathProvider> mockDataPathProvider;
        private Mock<IItemsRepository> mockItemsRepository;
        private Mock<IShoppingListRepository> mockShoppingListRepository;
        private Mock<ICategoryRepository> mockCategoryRepository;

        private Mock<HttpPostedFileBase> mockHttpPostedFile;

        [SetUp]
        public void Init()
        {
            mockBackupProcessor = new Mock<IBackupProcessor>();

            mockDataPathProvider = new Mock<IDataPathProvider>();
            mockItemsRepository = new Mock<IItemsRepository>();
            mockShoppingListRepository = new Mock<IShoppingListRepository>();
            mockCategoryRepository = new Mock<ICategoryRepository>();

            mockHttpPostedFile = new Mock<HttpPostedFileBase>();
        }

        [TearDown]
        public static void Dispose()
        {
        }

        [Test]
        public void BackupFailsWithNoFilesToBackupMessage_WhenBackupProcessorFails()
        {
            // Arrange
            mockBackupProcessor.Setup(processor => processor.SecureBackup()).Throws(new System.ArgumentNullException());

            // Act
            AdminController controller = new AdminController(mockBackupProcessor.Object, mockDataPathProvider.Object, mockItemsRepository.Object, mockShoppingListRepository.Object, mockCategoryRepository.Object);
            RedirectToRouteResult testee = controller.Backup();

            // Assert
            Assert.AreEqual(ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToBackup, controller.TempData["backup"]);
        }

        [Test]
        public void BackupFailsWithDeliveryFailedMessage_WhenBackupProcessorFails()
        {
            // Arrange
            mockBackupProcessor.Setup(processor => processor.SecureBackup()).Throws(new System.Net.Mail.SmtpFailedRecipientsException());

            // Act
            AdminController controller = new AdminController(mockBackupProcessor.Object, mockDataPathProvider.Object, mockItemsRepository.Object, mockShoppingListRepository.Object, mockCategoryRepository.Object);
            RedirectToRouteResult testee = controller.Backup();

            // Assert
            Assert.AreEqual(ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DeliveryFailed, controller.TempData["backup"]);
        }  

        [Test]
        public void BackupFailsWithConnectionFailedMessage_WhenBackupProcessorFails()
        {
            // Arrange
            mockBackupProcessor.Setup(processor => processor.SecureBackup()).Throws(new System.Net.Mail.SmtpException());

            // Act
            AdminController controller = new AdminController(mockBackupProcessor.Object, mockDataPathProvider.Object, mockItemsRepository.Object, mockShoppingListRepository.Object, mockCategoryRepository.Object);
            RedirectToRouteResult testee = controller.Backup();

            // Assert
            Assert.AreEqual(ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.ConnectionFailed, controller.TempData["backup"]);
        }

        [Test]
        public void RestoreBackupFailsWithFailureMessage_WhenBackupCouldNotBeRestored()
        {
            // Arrange
            mockBackupProcessor.Setup(processor => processor.RestoreBackup(It.IsNotNull<string>())).Throws(new System.Exception());
            mockHttpPostedFile.Setup(file => file.FileName).Returns("test.txt");
            mockHttpPostedFile.Setup(file => file.ContentLength).Returns(20);
            mockHttpPostedFile.Setup(file => file.SaveAs(It.IsNotNull<string>()));

            // Act
            AdminController controller = new AdminController(mockBackupProcessor.Object, mockDataPathProvider.Object, mockItemsRepository.Object, mockShoppingListRepository.Object, mockCategoryRepository.Object);
            RedirectToRouteResult testee = controller.RestoreBackup(mockHttpPostedFile.Object);

            // Assert
            Assert.AreEqual(ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure, controller.TempData["restoreBackup"]);
        }
    }
}