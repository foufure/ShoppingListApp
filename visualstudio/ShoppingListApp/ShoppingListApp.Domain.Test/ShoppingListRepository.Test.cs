using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public static class ShoppingListRepositoryTest
    {
        [Test]
        public static void ShoppingListRepositoryContainsShoppingListsFromPersistentRepository_WhenReadFromXmlFileRepository()
        {
            // Arrange
            IList<ShoppingList> expectedResult = new List<ShoppingList>();
            
            ShoppingList shoppingList1 = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList1", ShoppingListDueDate = new DateTime(1981, 04, 13) };
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1 });
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2 });
            expectedResult.Add(shoppingList1);
            
            ShoppingList shoppingList2 = new ShoppingList() { ShoppingListId = 2, ShoppingListName = "ShoppingList2", ShoppingListDueDate = new DateTime(1982, 05, 27) };
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 5 });
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 5, ItemName = "Item5" }, QuantityToBuy = 3 });
            expectedResult.Add(shoppingList2);
            
            IEnumerable<ShoppingList> testee = null;

            // Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = (new ShoppingListXmlRepository(repositoryNameProvider)).Repository;

            // Assert
            Assert.AreEqual(2, testee.Count());
            Assert.AreEqual(expectedResult.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable());
        }

        [Test]
        public static void ShoppingListAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Add.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.example.orig.Xml");

            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 3, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2 });

            // Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListXmlRepository(repositoryNameProvider);

            testee.Add(shoppingList);
            
            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.Add.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.Xml");
            File.Copy(@"./ShoppingListRepository.example.orig.Xml", @"./ShoppingListRepository.example.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Add.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Add.Actual.Xml").Replace("\r\n", "\n"));
        }

        [Test]
        public static void ShoppingListRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Remove.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.example.orig.Xml");

            // Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListXmlRepository(repositoryNameProvider);
            testee.Remove(1); // Remove ShoppingListId 1
            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.Remove.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.Xml");
            File.Copy(@"./ShoppingListRepository.example.orig.Xml", @"./ShoppingListRepository.example.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Remove.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Remove.Actual.Xml").Replace("\r\n", "\n"));
        }

        [Test]
        public static void ShoppingListModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ShoppingListXmlRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Modified.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.example.orig.Xml");

            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2 });

            // Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListXmlRepository(repositoryNameProvider);

            testee.Modify(shoppingList);

            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.Xml", @"./ShoppingListRepository.Modified.Actual.Xml");
            File.Delete(@"./ShoppingListRepository.example.Xml");
            File.Copy(@"./ShoppingListRepository.example.orig.Xml", @"./ShoppingListRepository.example.Xml");
            File.Delete(@"./ShoppingListRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Modified.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Modified.Actual.Xml").Replace("\r\n", "\n"));
        }
    }
}
