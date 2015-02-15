using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using System.Xml.Linq;
using ShoppingListApp.Domain.Entities;
using System.IO;
using System.Globalization;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemRepository : IItemsRepository
    {
        private List<Item> itemRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ItemRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            this.repositoryNameProvider = repositoryNameProvider;
            this.Load();
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
            uint itemId = itemRepository.OrderByDescending(item => item.ItemId).Select(item => item.ItemId).FirstOrDefault() + 1;
            Item itemToAdd = new Item() { ItemId = itemId, ItemName = itemName };
            itemRepository.Add(itemToAdd);
        }

        public void Modify(Item item)
        {
            itemRepository.Where(repositoryItem => repositoryItem.ItemId == item.ItemId).FirstOrDefault().ItemName = item.ItemName;
        }

        public void Remove(uint itemId)
        {
            itemRepository.Remove(itemRepository.Where(repositoryItem => repositoryItem.ItemId == itemId).FirstOrDefault());
        }

        public void Save()
        {
            XElement elements = new XElement("Items");
            foreach (Item item in itemRepository)
            {
                elements.Add(new XElement("Item", new XElement("ItemId") { Value = item.ItemId.ToString(CultureInfo.InvariantCulture) }, new XElement("ItemName") { Value = item.ItemName }));
            }
            elements.Save(repositoryNameProvider.RepositoryName);
        }

        private void Load()
        {
            if(repositoryNameProvider != null)
            { 
                if (!File.Exists(repositoryNameProvider.RepositoryName))
                {
                    XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Items"));
                    newRepository.Save(repositoryNameProvider.RepositoryName);
                }

                XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

                itemRepository = new List<Item>();
                foreach (XElement element in parsedFile.Elements("Items").Elements("Item"))
                {
                    itemRepository.Add(new Item() { ItemId = Convert.ToUInt32(element.Element("ItemId").Value, CultureInfo.InvariantCulture), ItemName = element.Element("ItemName").Value });
                }
            }
        }
    }
}
