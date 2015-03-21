﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.Web.UI.ViewModels;

namespace ShoppingListApp.Web.UI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private ICategoryRepository categoryRepository;
        private IItemsRepository itemRepository;

        public CategoryController(ICategoryRepository categoryRepository, IItemsRepository itemRepository)
        {
            this.categoryRepository = categoryRepository;
            this.itemRepository = itemRepository;
        }

        public ActionResult Categories()
        {
            return View(categoryRepository.Repository);
        }

        public ActionResult AddNewCategory(string newCategoryName, string returnUrl)
        {
            if (!string.IsNullOrEmpty(newCategoryName))
            {
                categoryRepository.Add(newCategoryName);
                categoryRepository.Save();
            }

            return Redirect(returnUrl);
        }

        public RedirectToRouteResult RemoveCategory(string categoryToRemove)
        {
            categoryRepository.Remove(categoryToRemove);
            categoryRepository.Save();
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public ViewResult ModifyCategory(string categoryToModify)
        {
            ItemsCategoryViewModel itemsCategoryViewModel = new ItemsCategoryViewModel();

            itemsCategoryViewModel.Category = categoryToModify;
            itemsCategoryViewModel.CategoryLines = new List<CategoryLine>();

            foreach (Item item in itemRepository.Repository)
            {
                if (item.ItemCategory == CategoryUtils.DefaultCategory || item.ItemCategory == categoryToModify)
                {
                    itemsCategoryViewModel.CategoryLines.Add(new CategoryLine() { itemToCategorize = item, categorySelection = (item.ItemCategory == categoryToModify) });
                }
            }

            return View("ModifyCategory", null, itemsCategoryViewModel);
        }

        [HttpPost]
        public RedirectToRouteResult ModifyCategory(ItemsCategoryViewModel itemsCategory, string newCategoryName)
        {
            if (!string.IsNullOrEmpty(newCategoryName))
            {
                categoryRepository.Modify(itemsCategory.Category, newCategoryName);
                categoryRepository.Save();
            }

            if (itemsCategory.CategoryLines != null)
            {
                foreach (CategoryLine categoryLine in itemsCategory.CategoryLines)
                {
                    if (categoryLine.categorySelection == true)
                    {
                        itemRepository.ModifyCategory(categoryLine.itemToCategorize.ItemId, newCategoryName);
                    }
                    else
                    {
                        itemRepository.ModifyCategory(categoryLine.itemToCategorize.ItemId, CategoryUtils.DefaultCategory);
                    }
                }
            }

            categoryRepository.Save();
            itemRepository.Save();

            return RedirectToAction("Categories");
        } 
    }
}