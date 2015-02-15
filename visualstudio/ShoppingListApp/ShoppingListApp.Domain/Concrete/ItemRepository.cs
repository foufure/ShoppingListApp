using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using System.Xml.Linq;
using ShoppingListApp.Domain.Entities;
using System.IO;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemRepository : IItemsRepository
    {
        private List<Item> itemRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ItemRepository(IRepositoryNameProvider repositoryNameProviderParam)
        {
            repositoryNameProvider = repositoryNameProviderParam;

            if (!File.Exists(repositoryNameProvider.repositoryName))
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Items"));
                newRepository.Save(repositoryNameProvider.repositoryName);
            }

            XDocument parsedFile = XDocument.Load(repositoryNameProvider.repositoryName);
            
            
            itemRepository = new List<Item>();
            foreach (XElement element in parsedFile.Elements("Items").Elements("Item"))
            {
                itemRepository.Add(new Item() { ItemID = Convert.ToUInt32(element.Element("ItemID").Value), ItemName = element.Element("ItemName").Value });
            }
        }

        public IEnumerable<Entities.Item> repository
        { 
            get
            {
                return itemRepository;
            }
        }

        public void Add(string itemName)
        {
            uint itemID = itemRepository.OrderByDescending(item => item.ItemID).Select(item => item.ItemID).FirstOrDefault() + 1;
            Item itemToAdd = new Item() { ItemID = itemID, ItemName = itemName };
            itemRepository.Add(itemToAdd);
        }

        public void Modify(Item item)
        {
            itemRepository.Where(repositoryItem => repositoryItem.ItemID == item.ItemID).FirstOrDefault().ItemName = item.ItemName;
        }

        public void Remove(uint itemID)
        {
            itemRepository.Remove(itemRepository.Where(repositoryItem => repositoryItem.ItemID == itemID).FirstOrDefault());
        }

        public void Save()
        {
            XElement elements = new XElement("Items");
            foreach (Item item in itemRepository)
            {
                elements.Add(new XElement("Item", new XElement("ItemID") { Value = item.ItemID.ToString() }, new XElement("ItemName") { Value = item.ItemName }));
            }
            elements.Save(repositoryNameProvider.repositoryName);
        }
    }
}
