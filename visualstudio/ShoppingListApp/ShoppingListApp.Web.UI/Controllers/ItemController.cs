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
    public class ItemController : Controller
    {
        private IItemsRepository itemRepository;

        public ItemController(IItemsRepository repositoryParam)
        {
            itemRepository = repositoryParam;
        }

        public ActionResult Items()
        {
            return View(itemRepository.repository);
        }

        public RedirectToRouteResult AddNewItem(string newItemName)
        {
            if(newItemName != "")
            { 
                itemRepository.Add(newItemName);
                itemRepository.Save();
            }

            return RedirectToAction("Items");
        }

        public RedirectToRouteResult RemoveItem(uint itemToRemoveID)
        {
            itemRepository.Remove(itemToRemoveID);
            itemRepository.Save();
            return RedirectToAction("Items");
        }

        [HttpGet]
        public ViewResult ModifyItem(uint itemToModifyID)
        {
            return View(itemRepository.repository.Where(item => item.ItemID == itemToModifyID).FirstOrDefault());
        }

        [HttpPost]
        public RedirectToRouteResult ModifyItem(uint itemToModifyID, string itemToModifyNewName)
        {
            if (itemToModifyNewName != "")
            {
                itemRepository.Modify(new Item() { ItemID = itemToModifyID, ItemName = itemToModifyNewName });
                itemRepository.Save();
            }
            
            return RedirectToAction("Items");
        } 
    }
}