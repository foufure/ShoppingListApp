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
            return View(shoppinglistRepository.Repository);
        }

        public ActionResult ShowShoppingList(uint? shoppingListId = null)
        {
            if (shoppinglistRepository.Repository.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View( 
                (shoppingListId == null) ? 
                shoppinglistRepository.Repository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListDueDate).First() :
                shoppinglistRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault()
                );
        }

        public RedirectToRouteResult RemoveShoppingList(uint shoppinglistToRemoveId)
        {
            shoppinglistRepository.Remove(shoppinglistToRemoveId);
            shoppinglistRepository.Save();
            return RedirectToAction("ShoppingLists");
        }

        public ActionResult AddShoppingList(string shoppinglistToAddName)
        {
            if(shoppinglistToAddName != "")
            {
                ShoppingList shoppinglistToCreate = new ShoppingList()
                {
                    ShoppingListId = shoppinglistRepository.Repository
                                            .OrderByDescending(shoppinglist => shoppinglist.ShoppingListId)
                                            .Select(shoppinglist => shoppinglist.ShoppingListId)
                                            .FirstOrDefault() + 1,
                    ShoppingListName = shoppinglistToAddName,
                    ShoppingListDueDate = DateTime.Now
                };

                foreach(Item item in itemRepository.Repository)
                {
                    shoppinglistToCreate.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0 });
                }

                return View(shoppinglistToCreate);
            }

            return RedirectToAction("ShoppingLists");
        }

        public ViewResult ModifyShoppingList(uint shoppinglistId)
        {
            ShoppingList shoppinglistToModify = shoppinglistRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppinglistId).FirstOrDefault();
            
            foreach(Item item in itemRepository.Repository)
            {
                if (!shoppinglistToModify.ShoppingListContent.Any(x => x.ItemToBuy.ItemId == item.ItemId))
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

                if (shoppinglistRepository.Repository.Any(shoppinglist => shoppinglist.ShoppingListId == shoppingListToSave.ShoppingListId))
                {
                    shoppinglistRepository.Modify(shoppingListToSave);
                }
                else 
                {
                    shoppinglistRepository.Add(shoppingListToSave);
                }
            
                shoppinglistRepository.Save();
                return RedirectToAction("ShowShoppingList", new { shoppingListId = shoppingListToSave.ShoppingListId });
            }

            return View("AddShoppingList", shoppingListToSave);
        }
    }
}