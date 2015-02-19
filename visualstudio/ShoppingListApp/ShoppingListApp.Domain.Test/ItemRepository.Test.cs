using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Test 
{    
    [TestFixture]
    public class ItemRepositoryTest 
    {
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

            IEnumerable<Item> testee = null;

            // Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = (new ItemRepository(repositoryNameProvider)).Repository;

            // Assert
            Assert.AreEqual(5, testee.Count());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemId).AsEnumerable(), testee.Select(Item => Item.ItemId).AsEnumerable());
            Assert.AreEqual(expectedResult.Select(Item => Item.ItemName).AsEnumerable(), testee.Select(Item => Item.ItemName).AsEnumerable());
        }

        [Test]
        public void ItemAddedToPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Add.Actual.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.example.orig.Xml");

            // Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Add("Item6");
            testee.Save();
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.Add.Actual.Xml");
            File.Delete(@"./ItemRepository.example.Xml");
            File.Copy(@"./ItemRepository.example.orig.Xml", @"./ItemRepository.example.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Add.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Add.Actual.Xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemRemovedFromPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Remove.Actual.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.example.orig.Xml");

            // Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Remove(3); // Remove ItemId 3
            testee.Save();
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.Remove.Actual.Xml");
            File.Delete(@"./ItemRepository.example.Xml");
            File.Copy(@"./ItemRepository.example.orig.Xml", @"./ItemRepository.example.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Remove.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Remove.Actual.Xml").Replace("\r\n", "\n"));
        }

        [Test]
        public void ItemModifiedOnPersistentRepository_WhenWrittenToXmlFileRepository()
        {
            // Arrange
            ItemRepository testee = null;
            File.Delete(@"./ItemRepository.Modified.Actual.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.example.orig.Xml");

            // Act
            ItemXmlTestRepositoryName repositoryNameProvider = new ItemXmlTestRepositoryName();
            testee = new ItemRepository(repositoryNameProvider);
            testee.Modify(new Item() { ItemId = 4, ItemName = "Item12" });
            testee.Save();
            File.Copy(@"./ItemRepository.example.Xml", @"./ItemRepository.Modified.Actual.Xml");
            File.Delete(@"./ItemRepository.example.Xml");
            File.Copy(@"./ItemRepository.example.orig.Xml", @"./ItemRepository.example.Xml");
            File.Delete(@"./ItemRepository.example.orig.Xml");

            // Assert
            Assert.AreEqual(File.ReadAllText(@"./ItemRepository.Modified.Expected.Xml").Replace("\r\n", "\n"), File.ReadAllText(@"./ItemRepository.Modified.Actual.Xml").Replace("\r\n", "\n"));
        }
    }
}
