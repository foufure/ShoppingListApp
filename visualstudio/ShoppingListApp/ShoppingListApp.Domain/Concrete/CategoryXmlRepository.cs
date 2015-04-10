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
    public class CategoryXmlRepository : BaseRepository, ICategoryRepository
    {
        private List<string> categoryRepository = null;

        public CategoryXmlRepository(IRepositoryNameProvider repositoryNameProvider)
            : base(repositoryNameProvider)
        {
            this.InitializeXmlPersistentStorage("Categories", RepositoriesXsd.Categories());
            this.LoadFromXmlPersistentStorage();
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

            if (!categoryRepository.Remove(categoryName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the category does not exist. Please enter a valid category name", (Exception)null);
            }
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
            else 
            {
                throw new ArgumentOutOfRangeException("Internal Error: the category does not exist. Please enter a valid category name", (Exception)null);
            }
        }

        public void Save()
        {
            XElement elements = new XElement("Categories");
            foreach (string category in categoryRepository)
            {
                elements.Add(new XElement("Category", category));
            }

            elements.Save(RepositoryName);
        }

        private void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(RepositoryName);

            categoryRepository = new List<string>();
            foreach (XElement element in parsedFile.Elements("Categories").Elements("Category"))
            {
                categoryRepository.Add(element.Value);
            }
        }
    }
}
