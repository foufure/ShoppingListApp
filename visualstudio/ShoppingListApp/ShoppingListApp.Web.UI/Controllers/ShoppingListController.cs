﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;
using Ninject;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private IShoppingListRepository shoppingListRepository;
        private IItemsRepository itemRepository;
        private IRepositoryNameProvider shoppingListsRepositoryName;

        public ShoppingListController(IShoppingListRepository shoppingListRepository, IItemsRepository itemRepository, [Named("ShoppingListRepositoryName")] IRepositoryNameProvider shoppingListsRepositoryName)
        {
            this.shoppingListRepository = shoppingListRepository;
            this.itemRepository = itemRepository;
            this.shoppingListsRepositoryName = shoppingListsRepositoryName;
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

            ViewBag.LastWriteTime = System.IO.File.GetLastWriteTime(shoppingListsRepositoryName.RepositoryName);

            return View((shoppingListId == null) ?
                shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListDueDate.Date >= DateTime.Now.Date)
                                                 .OrderBy(shoppinglist => shoppinglist.ShoppingListDueDate)
                                                 .DefaultIfEmpty(shoppingListRepository.Repository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListDueDate).First())
                                                 .FirstOrDefault() :
                shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault());
        }

        public ActionResult FastModifyShoppingList(uint shoppingListId)
        {
            return View(shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault());
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
                    shoppinglistToCreate.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0, LinePresentationOrder = 0, Unit = UnitsUtils.Units["default"], Done = false });
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
                    shoppinglistToModify.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = item, QuantityToBuy = 0, LinePresentationOrder = 0, Unit = UnitsUtils.Units["default"], Done = false });
                }
                else 
                {
                    //Item Category is only saved centrally in the item repository and not in the shoppinglists to always remain up-to-date and not create a dependency
                    shoppinglistToModify.ShoppingListContent.Where(line => line.ItemToBuy.ItemId == item.ItemId).FirstOrDefault().ItemToBuy.ItemCategory = item.ItemCategory;
                }
            }

            ViewBag.existingCategories = new List<string>(shoppinglistToModify.ShoppingListContent.Select(item => item.ItemToBuy.ItemCategory).Distinct().OrderBy(category => category).ToList());

            return View("AddShoppingList", shoppinglistToModify);
        }

        public ActionResult DeleteShoppingListLine(uint shoppingListId, uint itemId, string returnUrl)
        {
            ShoppingList shoppinglistToModify = shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault();
            ShoppingListLine shoppinglistLineToRemove = shoppinglistToModify.ShoppingListContent.Where(item => item.ItemToBuy.ItemId == itemId).FirstOrDefault();

            shoppinglistToModify.ShoppingListContent.Remove(shoppinglistLineToRemove);
            shoppingListRepository.Save();

            return Redirect(returnUrl);
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

                foreach (ShoppingListLine line in shoppingListToSave.ShoppingListContent)
                {
                    if (line.LinePresentationOrder == 0)
                    {
                        line.LinePresentationOrder = shoppingListToSave.ShoppingListContent.OrderByDescending(lastLine => lastLine.LinePresentationOrder).FirstOrDefault().LinePresentationOrder + 1;
                    }
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

        public ActionResult ResetAllDoneElements(uint shoppinglistIdToReset, string returnUrl)
        {
            foreach (ShoppingListLine shoppingListLine in shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppinglistIdToReset).FirstOrDefault().ShoppingListContent)
            {
                shoppingListLine.Done = false;
            }

            shoppingListRepository.Save();
            return Redirect(returnUrl);
        }

        // AJAX call!
        public JsonResult GetLastChangedDate()
        {
            string lastChangedDate = Convert.ToString(System.IO.File.GetLastWriteTime(shoppingListsRepositoryName.RepositoryName));
            return Json(lastChangedDate);
        }

        // AJAX call!
        public void ToggleStrikeOnDoneElement(uint shoppinglistIdToToggle, uint itemIdToToggle)
        {
            shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppinglistIdToToggle)
                                             .FirstOrDefault().ShoppingListContent
                                             .Where(item => item.ItemToBuy.ItemId == itemIdToToggle)
                                             .FirstOrDefault()
                                             .Done ^= true; // XOR operator to toggle value
            shoppingListRepository.Save();
        }

        // AJAX call!
        public void UpdateShoppingListLinesOrder(int  id, int fromPosition, int toPosition, string direction)
        {
            string incomingURL = Request.Url.PathAndQuery;
            uint shoppingListId = Convert.ToUInt32(incomingURL.Split('/').LastOrDefault(), CultureInfo.InvariantCulture);

            ShoppingList shoppingListToUpdate = shoppingListRepository.Repository
                            .Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId)
                            .FirstOrDefault();

            if (direction == "back")
            {
                List<ShoppingListLine> shoppingListLinesToReorder = shoppingListToUpdate.ShoppingListContent
                            .Where(shoppingListLine => (toPosition <= shoppingListLine.LinePresentationOrder && shoppingListLine.LinePresentationOrder <= fromPosition))
                            .ToList();

                foreach (ShoppingListLine shoppingListLine in shoppingListLinesToReorder)
                {
                    shoppingListLine.LinePresentationOrder++;
                }
            }
            else
            {
                List<ShoppingListLine> shoppingListLinesToReorder = shoppingListToUpdate.ShoppingListContent
                            .Where(shoppingListLine => (fromPosition <= shoppingListLine.LinePresentationOrder && shoppingListLine.LinePresentationOrder <= toPosition))
                            .ToList();

                foreach (ShoppingListLine shoppingListLine in shoppingListLinesToReorder)
                {
                    shoppingListLine.LinePresentationOrder--;
                }
            }

            shoppingListToUpdate.ShoppingListContent.Where(line => line.ItemToBuy.ItemId == id).FirstOrDefault().LinePresentationOrder = toPosition;

            shoppingListRepository.Save();
        }
    }
}