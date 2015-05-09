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
        private Mock<IDataPathProvider> dataPathProvider;

        [SetUp]
        public void Init()
        {
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.example.orig.xml");
            File.Copy(@"./ShoppingListRepository.invalid.xml", @"./ShoppingListRepository.invalid.orig.xml");
            File.Copy(@"./ShoppingListRepository.invalidempty.xml", @"./ShoppingListRepository.invalidempty.orig.xml");
            File.Copy(@"./ShoppingListRepository.empty.xml", @"./ShoppingListRepository.empty.orig.xml");
            File.Copy(@"./ShoppingListRepository.invalidxsd.xml", @"./ShoppingListRepository.invalidxsd.orig.xml");
            File.Copy(@"./ShoppingListRepository.example.DoneElements.xml", @"./ShoppingListRepository.example.DoneElements.orig.xml");
            this.repositoryNameProvider = new Mock<IRepositoryNameProvider>();
            this.dataPathProvider = new Mock<IDataPathProvider>();
            this.dataPathProvider.Setup(provider => provider.DataPath).Returns(@"./");
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

            File.Delete(@"./ShoppingListRepository.invalidxsd.xml");
            File.Copy(@"./ShoppingListRepository.invalidxsd.orig.xml", @"./ShoppingListRepository.invalidxsd.xml");
            File.Delete(@"./ShoppingListRepository.invalidxsd.orig.xml");

            File.Delete(@"./ShoppingListRepository.example.DoneElements.xml");
            File.Copy(@"./ShoppingListRepository.example.DoneElements.orig.xml", @"./ShoppingListRepository.example.DoneElements.xml");
            File.Delete(@"./ShoppingListRepository.example.DoneElements.orig.xml");

            File.Delete(@"./ShoppingListRepository.doesnotexists.xml");
        }

        [Test]
        public void ShoppingListRepositoryIsResetToDefault_WhenXsdSchemaIsInvalid()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.invalidxsd.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.True(testee.Count() == 0);
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.invalidxsd.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryContainsShoppingListsFromPersistentRepository_WhenReadFromXmlFileRepository()
        {
            // Arrange
            IList<ShoppingList> expectedResult = new List<ShoppingList>();
            
            ShoppingList shoppingList1 = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList1", ShoppingListDueDate = new DateTime(1981, 04, 13) };
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });
            expectedResult.Add(shoppingList1);
            
            ShoppingList shoppingList2 = new ShoppingList() { ShoppingListId = 2, ShoppingListName = "ShoppingList2", ShoppingListDueDate = new DateTime(1982, 05, 27) };
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 5, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 5, ItemName = "Item5" }, QuantityToBuy = 3, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });
            expectedResult.Add(shoppingList2);

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");

            // Act
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

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
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

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
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

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
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

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
            IEnumerable<ShoppingList> testee = (new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object)).Repository;

            // Assert
            Assert.AreEqual(0, testee.Count());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable(), testee.Select(ShoppingList => ShoppingList.ShoppingListName).AsEnumerable());
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.invalidempty.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenNoPersistentRepositoryNameProviderIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee = new ShoppingListXmlRepository(null, dataPathProvider.Object));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenNoRepositoryNameIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns((string)null);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenEmptyRepositoryNameIsAvailable()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(string.Empty);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object));
        }

        [Test]
        public void ShoppingListAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });

            ShoppingList shoppingListNext = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList4", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item4" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item5" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item6" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

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
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });

            ShoppingList shoppingListNext = new ShoppingList() { ShoppingListId = 0, ShoppingListName = "ShoppingList4", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item4" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item5" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingListNext.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item6" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.empty.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

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
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.Add(null));
        }

        [Test]
        public void ShoppingListRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

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
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Remove(1));
        }

        [Test]
        public void ShoppingListModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1, LinePresentationOrder = 1, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1, LinePresentationOrder = 2, Unit = UnitsUtils.Units["default"], Done = false });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2, LinePresentationOrder = 3, Unit = UnitsUtils.Units["default"], Done = false });

            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

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
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

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
            testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);

            // Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => testee.Modify(nonExistingShoppingList));
        }

        [Test]
        public void ShoppingListRepositoryIsEmpty_WhenResetToEmpty()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetToEmpty();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.empty.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryRemainsUnchanged_WhenTryingToDeleteALineInAShoppingListThatDoesNotExist()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint invalidShoppingListId = 934;
            uint invalidItemId = 12756;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.DeleteShoppingListLine(invalidShoppingListId, invalidItemId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryRemainsUnchanged_WhenTryingToDeleteALineThatDoesNotExist()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 2;
            uint invalidItemId = 12756;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.DeleteShoppingListLine(validShoppingListId, invalidItemId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryUpdated_WhenTryingToDeleteALineThatExists()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 2;
            uint validItemId = 5;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.DeleteShoppingListLine(validShoppingListId, validItemId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.DeletedLine.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryRemainsUnchanged_WhenTryingToResetAllDoneElementsOfAShoppingListThatDoesNotExists()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint invalidShoppingListId = 1288;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetAllDoneElementsFromShoppingList(invalidShoppingListId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryUpdated_WhenResettingAllDoneElements()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.DoneElements.xml");
            uint validShoppingListId = 2;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ResetAllDoneElementsFromShoppingList(validShoppingListId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.NoDoneElements.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryRemainsUnchanged_WhenTryingToToggleDoneStatusOfAShoppingListThatDoesNotExist()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.DoneElements.xml");
            uint invalidShoppingListId = 123;
            uint validItemId = 3;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ToggleShoppingListLineDoneStatus(invalidShoppingListId, validItemId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryRemainsUnchanged_WhenTryingToToggleDoneStatusOfAShoppingListLineThatDoesNotExist()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.DoneElements.xml");
            uint validShoppingListId = 2;
            uint invalidItemId = 12444;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ToggleShoppingListLineDoneStatus(validShoppingListId, invalidItemId);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryUpdated_WhenTogglingDoneStatusOfAnExistingElement()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.DoneElements.xml");
            uint validShoppingListId1 = 1;
            uint validItemId1 = 3;
            uint validShoppingListId2 = 2;
            uint validItemId2 = 1;
            uint validShoppingListId3 = 2;
            uint validItemId3 = 2;

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ToggleShoppingListLineDoneStatus(validShoppingListId1, validItemId1);
            testee.ToggleShoppingListLineDoneStatus(validShoppingListId2, validItemId2);
            testee.ToggleShoppingListLineDoneStatus(validShoppingListId3, validItemId3);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.ToggledDoneElements.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.DoneElements.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryUpdated_WhenShoppingListLinesAreReorderedBackwards()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint validItemId = 3;
            int initialPosition = 3;
            int targetPosition = 1;
            string direction = "back";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.ReorderedElementsBack.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRepositoryUpdated_WhenShoppingListLinesAreReorderedForwards()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 2;
            uint validItemId = 1;
            int initialPosition = 1;
            int targetPosition = 2;
            string direction = "forward";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.ReorderedElementsForward.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenShoppingListDoesNotExists()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint invalidShoppingListId = 123456666;
            uint validItemId = 1;
            int initialPosition = 1;
            int targetPosition = 2;
            string direction = "forward";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(invalidShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenItemDoesNotExists()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint invalidItemId = 999;
            int initialPosition = 1;
            int targetPosition = 2;
            string direction = "forward";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, invalidItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenSamePositionAsInitialPositionForward()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint validItemId = 1;
            int initialPosition = 1;
            int targetPosition = 1;
            string direction = "forward";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenSamePositionAsInitialPositionBackward()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint validItemId = 1;
            int initialPosition = 1;
            int targetPosition = 1;
            string direction = "back";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenInvalidTargetInitialPositionsBackward()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint validItemId = 1;
            int initialPosition = 1;
            int targetPosition = 3;
            string direction = "back";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListLinesOrderRemainsUnchanged_WhenInvalidTargetInitialPositionsForward()
        {
            // Arrange
            this.repositoryNameProvider.Setup(x => x.RepositoryName).Returns(@"./ShoppingListRepository.example.xml");
            uint validShoppingListId = 1;
            uint validItemId = 1;
            int initialPosition = 3;
            int targetPosition = 1;
            string direction = "forward";

            // Act
            ShoppingListXmlRepository testee = new ShoppingListXmlRepository(this.repositoryNameProvider.Object, dataPathProvider.Object);
            testee.ReorderShoppingListLines(validShoppingListId, validItemId, initialPosition, targetPosition, direction);
            testee.Save();

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.example.orig.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.example.xml").Replace("\r\n", "\n"));
        }
    }
}
