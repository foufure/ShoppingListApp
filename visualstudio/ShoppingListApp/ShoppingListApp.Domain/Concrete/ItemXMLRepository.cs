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
    public class ItemXmlRepository : IItemsRepository
    {
        private List<Item> itemRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ItemXmlRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            repositoryNameProvider.RepositoryNameValidation();

            this.repositoryNameProvider = repositoryNameProvider;
            this.InitializeXmlPersistentStorage();
            this.LoadFromXmlPersistentStorage();
        }

        public IEnumerable<Entities.Item> Repository
        { 
            get
            {
                return itemRepository;
            }
        }

        public void Add(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the item is empty or null. Please enter a valid name", (Exception)null);
            }

            uint itemId = itemRepository.OrderByDescending(item => item.ItemId).Select(item => item.ItemId).FirstOrDefault() + 1;
            Item itemToAdd = new Item() { ItemId = itemId, ItemName = itemName, ItemCategory = CategoryUtils.DefaultCategory };
            itemRepository.Add(itemToAdd);
        }

        public void Modify(uint itemId, string itemName)
        {
            Item itemToModify = null;

            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the item is empty or null. Please enter a valid name", (Exception)null);
            }

            if ((itemToModify = itemRepository.Where(repositoryItem => repositoryItem.ItemId == itemId).FirstOrDefault()) != null)
            {
                itemToModify.ItemName = itemName;
            }
            else 
            {
                throw new ArgumentOutOfRangeException("Internal Error: the item to be modified does not exist. Please enter a valid Item Id", (Exception)null);
            } 
        }

        public void ModifyCategory(uint itemId, string itemCategory)
        {
            Item itemToModify = null;

            if (string.IsNullOrEmpty(itemCategory))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the name of the category is empty or null. Please enter a valid name", (Exception)null);
            }

            if ((itemToModify = itemRepository.Where(repositoryItem => repositoryItem.ItemId == itemId).FirstOrDefault()) != null)
            {
                itemToModify.ItemCategory = itemCategory;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Internal Error: the item to be modified does not exist. Please enter a valid Item Id", (Exception)null);
            }
        }

        public void Remove(uint itemId)
        {
            if (!itemRepository.Remove(itemRepository.Where(repositoryItem => repositoryItem.ItemId == itemId).FirstOrDefault()))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the item to be deleted does not exist. Please enter a valid Item Id", (Exception)null);
            }
        }

        public void Save()
        {
            XElement elements = new XElement("Items");
            foreach (Item item in itemRepository)
            {
                elements.Add(new XElement(
                    "Item",
                    new XElement("ItemId") { Value = item.ItemId.ToString(CultureInfo.InvariantCulture) },
                    new XElement("ItemName") { Value = item.ItemName },
                    new XElement("ItemCategory") { Value = item.ItemCategory }));
            }

            elements.Save(repositoryNameProvider.RepositoryName);
        }

        private void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

            itemRepository = new List<Item>();
            foreach (XElement element in parsedFile.Elements("Items").Elements("Item"))
            {
                itemRepository.Add(new Item() 
                { 
                    ItemId = Convert.ToUInt32(element.Element("ItemId").Value, CultureInfo.InvariantCulture),
                    ItemName = element.Element("ItemName").Value,
                    ItemCategory = element.Element("ItemCategory").Value 
                });
            }
        }

        private void InitializeXmlPersistentStorage()
        {
            if (!File.Exists(repositoryNameProvider.RepositoryName) || !XmlRepositoryIsValid())
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Items"));
                newRepository.Save(repositoryNameProvider.RepositoryName);
            }
        }

        private bool XmlRepositoryIsValid()
        {
            return XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Items(), repositoryNameProvider.RepositoryName);
        }
    }
}
