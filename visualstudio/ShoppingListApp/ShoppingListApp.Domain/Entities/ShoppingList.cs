using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListApp.Domain.Entities
{
    public class ShoppingList
    {
        private List<ShoppingListLine> shoppingListContent;

        public ShoppingList()
        {
            shoppingListContent = new List<ShoppingListLine>();
        }

        public uint ShoppingListId { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.I18N.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListNameError")]
        public string ShoppingListName { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.I18N.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListDueDateError")]
        [DataType(DataType.Date)]
        public DateTime ShoppingListDueDate { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Reviewed. No inheritance needed. Performance win.")]
        public List<ShoppingListLine> ShoppingListContent 
        { 
            get 
            { 
                return shoppingListContent; 
            } 
        } 
    }
}
