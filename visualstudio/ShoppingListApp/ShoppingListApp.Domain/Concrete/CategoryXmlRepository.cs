using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Concrete
{
    public class CategoryXmlRepository : ICategoryRepository
    {
        private List<string> categoryRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public CategoryXmlRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            if (repositoryNameProvider.RepositoryNameIsValid())
            {
                this.repositoryNameProvider = repositoryNameProvider;
                this.InitializeXmlPersistentStorage();
                this.LoadFromXmlPersistentStorage();
            }
        }

        public IEnumerable<string> Repository
        {
            get
            {
                return categoryRepository;
            }
        }

        public void Add(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the category is empty or null. Please enter a valid name", (Exception)null);
            }

            if (!categoryRepository.Contains(categoryName))
            {
                categoryRepository.Add(categoryName);
            }
        }

        public void Remove(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the category is empty or null. Please enter a valid name", (Exception)null);
            }

            categoryRepository.Remove(categoryName);
        }

        public void Modify(string oldCategoryName, string newCategoryName)
        {
            if (string.IsNullOrEmpty(oldCategoryName) || string.IsNullOrEmpty(newCategoryName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the category is empty or null. Please enter a valid name", (Exception)null);
            }

            if (categoryRepository.Remove(oldCategoryName))
            {
                categoryRepository.Add(newCategoryName);
            }
        }

        public void Save()
        {
            XElement elements = new XElement("Categories");
            foreach (string category in categoryRepository)
            {
                elements.Add(new XElement("Category", category));
            }

            elements.Save(repositoryNameProvider.RepositoryName);
        }

        private void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

            categoryRepository = new List<string>();
            foreach (XElement element in parsedFile.Elements("Categories").Elements("Category"))
            {
                categoryRepository.Add(element.Value);
            }
        }

        private void InitializeXmlPersistentStorage()
        {
            if (!File.Exists(repositoryNameProvider.RepositoryName) || !XmlRepositoryIsValid())
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Categories"));
                newRepository.Save(repositoryNameProvider.RepositoryName);
            }
        }

        private bool XmlRepositoryIsValid()
        {
            return XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Categories(), repositoryNameProvider.RepositoryName);
        }
    }
}
