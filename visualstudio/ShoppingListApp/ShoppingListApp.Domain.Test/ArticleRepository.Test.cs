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
    public class ArticleRepositoryTest 
    {
        [Test]
        public void ArticleRepositoryContainsArticlesFromPersistentRepository_WhenReadFromXMLFileRepository() 
        {
            
            //Arrange
            IEnumerable<Article> Expected = new List<Article>()
            {
                new Article() { ArticleID=1, ArticleName="Article1"},
                new Article() { ArticleID=2, ArticleName="Article2"},
                new Article() { ArticleID=3, ArticleName="Article3"},
                new Article() { ArticleID=4, ArticleName="Article4"},
                new Article() { ArticleID=5, ArticleName="Article5"}
            };

            IEnumerable<Article> testee = null;

            //Act
            ArticleXMLTestRepositoryName repositoryNameProvider = new ArticleXMLTestRepositoryName();
            testee = (new ArticleRepository(repositoryNameProvider)).repository;

            //Assert
            Assert.AreEqual(5, testee.Count());
            Assert.AreEqual(Expected.Select(article => article.ArticleID).AsEnumerable(), testee.Select(article => article.ArticleID).AsEnumerable());
            Assert.AreEqual(Expected.Select(article => article.ArticleName).AsEnumerable(), testee.Select(article => article.ArticleName).AsEnumerable());
        }

        [Test]
        public void ArticleAddedToPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ArticleRepository testee = null;
            File.Delete(@"./ArticleRepository.Add.Actual.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.example.orig.xml");

            //Act
            ArticleXMLTestRepositoryName repositoryNameProvider = new ArticleXMLTestRepositoryName();
            testee = new ArticleRepository(repositoryNameProvider);
            testee.Add("Article6");
            testee.Save();
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.Add.Actual.xml");
            File.Delete(@"./ArticleRepository.example.xml");
            File.Copy(@"./ArticleRepository.example.orig.xml", @"./ArticleRepository.example.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");

            //Assert
            FileAssert.AreEqual(@"./ArticleRepository.Add.Expected.xml", @"./ArticleRepository.Add.Actual.xml");
        }

        [Test]
        public void ArticleRemovedFromPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ArticleRepository testee = null;
            File.Delete(@"./ArticleRepository.Remove.Actual.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.example.orig.xml");

            //Act
            ArticleXMLTestRepositoryName repositoryNameProvider = new ArticleXMLTestRepositoryName();
            testee = new ArticleRepository(repositoryNameProvider);
            testee.Remove(3); //Remove ArticleID 3
            testee.Save();
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.Remove.Actual.xml");
            File.Delete(@"./ArticleRepository.example.xml");
            File.Copy(@"./ArticleRepository.example.orig.xml", @"./ArticleRepository.example.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");

            //Assert
            FileAssert.AreEqual(@"./ArticleRepository.Remove.Expected.xml", @"./ArticleRepository.Remove.Actual.xml");
        }

        [Test]
        public void ArticleModifiedOnPersistentRepository_WhenWrittenToXMLFileRepository()
        {
            //Arrange
            ArticleRepository testee = null;
            File.Delete(@"./ArticleRepository.Modified.Actual.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.example.orig.xml");

            //Act
            ArticleXMLTestRepositoryName repositoryNameProvider = new ArticleXMLTestRepositoryName();
            testee = new ArticleRepository(repositoryNameProvider);
            testee.Modify(new Article() { ArticleID = 4, ArticleName = "Article12" });
            testee.Save();
            File.Copy(@"./ArticleRepository.example.xml", @"./ArticleRepository.Modified.Actual.xml");
            File.Delete(@"./ArticleRepository.example.xml");
            File.Copy(@"./ArticleRepository.example.orig.xml", @"./ArticleRepository.example.xml");
            File.Delete(@"./ArticleRepository.example.orig.xml");

            //Assert
            FileAssert.AreEqual(@"./ArticleRepository.Modified.Expected.xml", @"./ArticleRepository.Modified.Actual.xml");
        }
    }
}
