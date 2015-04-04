using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.ViewModels
{
    public class CategoriesItemViewModel
    {
        public Item ItemToModify { get; set; }
        public List<string> CategoriesToChooseFrom { get; set; }
    }
}