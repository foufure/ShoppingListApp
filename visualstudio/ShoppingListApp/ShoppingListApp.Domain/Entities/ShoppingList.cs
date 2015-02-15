using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ShoppingListApp.i18n.Resources.ModelValidationMessages;
using System.Collections.ObjectModel;

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
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.i18n.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListNameError")]
        public string ShoppingListName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.i18n.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListDueDateError")]
        [DataType(DataType.Date)]
        public DateTime ShoppingListDueDate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<ShoppingListLine> ShoppingListContent { get { return shoppingListContent; } } 
    }

    public class ShoppingListLine
    { 
        public Item ItemToBuy { get; set; }
        public int QuantityToBuy { get; set; }
    }
}
