using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using System.IO;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    //Culture must be set because of DATE TIME format which is culture dependent
    [SetCulture("en-US")]
    [SetUICulture("en-US")]
    public class ShoppingListRepositoryTest
    {
        [Test]
        public void ShoppingListRepositoryContainsShoppingListsFromPersistentRepository_WhenReadFromXMLFileRepository()
        {
            //Arrange
            IList<ShoppingList> Expected = new List<ShoppingList>();
            
            ShoppingList shoppingList1 = new ShoppingList() { ShoppingListId=1, ShoppingListName="ShoppingList1", ShoppingListDueDate= new DateTime(1981,04,13)};
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=1, ItemName="Item1"}, QuantityToBuy=1});
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=2, ItemName="Item2"}, QuantityToBuy=1});
            shoppingList1.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=3, ItemName="Item3"}, QuantityToBuy=2});
            Expected.Add(shoppingList1);
            
            ShoppingList shoppingList2 = new ShoppingList() { ShoppingListId=2, ShoppingListName="ShoppingList2", ShoppingListDueDate= new DateTime(1982,05,27)};
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=1, ItemName="Item1"}, QuantityToBuy=1});
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=2, ItemName="Item2"}, QuantityToBuy=5});
            shoppingList2.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy= new Item() {ItemId=5, ItemName="Item5"}, QuantityToBuy=3});
            Expected.Add(shoppingList2);
            
            IEnumerable<ShoppingList> testee = null;

            //Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = (new ShoppingListRepository(repositoryNameProvider)).Repository;

            //Assert
            Assert.AreEqual(2, testee.Count());
            Assert.AreEqual(Expected.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListId).AsEnumerable(), "Failure by Id");
            Assert.AreEqual(Expected.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListName).AsEnumerable(), "Failure by Name");
        }

        [Test]
        public void ShoppingListAddedToPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ShoppingListRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Add.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.example.orig.xml");

            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 3, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2 });

            //Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);

            testee.Add(shoppingList);
            
            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.Add.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.xml");
            File.Copy(@"./ShoppingListRepository.example.orig.xml", @"./ShoppingListRepository.example.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Add.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Add.Actual.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListRemovedFromPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ShoppingListRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Remove.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.example.orig.xml");

            //Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);
            testee.Remove(1); //Remove ShoppingListId 1
            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.Remove.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.xml");
            File.Copy(@"./ShoppingListRepository.example.orig.xml", @"./ShoppingListRepository.example.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Remove.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Remove.Actual.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ShoppingListModifiedOnPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ShoppingListRepository testee = null;
            File.Delete(@"./ShoppingListRepository.Modified.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.example.orig.xml");

            ShoppingList shoppingList = new ShoppingList() { ShoppingListId = 1, ShoppingListName = "ShoppingList3", ShoppingListDueDate = new DateTime(2014, 12, 04) };
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 1, ItemName = "Item1" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 2, ItemName = "Item2" }, QuantityToBuy = 1 });
            shoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = 3, ItemName = "Item3" }, QuantityToBuy = 2 });

            //Act
            ShoppingListXmlTestRepositoryName repositoryNameProvider = new ShoppingListXmlTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);

            testee.Modify(shoppingList);

            testee.Save();
            File.Copy(@"./ShoppingListRepository.example.xml", @"./ShoppingListRepository.Modified.Actual.xml");
            File.Delete(@"./ShoppingListRepository.example.xml");
            File.Copy(@"./ShoppingListRepository.example.orig.xml", @"./ShoppingListRepository.example.xml");
            File.Delete(@"./ShoppingListRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ShoppingListRepository.Modified.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ShoppingListRepository.Modified.Actual.xml").Replace("\r\n", "\n"));
        }
    }
}
