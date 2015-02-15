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
using System.Globalization;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private List<ShoppingList> shoppinglistRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ShoppingListRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            this.repositoryNameProvider = repositoryNameProvider;
            this.Load();
        }

        public IEnumerable<ShoppingList> Repository
        {
            get
            {
                return shoppinglistRepository;
            }
        }

        public void Add(ShoppingList newShoppingList)
        {
            shoppinglistRepository.Add(newShoppingList);
        }

        public void Remove(uint shoppingListId)
        {
            shoppinglistRepository.Remove(shoppinglistRepository.Where(repositoryShoppingList => repositoryShoppingList.ShoppingListId == shoppingListId).FirstOrDefault());
        }

        public void Modify(ShoppingList shoppingList)
        {
            shoppinglistRepository.RemoveAll( item => item.ShoppingListId == shoppingList.ShoppingListId);
            shoppinglistRepository.Add(shoppingList);
        }

        public void Save()
        {
            XElement elements = new XElement("ShoppingLists");
            foreach (ShoppingList shoppinglist in shoppinglistRepository.OrderBy(list => list.ShoppingListId))
            {
                List<XElement> lines = new List<XElement>();
                
                foreach(ShoppingListLine line in shoppinglist.ShoppingListContent)
                {
                    lines.Add(new XElement("ShoppingListLine",
                                            new XElement("ItemId") { Value = line.ItemToBuy.ItemId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ItemName") { Value = line.ItemToBuy.ItemName.ToString() },
                                            new XElement("ItemQuantity") { Value = line.QuantityToBuy.ToString(CultureInfo.InvariantCulture) }
                                            )
                              );
                }
                
                elements.Add(new XElement("ShoppingList",
                                            new XElement("ShoppingListId") { Value = shoppinglist.ShoppingListId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ShoppingListName") { Value = shoppinglist.ShoppingListName },
                                            new XElement("ShoppingListDueDate") { Value = shoppinglist.ShoppingListDueDate.Date.ToString("u", CultureInfo.InvariantCulture).Split(' ')[0] }, // Universal Date without Time
                                            lines
                                            )
                             );
            }
            elements.Save(repositoryNameProvider.RepositoryName);
        }

        public void Load()
        {
            if(repositoryNameProvider.RepositoryName != null)
            { 
                if (!File.Exists(repositoryNameProvider.RepositoryName))
                {
                    XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("ShoppingLists"));
                    newRepository.Save(repositoryNameProvider.RepositoryName);
                }
                XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

                shoppinglistRepository = new List<ShoppingList>();
                foreach (XElement element in parsedFile.Elements("ShoppingLists").Elements("ShoppingList"))
                {
                    ShoppingList newShoppingList = new ShoppingList()
                    {
                        ShoppingListId = Convert.ToUInt32(element.Element("ShoppingListId").Value, CultureInfo.InvariantCulture),
                        ShoppingListName = element.Element("ShoppingListName").Value,
                        ShoppingListDueDate = Convert.ToDateTime(element.Element("ShoppingListDueDate").Value, CultureInfo.InvariantCulture)
                    };
                    
                    foreach (XElement itemElement in element.Elements("ShoppingListLine"))
                    {
                        newShoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = Convert.ToUInt32(itemElement.Element("ItemId").Value, CultureInfo.InvariantCulture), ItemName = itemElement.Element("ItemName").Value }, QuantityToBuy = Convert.ToInt32(itemElement.Element("ItemQuantity").Value, CultureInfo.InvariantCulture) });
                    }

                    shoppinglistRepository.Add(newShoppingList);
                }
            }
        }
    }
}
