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
    public class ItemXmlRepositoryTest 
    {
        private Mock<IRepositoryNameProvider> repositoryNameProvider;
        private Mock<IDataPathProvider> dataPathProvider;

        [SetUp]
        public void Init()
        {
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.example.orig.xml");
            File.Copy(@"./ItemRepository.invalid.xml", @"./ItemRepository.invalid.orig.xml");
            File.Copy(@"./ItemRepository.invalidempty.xml", @"./ItemRepository.invalidempty.orig.xml");
            File.Copy(@"./ItemRepository.empty.xml", @"./ItemRepository.empty.orig.xml");
            File.Copy(@"./ItemRepository.invalidxsd.xml", @"./ItemRepository.invalidxsd.orig.xml");
            File.Copy(@"./ItemRepository.Category.example.xml", @"./ItemRepository.Category.example.orig.xml");
            this.repositoryNameProvider = new Mock<IRepositoryNameProvider>();
            this.dataPathProvider = new Mock<IDataPathProvider>();
            this.dataPathProvider.Setup(provider => provider.DataPath).Returns(@"./");
        }

        [TearDown]
        public static void Dispose()
        {
            Thread.Sleep(10); // otherwise access to filesystem is too fast and creates access denied

            File.Delete(@"./ItemRepository.example.xml");
            File.Copy(@"./ItemRepository.example.orig.xml", @"./ItemRepository.example.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");

            File.Delete(@"./ItemRepository.invalid.xml");
            File.Copy(@"./ItemRepository.invalid.orig.xml", @"./ItemRepository.invalid.xml");
            File.Delete(@"./ItemRepository.invalid.orig.xml");

            File.Delete(@"./ItemRepository.invalidempty.xml");
            File.Copy(@"./ItemRepository.invalidempty.orig.xml", @"./ItemRepository.invalidempty.xml");
            File.Delete(@"./ItemRepository.invalidempty.orig.xml");

            File.Delete(@"./ItemRepository.empty.xml");
            File.Copy(@"./ItemRepository.empty.orig.xml", @"./ItemRepository.empty.xml");
            File.Delete(@"./ItemRepository.empty.orig.xml");

            File.Delete(@"./ItemRepository.invalidxsd.xml");
            File.Copy(@"./ItemRepository.invalidxsd.orig.xml", @"./ItemRepository.invalidxsd.xml");
            File.Delete(@"./ItemRepository.invalidxsd.orig.xml");

            File.Delete(@"./ItemRepository.Category.example.xml");
            File.Copy(@"./ItemRepository.Category.example.orig.xml", @"./ItemRepository.Category.example.xml");
            File.Delete(@"./ItemRepository.Category.example.orig.xml");

            File.Delete(@"./ItemRepository.doesnotexists.xml");
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryIsResetToDefaultUS_WhenXsdSchemaIsInvalid()
        { 
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalidxsd.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("fr-FR")]
        public void ItemRepositoryIsResetToDefaultFR_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalidxsd.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository_fr.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("de-DE")]
        public void ItemRepositoryIsResetToDefaultDE_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalidxsd.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository_de.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRepositoryContainsItemsFromPersistentRepository_WhenReadFromXmlFileRepository() 
        {        
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>()
            {
                new Item() { ItemId = 1, ItemName = "Item1", ItemCategory = "-" },
                new Item() { ItemId = 2, ItemName = "Item2", ItemCategory = "Category1" },
                new Item() { ItemId = 3, ItemName = "Item3", ItemCategory = "-" },
                new Item() { ItemId = 4, ItemName = "Item4", ItemCategory = "Category325" },
                new Item() { ItemId = 5, ItemName = "Item5", ItemCategory = "-" }
            };

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(5, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenReadFromEmptyXmlFileRepository()
        {
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.empty.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichDidNotExistPreviously()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.doesnotexists.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.doesnotexists.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidXmlFile()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalid.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalid.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidEmptyXmlFile()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalidempty.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalidempty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenNoPersistentRepositoryNameProviderIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ItemXmlRepository(null, dataPathProvider.Object));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenNoRepositoryNameIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns((string)null);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenEmptyRepositoryNameIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(string.Empty);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void ItemsAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Add("Item6");
            testee.Save();
            testee.Add("Item7");
            testee.Save();
            testee.Add("Item8");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Add.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void FirstItemAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.empty.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Add("Item15");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.AddFirst.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.empty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void InvalidItemNameThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            
            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Add(string.Empty));
        }

        [Test]
        public void NullItemNameThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Add(null));
        }

        [Test]
        public void ItemRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.Remove(3); // Remove ItemId 3
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Remove.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingItemRemovedFromPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.empty.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Remove(1));
        }

        [Test]
        public void ItemModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 4;
            string itemName = "Item12";
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ModifyName(itemId, itemName);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Modified.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingItemModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 4;
            string itemName = "Item12";
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.empty.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.ModifyName(itemId, itemName));
        }

        [Test]
        public void InvalidItemNameModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 1;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.ModifyName(itemId, null));
        }

        [Test]
        public void ItemCategoryModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 4;
            string itemCategory = "Others";
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ChangeItemCategory(itemId, itemCategory);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.ModifiedCategory.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingItemCategoryModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 4;
            string itemCategory = "Others";
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.empty.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.ChangeItemCategory(itemId, itemCategory));
        }

        [Test]
        public void InvalidItemCategoryModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 1;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.ChangeItemCategory(itemId, null));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenResetToEmpty()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            ItemXmlRepository testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetToEmpty();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryContainsDefaults_WhenResetToDefaults()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            ItemXmlRepository testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetToDefault();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./DefaultItemRepository.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryCategoryIsNotUpdated_WhenOldCategoryIsNotInUse()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            ItemXmlRepository testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.UpdateChangedCategoryName(null, "newCategoryName");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRepositoryCategoryThrowsArgumentOutOfRangeException_WhenNewCategoryIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            ItemXmlRepository testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.UpdateChangedCategoryName("Category1", null));
        }

        [Test]
        [SetUICulture("en-US")]
        public void ItemRepositoryCategoryIsUpdatedForAllItemsInTheCategory_WhenOldCategoryIsRenamedToNewCategory()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.Category.example.xml");

            // Act
            ItemXmlRepository testee = new ItemXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.UpdateChangedCategoryName("OldCategory", "newCategoryName");
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.UpdatedCategory.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Category.example.xml").Replace("\r\n", "\n"));
        }
    }
}