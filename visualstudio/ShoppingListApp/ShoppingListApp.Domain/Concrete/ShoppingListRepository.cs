using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;
using System.Xml.Linq;
using ShoppingListApp.i18n.Utils;
using System.IO;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private List<ShoppingList> shoppinglistRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ShoppingListRepository(IRepositoryNameProvider repositoryNameProviderParam)
        {
            repositoryNameProvider = repositoryNameProviderParam;

            if (!File.Exists(repositoryNameProvider.repositoryName))
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("ShoppingLists"));
                newRepository.Save(repositoryNameProvider.repositoryName);
            }
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.repositoryName);
            
            shoppinglistRepository = new List<ShoppingList>();
            foreach (XElement element in parsedFile.Elements("ShoppingLists").Elements("ShoppingList"))
            {
                List<ShoppingListLine> shoppingListContent = new List<ShoppingListLine>();
                foreach (XElement itemElement in element.Elements("ShoppingListLine"))
                {
                    shoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemID = Convert.ToUInt32(itemElement.Element("ItemID").Value), ItemName = itemElement.Element("ItemName").Value }, QuantityToBuy = Convert.ToInt32(itemElement.Element("ItemQuantity").Value) });
                }

                shoppinglistRepository.Add(new ShoppingList() { ShoppingListID = Convert.ToUInt32(element.Element("ShoppingListID").Value), 
                                                                ShoppingListName = element.Element("ShoppingListName").Value,
                                                                ShoppingListDueDate = Convert.ToDateTime(element.Element("ShoppingListDueDate").Value),
                                                                ShoppingListContent = shoppingListContent
                });
            }
        }

        public IEnumerable<ShoppingList> repository
        {
            get
            {
                return shoppinglistRepository;
            }
        }

        public void Add(ShoppingList shoppinglistNew)
        {
            shoppinglistRepository.Add(shoppinglistNew);
        }

        public void Remove(uint shoppinglistID)
        {
            shoppinglistRepository.Remove(shoppinglistRepository.Where(repositoryShoppingList => repositoryShoppingList.ShoppingListID == shoppinglistID).FirstOrDefault());
        }

        public void Modify(ShoppingList shoppingList)
        {
            shoppinglistRepository.RemoveAll( item => item.ShoppingListID == shoppingList.ShoppingListID);
            shoppinglistRepository.Add(shoppingList);
        }

        public void Save()
        {
            XElement elements = new XElement("ShoppingLists");
            foreach (ShoppingList shoppinglist in shoppinglistRepository.OrderBy(list => list.ShoppingListID))
            {
                List<XElement> lines = new List<XElement>();
                
                foreach(ShoppingListLine line in shoppinglist.ShoppingListContent)
                {
                    lines.Add(new XElement("ShoppingListLine",
                                            new XElement("ItemID") { Value = line.ItemToBuy.ItemID.ToString() },
                                            new XElement("ItemName") { Value = line.ItemToBuy.ItemName.ToString() },
                                            new XElement("ItemQuantity") { Value = line.QuantityToBuy.ToString() }
                                            )
                              );
                }
                
                elements.Add(new XElement("ShoppingList",
                                            new XElement("ShoppingListID") { Value = shoppinglist.ShoppingListID.ToString() },
                                            new XElement("ShoppingListName") { Value = shoppinglist.ShoppingListName },
                                            new XElement("ShoppingListDueDate") { Value = shoppinglist.ShoppingListDueDate.Date.ToString("u").Split(' ')[0] }, // Universal Date without Time
                                            lines
                                            )
                             );
            }
            elements.Save(repositoryNameProvider.repositoryName);
        }
    }
}
