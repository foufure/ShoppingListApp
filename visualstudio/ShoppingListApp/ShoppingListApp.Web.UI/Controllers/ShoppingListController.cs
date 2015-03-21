using System;
using System.Collections.Generic;
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
                        shoppingListRepository.Repository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListDueDate).FirstOrDefault() :
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

                ViewBag.existingCategories = new List<string>(shoppinglistToCreate.ShoppingListContent.Select(item => item.ItemToBuy.ItemCategory).Distinct().OrderBy(category => category).ToList());

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
                else 
                {
                    //Item Category is only saved centrally in the item repository and not in the shoppinglists to alway remain up-to-date and not create a dependency
                    shoppinglistToModify.ShoppingListContent.Where(line => line.ItemToBuy.ItemId == item.ItemId).FirstOrDefault().ItemToBuy.ItemCategory = item.ItemCategory;
                }
            }

            ViewBag.existingCategories = new List<string>(shoppinglistToModify.ShoppingListContent.Select(item => item.ItemToBuy.ItemCategory).Distinct().OrderBy(category => category).ToList());

            return View("AddShoppingList", shoppinglistToModify);
        }

        public ActionResult SaveShoppingList(ShoppingList shoppingListToSave, string newItemName, string returnUrl)
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

                if (!string.IsNullOrEmpty(newItemName))
                {
                    itemRepository.Add(newItemName);
                    itemRepository.Save();

                    return Redirect(returnUrl);
                }

                return RedirectToAction("ShowShoppingList", new { shoppingListId = shoppingListToSave.ShoppingListId });
            }
            else 
            {
                return View("AddShoppingList", shoppingListToSave);
            }
        }
    }
}