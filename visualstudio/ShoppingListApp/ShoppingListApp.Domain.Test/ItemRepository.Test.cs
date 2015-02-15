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
    public class ItemRepositoryTest 
    {
        [Test]
        public void ItemRepositoryContainsItemsFromPersistentRepository_WhenReadFromXMLFileRepository() 
        {
            
            //Arrange
            IEnumerable<Item> Expected = new List<Item>()
            {
                new Item() { ItemId=1, ItemName="Item1"},
                new Item() { ItemId=2, ItemName="Item2"},
                new Item() { ItemId=3, ItemName="Item3"},
                new Item() { ItemId=4, ItemName="Item4"},
                new Item() { ItemId=5, ItemName="Item5"}
            };

            IEnumerable<Item> testee = null;

            //Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = (new ItemRepository(repositoryNameProvider)).Repository;

            //Assert
            Assert.AreEqual(5, testee.Count());
            Assert.AreEqual(Expected.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(Expected.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
        }

        [Test]
        public void ItemAddedToPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Add.Actual.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.example.orig.xml");

            //Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Add("Item6");
            testee.Save();
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.Add.Actual.xml");
            File.Delete(@"./ItemRepository.example.xml");
            File.Copy(@"./ItemRepository.example.orig.xml", @"./ItemRepository.example.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Add.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Add.Actual.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRemovedFromPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Remove.Actual.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.example.orig.xml");

            //Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Remove(3); //Remove ItemId 3
            testee.Save();
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.Remove.Actual.xml");
            File.Delete(@"./ItemRepository.example.xml");
            File.Copy(@"./ItemRepository.example.orig.xml", @"./ItemRepository.example.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Remove.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Remove.Actual.xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemModifiedOnPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Modified.Actual.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.example.orig.xml");

            //Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Modify(new Item() { ItemId = 4, ItemName = "Item12" });
            testee.Save();
            File.Copy(@"./ItemRepository.example.xml", @"./ItemRepository.Modified.Actual.xml");
            File.Delete(@"./ItemRepository.example.xml");
            File.Copy(@"./ItemRepository.example.orig.xml", @"./ItemRepository.example.xml");
            File.Delete(@"./ItemRepository.example.orig.xml");

            //Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Modified.Expected.xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Modified.Actual.xml").Replace("\r\n", "\n"));
        }
    }
}
