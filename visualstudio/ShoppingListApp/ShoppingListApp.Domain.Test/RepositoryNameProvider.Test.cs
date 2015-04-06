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
    public class RepositoryNameProviderTest
    {
        private GoogleUserInformation googleUserInformationInvalid;
        private Mock<IUserInformation> userInformationValid;
        private Mock<IDataPathProvider> dataPathProvider;

        [SetUp]
        public void Init()
        {
            googleUserInformationInvalid = new GoogleUserInformation();
            userInformationValid = new Mock<IUserInformation>();
            userInformationValid.Setup(x => x.UserName).Returns("ValidUser");
            dataPathProvider = new Mock<IDataPathProvider>();
            dataPathProvider.Setup(x => x.DataPath).Returns(@"C:\myPath");
        }

        [Test]
        public void ValidCategoryName_WhenLoggedOn()
        {
            // Arrange
            CategoryXmlRepositoryName testee = new CategoryXmlRepositoryName(userInformationValid.Object, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == @"C:\myPath\CategoryRepository.ValidUser.xml");   
        }

        [Test]
        public void InvalidCategoryName_WhenNotLoggedOn()
        {
            // Arrange
            CategoryXmlRepositoryName testee = new CategoryXmlRepositoryName(googleUserInformationInvalid, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == null);
        }

        [Test]
        public void ValidItemName_WhenLoggedOn()
        {
            // Arrange
            ItemXmlRepositoryName testee = new ItemXmlRepositoryName(userInformationValid.Object, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == @"C:\myPath\ItemRepository.ValidUser.xml");
        }

        [Test]
        public void InvalidItemName_WhenNotLoggedOn()
        {
            // Arrange
            ItemXmlRepositoryName testee = new ItemXmlRepositoryName(googleUserInformationInvalid, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == null);
        }

        [Test]
        public void ValidShoppingListName_WhenLoggedOn()
        {
            // Arrange
            ShoppingListXmlRepositoryName testee = new ShoppingListXmlRepositoryName(userInformationValid.Object, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == @"C:\myPath\ShoppingListRepository.ValidUser.xml");
        }

        [Test]
        public void InvalidShoppingListName_WhenNotLoggedOn()
        {
            // Arrange
            ShoppingListXmlRepositoryName testee = new ShoppingListXmlRepositoryName(googleUserInformationInvalid, dataPathProvider.Object);

            // Act

            // Assert
            Assert.True(testee.RepositoryName == null);
        }
    }
}
