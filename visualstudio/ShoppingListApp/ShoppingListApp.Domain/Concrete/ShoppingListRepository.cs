using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;
using System.Xml.Linq;
using ShoppingListApp.i18n.Utils;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private List<ShoppingList> shoppinglistRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ShoppingListRepository(IRepositoryNameProvider repositoryNameProviderParam)
        {
            repositoryNameProvider = repositoryNameProviderParam;
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.repositoryName);
            shoppinglistRepository = new List<ShoppingList>();
            foreach (XElement element in parsedFile.Elements("ShoppingLists").Elements("ShoppingList"))
            {
                List<ShoppingListLine> shoppingListContent = new List<ShoppingListLine>();
                foreach (XElement articleElement in element.Elements("ShoppingListLine"))
                {
                    shoppingListContent.Add( new ShoppingListLine() { ArticleToBuy= new Article() {ArticleID = Convert.ToUInt32(articleElement.Element("ArticleID").Value), ArticleName = articleElement.Element("ArticleName").Value}, QuantityToBuy = Convert.ToInt32(articleElement.Element("ArticleQuantity").Value)});
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
                                            new XElement("ArticleID") { Value = line.ArticleToBuy.ArticleID.ToString() },
                                            new XElement("ArticleName") { Value = line.ArticleToBuy.ArticleName.ToString() },
                                            new XElement("ArticleQuantity") { Value = line.QuantityToBuy.ToString() }
                                            )
                              );
                }
                
                elements.Add(new XElement("ShoppingList",
                                            new XElement("ShoppingListID") { Value = shoppinglist.ShoppingListID.ToString() },
                                            new XElement("ShoppingListName") { Value = shoppinglist.ShoppingListName },
                                            new XElement("ShoppingListDueDate") { Value = shoppinglist.ShoppingListDueDate.Date.ToString("d", CultureHelper.getCurrentCulture()) },
                                            lines
                                            )
                             );
            }
            elements.Save(repositoryNameProvider.repositoryName);
        }
    }
}
