using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Test 
{    
    [TestFixture]
    public class CategoryXmlRepositoryTest 
    {
        private Mock<IRepositoryNameProvider> repositoryNameProvider;
        private Mock<IDataPathProvider> dataPathProvider;

        [SetUp]
        public void Init()
        {
            File.Copy(@"./CategoryRepository.example.xml", @"./CategoryRepository.example.orig.xml");
            File.Copy(@"./CategoryRepository.invalid.xml", @"./CategoryRepository.invalid.orig.xml");
            File.Copy(@"./CategoryRepository.invalidempty.xml", @"./CategoryRepository.invalidempty.orig.xml");
            File.Copy(@"./CategoryRepository.empty.xml", @"./CategoryRepository.empty.orig.xml");
            File.Copy(@"./CategoryRepository.invalidxsd.xml", @"./CategoryRepository.invalidxsd.orig.xml");
            this.repositoryNameProvider = new Mock<IRepositoryNameProvider>();
            this.dataPathProvider = new Mock<IDataPathProvider>();
            this.dataPathProvider.Setup(provider => provider.DataPath).Returns(@"./");
        }

        [TearDown]
        public static void Dispose()
        {
            Thread.Sleep(10); // otherwise access to filesystem is too fast and creates access denied

            File.Delete(@"./CategoryRepository.example.xml");
            File.Copy(@"./CategoryRepository.example.orig.xml", @"./CategoryRepository.example.xml");
            File.Delete(@"./CategoryRepository.example.orig.xml");

            File.Delete(@"./CategoryRepository.invalid.xml");
            File.Copy(@"./CategoryRepository.invalid.orig.xml", @"./CategoryRepository.invalid.xml");
            File.Delete(@"./CategoryRepository.invalid.orig.xml");

            File.Delete(@"./CategoryRepository.invalidempty.xml");
            File.Copy(@"./CategoryRepository.invalidempty.orig.xml", @"./CategoryRepository.invalidempty.xml");
            File.Delete(@"./CategoryRepository.invalidempty.orig.xml");

            File.Delete(@"./CategoryRepository.empty.xml");
            File.Copy(@"./CategoryRepository.empty.orig.xml", @"./CategoryRepository.empty.xml");
            File.Delete(@"./CategoryRepository.empty.orig.xml");

            File.Delete(@"./CategoryRepository.invalidxsd.xml");
            File.Copy(@"./CategoryRepository.invalidxsd.orig.xml", @"./CategoryRepository.invalidxsd.xml");
            File.Delete(@"./CategoryRepository.invalidxsd.orig.xml");

            File.Delete(@"./CategoryRepository.doesnotexists.xml");
        }

        [Test]
        [SetUICulture("en-US")]
        public void CategoryRepositoryIsResetToDefaultUS_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.invalidxsd.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("fr-FR")]
        public void CategoryRepositoryIsResetToDefaultFR_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.invalidxsd.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository_fr.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("de-DE")]
        public void CategoryRepositoryIsResetToDefaultDE_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.invalidxsd.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository_de.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void CategoryRepositoryContainsCategoriesFromPersistentRepository_WhenReadFromXmlFileRepository() 
        {        
            // Arrange
            IEnumerable<string> expectedResult = new List<string>()
            {
                "Category1",
                "Category2",
                "Category3",
                "Category4",
                "Category5"
            };

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(5, testee.Count());
            Assert.AreEqual(expectedResult.AsEnumerable(), testee.AsEnumerable());
            Assert.AreEqual(expectedResult.AsEnumerable(), testee.AsEnumerable());
        }

        [Test]
        public void CategoryRepositoryIsEmpty_WhenReadFromEmptyXmlFileRepository()
        {
            // Arrange
            IEnumerable<string> expectedResult = new List<string>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.empty.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.AsEnumerable(), testee.AsEnumerable());
            Assert.AreEqual(expectedResult.AsEnumerable(), testee.AsEnumerable());
        }

        [Test]
        [SetUICulture("en-US")]
        public void CategoryRepositoryIsSetToDefaults_WhenReadFromNewlyCreatedXmlFileRepository_WhichDidNotExistPreviously()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.doesnotexists.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.doesnotexists.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void CategoryRepositoryIsSetToDefaults_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidXmlFile()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.invalid.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.invalid.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void CategoryRepositoryIsSetToDefaults_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidEmptyXmlFile()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.invalidempty.xml");

            // Act
            IEnumerable<string> testee = (new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.invalidempty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void CategoryRepositoryIsEmpty_WhenNoPersistentRepositoryNameProviderIsAvailable()
        {
            // Arrange
            CategoryXmlRepository testee = null;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new CategoryXmlRepository(null, dataPathProvider.Object));
        }

        [Test]
        public void CategoryRepositoryIsEmpty_WhenNoRepositoryNameIsAvailable()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns((string)null);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void CategoryRepositoryIsEmpty_WhenEmptyRepositoryNameIsAvailable()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(string.Empty);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void CategoriesAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Add("Category6");
            testee.Save();
            testee.Add("Category7");
            testee.Save();
            testee.Add("Category8");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.Add.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void FirstCategoryAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.empty.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Add("Category15");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.AddFirst.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.empty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ExistingCategoryNotAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Add("Category1");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.orig.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void InvalidCategoryNameThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            
            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Add(string.Empty));
        }

        [Test]
        public void NullCategoryNameThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Add(null));
        }

        [Test]
        public void CategoryRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Remove("Category3"); // Remove Category 3
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.Remove.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingCategoryRemovedFromPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.empty.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Remove("NonExistingCategory"));
        }

        [Test]
        public void EmptyCategoryRemovedFromPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.empty.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Remove(string.Empty));
        }

        [Test]
        public void CategoryModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Modify("Category4", "Category12");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.Modified.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingCategoryModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            string itemName = "Category12";
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.empty.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify("Category00", itemName));
        }

        [Test]
        public void InvalidCategoryNewNameModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify("Category1", null));
        }

        [Test]
        public void InvalidCategoryOldNameModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            CategoryXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify(null, "Category1"));
        }

        [Test]
        public void CategoryRepositoryIsEmpty_WhenResetToEmpty()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            CategoryXmlRepository testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetToEmpty();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./CategoryRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void CategoryRepositoryContainsDefaults_WhenResetToDefaults()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./CategoryRepository.example.xml");

            // Act
            CategoryXmlRepository testee = new CategoryXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetToDefault();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultCategoryRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./CategoryRepository.example.xml").Replace("\r\n", "\n"));
        }
    }
}
