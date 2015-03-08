using System;
using System.Linq;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private IItemsRepository itemRepository;

        public ItemController(IItemsRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public ActionResult Items()
        {
            return View(itemRepository.Repository);
        }

        public RedirectToRouteResult AddNewItem(string newItemName)
        {
            if (!string.IsNullOrEmpty(newItemName))
            { 
                itemRepository.Add(newItemName);
                itemRepository.Save();
            }

            return RedirectToAction("Items");
        }

        public RedirectToRouteResult RemoveItem(uint itemToRemoveId)
        {
            itemRepository.Remove(itemToRemoveId);
            itemRepository.Save();
            return RedirectToAction("Items");
        }

        [HttpGet]
        public ViewResult ModifyItem(uint itemToModifyId)
        {
            return View(itemRepository.Repository.Where(item => item.ItemId == itemToModifyId).FirstOrDefault());
        }

        [HttpPost]
        public RedirectToRouteResult ModifyItem(uint itemToModifyId, string itemToModifyNewName)
        {
            if (!string.IsNullOrEmpty(itemToModifyNewName))
            {
                itemRepository.Modify(itemToModifyId, itemToModifyNewName);
                itemRepository.Save();
            }
            
            return RedirectToAction("Items");
        } 
    }
}