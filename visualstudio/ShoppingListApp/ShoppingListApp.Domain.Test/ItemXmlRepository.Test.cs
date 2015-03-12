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

        [SetUp]
        public void Init()
        {
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.example.orig.xml");
            File.Copy(@"./ItemRepository.invalid.xml", @"./ItemRepository.invalid.orig.xml");
            File.Copy(@"./ItemRepository.invalidempty.xml", @"./ItemRepository.invalidempty.orig.xml");
            File.Copy(@"./ItemRepository.empty.xml", @"./ItemRepository.empty.orig.xml");
            this.repositoryNameProvider = new Mock<IRepositoryNameProvider>();
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

            File.Delete(@"./ItemRepository.doesnotexists.xml");
        }

        [Test]
        public void ItemRepositoryContainsItemsFromPersistentRepository_WhenReadFromXmlFileRepository() 
        {        
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>()
            {
                new Item() { ItemId = 1, ItemName = "Item1" },
                new Item() { ItemId = 2, ItemName = "Item2" },
                new Item() { ItemId = 3, ItemName = "Item3" },
                new Item() { ItemId = 4, ItemName = "Item4" },
                new Item() { ItemId = 5, ItemName = "Item5" }
            };

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object)).Repository;

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
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichDidNotExistPreviously()
        {
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.doesnotexists.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.doesnotexists.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidXmlFile()
        {
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalid.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalid.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidEmptyXmlFile()
        {
            // Arrange
            IEnumerable<Item> expectedResult = new List<Item>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.invalidempty.xml");

            // Act
            IEnumerable<Item> testee = (new ItemXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.invalidempty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public static void ItemRepositoryIsEmpty_WhenNoPersistentRepositoryNameProviderIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ItemXmlRepository(null));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenNoRepositoryNameIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns((string)null);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ItemXmlRepository(this.repositoryNameProvider.Object));
        }

        [Test]
        public void ItemRepositoryIsEmpty_WhenEmptyRepositoryNameIsAvailable()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(string.Empty);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ItemXmlRepository(this.repositoryNameProvider.Object));
        }

        [Test]
        public void ItemsAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);
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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);
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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);
            
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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);

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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);
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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);

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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);
            testee.Modify(itemId, itemName);
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
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify(itemId, itemName));
        }

        [Test]
        public void InvalidItemNameModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemXmlRepository testee = null;
            uint itemId = 1;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ItemRepository.example.xml");

            // Act
            testee = new ItemXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify(itemId, null));
        }
    }
}
