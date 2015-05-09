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
    public class ShoppingListXmlRepository : BaseRepository, IShoppingListRepository
    {
        private List<ShoppingList> shoppingListRepository = null;

        public ShoppingListXmlRepository(IRepositoryNameProvider repositoryNameProvider, IDataPathProvider dataPathProvider)
            : base(repositoryNameProvider, dataPathProvider)
        {
            this.InitializeXmlPersistentStorage("ShoppingList", RepositoriesXsd.ShoppingLists());
            this.LoadFromXmlPersistentStorage();
        }

        public IEnumerable<ShoppingList> Repository
        {
            get
            {
                return shoppingListRepository;
            }
        }

        public void ResetToEmpty()
        {
            ResetToEmptyRepository("ShoppingLists");
        }

        public void Add(ShoppingList newShoppingList)
        {
            if (newShoppingList == null)
            {
                throw new ArgumentNullException("Internal Error: the shopping list to add is empty or null. Please enter a valid shopping list", (Exception)null);
            }

            newShoppingList.ShoppingListId = shoppingListRepository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListId).Select(shoppinglist => shoppinglist.ShoppingListId).FirstOrDefault() + 1;
            shoppingListRepository.Add(newShoppingList);
        }

        public void Remove(uint shoppingListId)
        {
            if (!shoppingListRepository.Remove(shoppingListRepository.Where(repositoryShoppingList => repositoryShoppingList.ShoppingListId == shoppingListId).FirstOrDefault()))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the item to be deleted does not exist. Please enter a valid Shopping List Id", (Exception)null);
            }
        }

        public void Modify(ShoppingList shoppingList)
        {
            int shoppingListNotFound = 0;

            if (shoppingList == null)
            {
                throw new ArgumentNullException("Internal Error: the shopping list to modify is empty or null. Please enter a valid shopping list", (Exception)null);
            }

            if (shoppingListRepository.RemoveAll(item => item.ShoppingListId == shoppingList.ShoppingListId) == shoppingListNotFound)
            {
                throw new ArgumentOutOfRangeException("Internal Error: the shopping list to modify does not exist in the repository. Please enter a valid shopping list", (Exception)null);
            }

            shoppingListRepository.Add(shoppingList);
        }

        public void DeleteShoppingListLine(uint shoppingListId, uint itemId)
        {
            ShoppingList shoppingListToModify = shoppingListRepository.Where(shoppingList => shoppingList.ShoppingListId == shoppingListId).FirstOrDefault();

            if (shoppingListToModify != null)
            { 
                ShoppingListLine shoppingListLineToRemove = shoppingListToModify.ShoppingListContent.Where(item => item.ItemToBuy.ItemId == itemId).FirstOrDefault();
                shoppingListToModify.ShoppingListContent.Remove(shoppingListLineToRemove);
            }
        }

        public void ResetAllDoneElementsFromShoppingList(uint shoppingListId)
        {
            ShoppingList shoppingListToModify = shoppingListRepository.Where(shoppingList => shoppingList.ShoppingListId == shoppingListId).FirstOrDefault();

            if (shoppingListToModify != null)
            {
                foreach (ShoppingListLine shoppingListLine in shoppingListToModify.ShoppingListContent)
                {
                    shoppingListLine.Done = false;
                }
            }
        }

        public void ToggleShoppingListLineDoneStatus(uint shoppingListId, uint itemId)
        {
            ShoppingList shoppingListToModify = shoppingListRepository.Where(shoppingList => shoppingList.ShoppingListId == shoppingListId).FirstOrDefault();

            if (shoppingListToModify != null)
            { 
                ShoppingListLine shoppingListLineToModify = shoppingListToModify.ShoppingListContent.Where(item => item.ItemToBuy.ItemId == itemId).FirstOrDefault();

                if (shoppingListLineToModify != null)
                {
                    shoppingListLineToModify.Done ^= true; // XOR operator to toggle value
                }
            }
        }

        public void ReorderShoppingListLines(uint shoppingListId, uint itemId, int initialPositionOfElementToMove, int targetPositionOfElementToMove, string directionInWhichToMoveElement)
        {
            ShoppingList shoppingList = shoppingListRepository.Where(shoppingListToReorder => shoppingListToReorder.ShoppingListId == shoppingListId).FirstOrDefault();

            if (shoppingList != null)
            {
                ShoppingListLine elementToMove = shoppingList.ShoppingListContent.Where(line => line.ItemToBuy.ItemId == itemId).FirstOrDefault();

                if (elementToMove != null)
                { 
                    List<ShoppingListLine> shoppingListLinesToReorder = null;

                    if (directionInWhichToMoveElement == "back")
                    {
                        shoppingListLinesToReorder = shoppingList.ShoppingListContent
                                    .Where(shoppingListLine => (targetPositionOfElementToMove <= shoppingListLine.LinePresentationOrder && shoppingListLine.LinePresentationOrder <= initialPositionOfElementToMove))
                                    .ToList();

                        foreach (ShoppingListLine shoppingListLine in shoppingListLinesToReorder)
                        {
                            shoppingListLine.LinePresentationOrder++;
                        }
                    }
                    else
                    {
                        shoppingListLinesToReorder = shoppingList.ShoppingListContent
                                    .Where(shoppingListLine => (initialPositionOfElementToMove <= shoppingListLine.LinePresentationOrder && shoppingListLine.LinePresentationOrder <= targetPositionOfElementToMove))
                                    .ToList();

                        foreach (ShoppingListLine shoppingListLine in shoppingListLinesToReorder)
                        {
                            shoppingListLine.LinePresentationOrder--;
                        }
                    }

                    if (shoppingListLinesToReorder.Count != 0)
                    { 
                        elementToMove.LinePresentationOrder = targetPositionOfElementToMove;
                    }
                } 
            }
        }

        public void Save()
        {
            XElement elements = new XElement("ShoppingLists");
            foreach (ShoppingList shoppinglist in shoppingListRepository.OrderBy(list => list.ShoppingListId))
            {
                List<XElement> lines = new List<XElement>();

                foreach (ShoppingListLine line in shoppinglist.ShoppingListContent)
                {
                    lines.Add(new XElement(
                                            "ShoppingListLine",
                                            new XElement("ItemId") { Value = line.ItemToBuy.ItemId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ItemName") { Value = line.ItemToBuy.ItemName.ToString() },
                                            new XElement("ItemQuantity") { Value = line.QuantityToBuy.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("LinePresentationOrder") { Value = line.LinePresentationOrder.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("Unit") { Value = line.Unit },
                                            new XElement("Done") { Value = line.Done.ToString() }));
                }

                elements.Add(new XElement(
                                            "ShoppingList",
                                            new XElement("ShoppingListId") { Value = shoppinglist.ShoppingListId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ShoppingListName") { Value = shoppinglist.ShoppingListName },
                                            new XElement("ShoppingListDueDate") { Value = shoppinglist.ShoppingListDueDate.Date.ToString("u", CultureInfo.InvariantCulture).Split(' ')[0] }, // Universal Date without Time
                                            lines));
            }

            elements.Save(RepositoryName);
        }

        public void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(RepositoryName);

            shoppingListRepository = new List<ShoppingList>();
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
                    newShoppingList.ShoppingListContent.Add(new ShoppingListLine()
                    {
                        ItemToBuy = new Item() { ItemId = Convert.ToUInt32(itemElement.Element("ItemId").Value, CultureInfo.InvariantCulture), ItemName = itemElement.Element("ItemName").Value },
                        QuantityToBuy = Convert.ToDecimal(itemElement.Element("ItemQuantity").Value, CultureInfo.InvariantCulture),
                        LinePresentationOrder = Convert.ToInt32(itemElement.Element("LinePresentationOrder").Value, CultureInfo.InvariantCulture),
                        Unit = itemElement.Element("Unit").Value,
                        Done = Convert.ToBoolean(itemElement.Element("Done").Value, CultureInfo.InvariantCulture)
                    });
                }

                shoppingListRepository.Add(newShoppingList);
            }
        }
    }
}
