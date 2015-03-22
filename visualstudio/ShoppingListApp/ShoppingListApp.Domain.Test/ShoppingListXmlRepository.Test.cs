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
    public class ShoppingListXmlRepositoryTest
    {
        private Mock<IRepositoryNameProvider> repositoryNameProvider;

        [SetUp]
        public void Init()
        {
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.example.orig.xml");
            File.Copy(@"./ShoppingListRepository.invalid.xml", @"./ShoppingListRepository.invalid.orig.xml");
            File.Copy(@"./ShoppingListRepository.invalidempty.xml", @"./ShoppingListRepository.invalidempty.orig.xml");
            File.Copy(@"./ShoppingListRepository.empty.xml", @"./ShoppingListRepository.empty.orig.xml");
            this.repositoryNameProvider = new Mock<IRepositoryNameProvider>();
        }

        [TearDown]
        public static void Dispose()
        {
            Thread.Sleep(10); // otherwise access to filesystem is too fast and creates access denied

            File.Delete(@"./ShoppingListRepository.example.xml");
            File.Copy(@"./ShoppingListRepository.example.orig.xml", @"./ShoppingListRepository.example.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");

            File.Delete(@"./ShoppingListRepository.invalid.xml");
            File.Copy(@"./ShoppingListRepository.invalid.orig.xml", @"./ShoppingListRepository.invalid.xml");
            File.Delete(@"./ShoppingListRepository.invalid.orig.xml");

            File.Delete(@"./ShoppingListRepository.invalidempty.xml");
            File.Copy(@"./ShoppingListRepository.invalidempty.orig.xml", @"./ShoppingListRepository.invalidempty.xml");
            File.Delete(@"./ShoppingListRepository.invalidempty.orig.xml");

            File.Delete(@"./ShoppingListRepository.empty.xml");
            File.Copy(@"./ShoppingListRepository.empty.orig.xml", @"./ShoppingListRepository.empty.xml");
            File.Delete(@"./ShoppingListRepository.empty.orig.xml");

            File.Delete(@"./ShoppingListRepository.doesnotexists.xml");
        }

        [Test]
        public void ShoppingListRepositoryContainsShoppingListsFromPersistentRepository_WhenReadFromXmlFileRepository()
        {
            // Arrange
            IList<ShoppingList> expectedResult = new List<ShoppingList>();
            
            ShoppingList shoppingList1 = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList1", ShoppingListDueDate = new DateTime(1981, 04, 13) };
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3 });
            expectedResult.Add(shoppingList1);
            
            ShoppingList shoppingList2 = new ShoppingList() { ShoppingListId = 2, ShoppingListName = "ShoppingList2", ShoppingListDueDate = new DateTime(1982, 05, 27) };
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 5, LinePresentationOrder = 2 });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 5, ItemName = "Item5" }, QuantityToBuy = 3, LinePresentationOrder = 3 });
            expectedResult.Add(shoppingList2);

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(2, testee.Count());
            Assert.AreEqual(expectedResult.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable());
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenReadFromEmptyXmlFileRepository()
        {
            // Arrange
            IEnumerable<ShoppingList> expectedResult = new List<ShoppingList>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.empty.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable());
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichDidNotExistPreviously()
        {
            // Arrange
            IEnumerable<ShoppingList> expectedResult = new List<ShoppingList>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.doesnotexists.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.doesnotexists.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidXmlFile()
        {
            // Arrange
            IEnumerable<ShoppingList> expectedResult = new List<ShoppingList>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.invalid.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.invalid.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenReadFromNewlyCreatedXmlFileRepository_WhichWasAnInvalidEmptyXmlFile()
        {
            // Arrange
            IEnumerable<ShoppingList> expectedResult = new List<ShoppingList>();
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.invalidempty.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.invalidempty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public static void ShoppingListRepositoryIsEmpty_WhenNoPersistentRepositoryNameProviderIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ShoppingListXmlRepository(null));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenNoRepositoryNameIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns((string)null);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenEmptyRepositoryNameIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(string.Empty);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object));
        }

        [Test]
        public void ShoppingListAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3 });

            ShoppingList shoppingListNext = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList4", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item4" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item5" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item6" }, QuantityToBuy = 2, LinePresentationOrder = 3 });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Act
            testee.Add(shoppingList);
            testee.Add(shoppingListNext);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Add.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void FirstShoppingListAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3 });

            ShoppingList shoppingListNext = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList4", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item4" }, QuantityToBuy = 1, LinePresentationOrder = 1  });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item5" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item6" }, QuantityToBuy = 2, LinePresentationOrder = 3 });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.empty.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Act
            testee.Add(shoppingList);
            testee.Add(shoppingListNext);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.AddFirst.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NullShoppingListThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");

            // Act
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.Add(null));
        }

        [Test]
        public void ShoppingListRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Act
            testee.Remove(1); // Remove ShoppingListId 1
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Remove.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void NonExistingShoppingListRemovedFromPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.empty.xml");

            // Act
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Remove(1));
        }

        [Test]
        public void ShoppingListModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3 });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Act
            testee.Modify(shoppingList);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Modified.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void InvalidShoppingListModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");

            // Act
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.Modify(null));
        }

        [Test]
        public void NonExistingShoppingListModifiedOnPersistentRepositoryThrowsException_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.empty.xml");
            ShoppingList nonExistingShoppingList = new ShoppingList() { ShoppingListId = 1 };

            // Act
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify(nonExistingShoppingList));
        }
    }
}
