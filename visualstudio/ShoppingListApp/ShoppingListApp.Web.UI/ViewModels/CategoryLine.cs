using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.ViewModels
{
    public class CategoryLine
    {
        public Item ItemToCategorize { get; set; }
        
        public bool CategorySelection { get; set; }
    }
}