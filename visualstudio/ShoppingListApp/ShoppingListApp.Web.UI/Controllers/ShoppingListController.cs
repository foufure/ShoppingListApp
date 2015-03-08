using System;
using System.Linq;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private IShoppingListRepository shoppingListRepository;
        private IItemsRepository itemRepository;

        public ShoppingListController(IShoppingListRepository shoppingListRepository, IItemsRepository itemRepository)
        {
            this.shoppingListRepository = shoppingListRepository;
            this.itemRepository = itemRepository;
        }

        public ViewResult ShoppingLists()
        {
            return View(shoppingListRepository.Repository);
        }

        public ActionResult ShowShoppingList(uint? shoppingListId = null)
        {
            if (shoppingListRepository.Repository.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View((shoppingListId == null) ? 
                        shoppingListRepository.Repository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListDueDate).First() :
                        shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault());
        }

        public RedirectToRouteResult RemoveShoppingList(uint shoppingListToRemoveId)
        {
            shoppingListRepository.Remove(shoppingListToRemoveId);
            shoppingListRepository.Save();
            return RedirectToAction("ShoppingLists");
        }

        public ActionResult AddShoppingList(string shoppingListToAddName)
        {
            if (!string.IsNullOrEmpty(shoppingListToAddName))
            {
                uint defaultShoppingListId = 0;
                ShoppingList shoppinglistToCreate = new ShoppingList()
                {
                    ShoppingListId = defaultShoppingListId,
                    ShoppingListName = shoppingListToAddName,
                    ShoppingListDueDate = DateTime.Now
                };

                foreach (Item item in itemRepository.Repository)
                {
                    shoppinglistToCreate.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0 });
                }

                return View(shoppinglistToCreate);
            }

            return RedirectToAction("ShoppingLists");
        }

        public ViewResult ModifyShoppingList(uint shoppingListId)
        {
            ShoppingList shoppinglistToModify = shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault();
            
            foreach (Item item in itemRepository.Repository)
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
            if (ModelState.IsValid)
            { 
                shoppingListToSave.ShoppingListContent.RemoveAll(item => item.QuantityToBuy == 0);

                if (shoppingListRepository.Repository.Any(shoppinglist => shoppinglist.ShoppingListId == shoppingListToSave.ShoppingListId))
                {
                    shoppingListRepository.Modify(shoppingListToSave);
                }
                else 
                {
                    shoppingListRepository.Add(shoppingListToSave);
                }
            
                shoppingListRepository.Save();
                return RedirectToAction("ShowShoppingList", new { shoppingListId = shoppingListToSave.ShoppingListId });
            }

            return View("AddShoppingList", shoppingListToSave);
        }
    }
}