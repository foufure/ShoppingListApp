﻿using NUnit.Framework;
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
            IEnumerable<ShoppingList> Expected = new List<ShoppingList>()
            {
                new ShoppingList() { ShoppingListID=1, ShoppingListName="ShoppingList1", ShoppingListDueDate= new DateTime(1981,04,13), 
                                        ShoppingListContent= new List<ShoppingListLine>(){ 
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=1, ItemName="Item1"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=2, ItemName="Item2"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=3, ItemName="Item3"}, QuantityToBuy=2}
                                                                        }
                                    },
                new ShoppingList()  { ShoppingListID=2, ShoppingListName="ShoppingList2", ShoppingListDueDate= new DateTime(1982,05,27), 
                                        ShoppingListContent= new List<ShoppingListLine>(){ 
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=1, ItemName="Item1"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=2, ItemName="Item2"}, QuantityToBuy=5},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=5, ItemName="Item5"}, QuantityToBuy=3}
                                                                        }
                                    },
            };

            IEnumerable<ShoppingList> testee = null;

            //Act
            ShoppingListXMLTestRepositoryName repositoryNameProvider = new ShoppingListXMLTestRepositoryName();
            testee = (new ShoppingListRepository(repositoryNameProvider)).repository;

            //Assert
            Assert.AreEqual(2, testee.Count());
            Assert.AreEqual(Expected.Select(shoppingList => shoppingList.ShoppingListID).AsEnumerable(), testee.Select(shoppingList => shoppingList.ShoppingListID).AsEnumerable(), "Failure by ID");
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

            ShoppingList shoppinglist = new ShoppingList() {
                                                                ShoppingListID = 3,
                                                                ShoppingListName = "ShoppingList3",
                                                                ShoppingListDueDate = new DateTime(2014, 12, 04),
                                                                ShoppingListContent = new List<ShoppingListLine>()
                                                                { 
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=1, ItemName="Item1"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=2, ItemName="Item2"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=3, ItemName="Item3"}, QuantityToBuy=2}
                                                                }
            };

            //Act
            ShoppingListXMLTestRepositoryName repositoryNameProvider = new ShoppingListXMLTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);

            testee.Add(shoppinglist);
            
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
            ShoppingListXMLTestRepositoryName repositoryNameProvider = new ShoppingListXMLTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);
            testee.Remove(1); //Remove ShoppingListID 1
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

            ShoppingList shoppinglist = new ShoppingList()
            {
                ShoppingListID = 1,
                ShoppingListName = "ShoppingList3",
                ShoppingListDueDate = new DateTime(2014, 12, 04),
                ShoppingListContent = new List<ShoppingListLine>()
                                                                { 
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=1, ItemName="Item1"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=2, ItemName="Item2"}, QuantityToBuy=1},
                                                                        new ShoppingListLine() { ItemToBuy= new Item() {ItemID=3, ItemName="Item3"}, QuantityToBuy=2}
                                                                }
            };

            //Act
            ShoppingListXMLTestRepositoryName repositoryNameProvider = new ShoppingListXMLTestRepositoryName();
            testee = new ShoppingListRepository(repositoryNameProvider);

            testee.Modify(shoppinglist);

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
