using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private IShoppingListRepository shoppingListRepository;
        private IItemsRepository itemRepository;

        private string lastChangedDateCookieName = "lastChangedDate";

        public ShoppingListController(IShoppingListRepository shoppingListRepository, IItemsRepository itemRepository)
        {
            this.shoppingListRepository = shoppingListRepository;
            this.itemRepository = itemRepository;
        }

        public ViewResult ShoppingLists()
        {
            return View(shoppingListRepository.Repository);
        }

        public ActionResult ShowShoppingList(uint shoppingListId)
        {
            SetCookieContainingLastChangedDateOfShoppingList(LastChangedDate());

            return View(shoppingListRepository.Repository.Where(shoppinglist => shoppinglist.ShoppingListId == shoppingListId).FirstOrDefault());
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

                ViewBag.existingCategories = ListOfExistingCategoriesContainingAtLeastOneItem(shoppinglistToCreate);

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
                    // Item Category is only saved centrally in the item repository and not in the shoppinglists to always remain up-to-date and not create a dependency
                    shoppinglistToModify.ShoppingListContent.Where(line => line.ItemToBuy.ItemId == item.ItemId).FirstOrDefault().ItemToBuy.ItemCategory = item.ItemCategory;
                }
            }

            ViewBag.existingCategories = ListOfExistingCategoriesContainingAtLeastOneItem(shoppinglistToModify);

            return View("AddShoppingList", shoppinglistToModify);
        }

        public ActionResult DeleteShoppingListLine(uint shoppingListId, uint itemId, string returnUrl)
        {
            shoppingListRepository.DeleteShoppingListLine(shoppingListId, itemId);
            shoppingListRepository.Save();

            return Redirect(returnUrl);
        }

        public ActionResult SaveShoppingList(ShoppingList shoppingListToSave, string newItemName, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                shoppingListToSave.ShoppingListContent.RemoveAll(item => item.QuantityToBuy == 0);

                foreach (ShoppingListLine line in shoppingListToSave.ShoppingListContent)
                {
                    if (line.LinePresentationOrder == 0)
                    {
                        line.LinePresentationOrder = shoppingListToSave.ShoppingListContent.OrderByDescending(lastLine => lastLine.LinePresentationOrder).FirstOrDefault().LinePresentationOrder + 1;
                    }
                }

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

        public ActionResult ResetAllDoneElements(uint shoppingListIdToReset, string returnUrl)
        {
            shoppingListRepository.ResetAllDoneElementsFromShoppingList(shoppingListIdToReset);
            shoppingListRepository.Save();

            return Redirect(returnUrl);
        }

        // AJAX call!
        public JsonResult GetShoppingListChangedStatus()
        {
            string changedStatus = bool.FalseString;

            string lastChangedDateOfCookie = GetCookieValueContainingLastChangedDateOfShoppingList();
            string actualLastChangedDate = LastChangedDate();

            if (lastChangedDateOfCookie != actualLastChangedDate)
            { 
                changedStatus = bool.TrueString;
            }

            return Json(changedStatus);
        }

        // AJAX call!
        public void ToggleStrikeOnDoneElement(uint shoppingListIdToToggle, uint itemIdToToggle)
        {
            shoppingListRepository.ToggleShoppingListLineDoneStatus(shoppingListIdToToggle, itemIdToToggle);
            shoppingListRepository.Save();

            SetCookieContainingLastChangedDateOfShoppingList(LastChangedDate());
        }

        // AJAX call!
        public void UpdateShoppingListLinesOrder(int id, int fromPosition, int toPosition, string direction)
        {
            string incomingURL = Request.Url.PathAndQuery;
            uint shoppingListId = Convert.ToUInt32(incomingURL.Split('/').LastOrDefault(), CultureInfo.InvariantCulture);

            shoppingListRepository.ReorderShoppingListLines(shoppingListId, (uint)id, fromPosition, toPosition, direction);
            shoppingListRepository.Save();

            SetCookieContainingLastChangedDateOfShoppingList(LastChangedDate());
        }

        private static List<string> ListOfExistingCategoriesContainingAtLeastOneItem(ShoppingList shoppingListToCreateOrModify)
        {
            return new List<string>(shoppingListToCreateOrModify.ShoppingListContent.Select(item => item.ItemToBuy.ItemCategory).Distinct().OrderBy(category => category).ToList());
        }

        private void SetCookieContainingLastChangedDateOfShoppingList(string lastChangedDate)
        {
            if (!string.IsNullOrEmpty(lastChangedDate))
            {
                HttpCookie lastChangedDateCookie = new HttpCookie(lastChangedDateCookieName, lastChangedDate);
                lastChangedDateCookie.Expires = DateTime.Now.AddYears(10);

                this.ControllerContext.HttpContext.Response.Cookies.Set(lastChangedDateCookie);
            }
        }

        private string GetCookieValueContainingLastChangedDateOfShoppingList()
        {
            return this.ControllerContext.HttpContext.Request.Cookies.Get(lastChangedDateCookieName).Value;
        }

        private string LastChangedDate()
        {
            return System.IO.File.GetLastWriteTime(shoppingListRepository.RepositoryName).ToString();
        }
    }
}