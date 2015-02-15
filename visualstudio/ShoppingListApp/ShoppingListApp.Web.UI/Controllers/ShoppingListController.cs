using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private IShoppingListRepository shoppinglistRepository;
        private IItemsRepository itemRepository;

        public ShoppingListController(IShoppingListRepository repositoryParamShoppingLists, IItemsRepository repositoryParamItems)
        {
            shoppinglistRepository = repositoryParamShoppingLists;
            itemRepository = repositoryParamItems;
        }

        public ViewResult ShoppingLists()
        {
            return View(shoppinglistRepository.repository);
        }

        public ActionResult ShowShoppingList(uint? shoppingListID = null)
        {
            if (shoppinglistRepository.repository.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View( 
                (shoppingListID == null) ? 
                shoppinglistRepository.repository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListDueDate).First() :
                shoppinglistRepository.repository.Where(shoppinglist => shoppinglist.ShoppingListID == shoppingListID).FirstOrDefault()
                );
        }

        public RedirectToRouteResult RemoveShoppingList(uint shoppinglistToRemoveID)
        {
            shoppinglistRepository.Remove(shoppinglistToRemoveID);
            shoppinglistRepository.Save();
            return RedirectToAction("ShoppingLists");
        }

        public ActionResult AddShoppingList(string shoppinglistToAddName)
        {
            if(shoppinglistToAddName != "")
            { 
                List<ShoppingListLine> lines = new List<ShoppingListLine>();
                foreach(Item item in itemRepository.repository)
                {
                    lines.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0});
                }
            
                ShoppingList shoppinglistToCreate = new ShoppingList()
                {
                    ShoppingListID = shoppinglistRepository.repository
                                            .OrderByDescending(shoppinglist => shoppinglist.ShoppingListID)
                                            .Select(shoppinglist => shoppinglist.ShoppingListID)
                                            .FirstOrDefault() + 1,
                    ShoppingListName = shoppinglistToAddName,
                    ShoppingListDueDate = DateTime.Now,
                    ShoppingListContent = new List<ShoppingListLine>(lines)
                };

                return View(shoppinglistToCreate);
            }

            return RedirectToAction("ShoppingLists");
        }

        public ViewResult ModifyShoppingList(uint shoppinglistID)
        {
            ShoppingList shoppinglistToModify = shoppinglistRepository.repository.Where(shoppinglist => shoppinglist.ShoppingListID == shoppinglistID).FirstOrDefault();
            
            foreach(Item item in itemRepository.repository)
            {
                if (!shoppinglistToModify.ShoppingListContent.Any(x => x.ItemToBuy.ItemID == item.ItemID))
                {
                    shoppinglistToModify.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0 });
                }
            }

            return View("AddShoppingList", shoppinglistToModify);
        }

        public ActionResult SaveShoppingList(ShoppingList shoppingListToSave)
        {
            if(ModelState.IsValid)
            { 
                shoppingListToSave.ShoppingListContent.RemoveAll(item => item.QuantityToBuy == 0);

                if (shoppinglistRepository.repository.Any(shoppinglist => shoppinglist.ShoppingListID == shoppingListToSave.ShoppingListID))
                {
                    shoppinglistRepository.Modify(shoppingListToSave);
                }
                else 
                {
                    shoppinglistRepository.Add(shoppingListToSave);
                }
            
                shoppinglistRepository.Save();
                return RedirectToAction("ShowShoppingList", new { shoppingListID = shoppingListToSave.ShoppingListID });
            }

            return View("AddShoppingList", shoppingListToSave);
        }
    }
}