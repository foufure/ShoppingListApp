using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.ViewModels
{
    public class ItemsCategoryViewModel
    {
        private List<CategoryLine> categoryLines;
        private string category;

        public ItemsCategoryViewModel()
        {
            this.categoryLines = null;
            this.category = null;
        }

        public List<CategoryLine> CategoryLines 
        { 
            get { return categoryLines; } 
            set { categoryLines = value; } 
        }
        
        public string Category 
        { 
            get { return this.category; } 
            set { this.category = value; } 
        }
    }
}