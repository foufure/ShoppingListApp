using System;
using System.Linq;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.Web.UI.ViewModels;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private IItemsRepository itemRepository;
        private ICategoryRepository categoryRepository;

        public ItemController(IItemsRepository itemRepository, ICategoryRepository categoryRepository)
        {
            this.itemRepository = itemRepository;
            this.categoryRepository = categoryRepository;
        }

        public ActionResult Items()
        {
            return View(itemRepository.Repository);
        }

        public ActionResult AddNewItem(string newItemName, string returnUrl)
        {
            if (!string.IsNullOrEmpty(newItemName))
            { 
                itemRepository.Add(newItemName);
                itemRepository.Save();
            }

            return Redirect(returnUrl);
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
            CategoriesItemViewModel categoriesItemViewModel = new CategoriesItemViewModel();

            categoriesItemViewModel.ItemToModify = itemRepository.Repository.Where(item => item.ItemId == itemToModifyId).FirstOrDefault();
            categoriesItemViewModel.CategoriesToChooseFrom = categoryRepository.Repository.ToList();

            return View(categoriesItemViewModel);
        }

        [HttpPost]
        public RedirectToRouteResult ModifyItem(uint itemToModifyId, string itemToModifyNewName, string itemToModifyCategory)
        {
            if (!string.IsNullOrEmpty(itemToModifyNewName))
            {
                itemRepository.ModifyName(itemToModifyId, itemToModifyNewName);
                itemRepository.ModifyCategory(itemToModifyId, itemToModifyCategory);
                itemRepository.Save();
            }
            
            return RedirectToAction("Items");
        } 
    }
}