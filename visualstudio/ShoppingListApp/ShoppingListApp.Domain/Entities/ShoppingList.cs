using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ShoppingListApp.i18n.Resources.ModelValidationMessages;

namespace ShoppingListApp.Domain.Entities
{
    public class ShoppingList
    {
        public uint ShoppingListID { get; set; }
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.i18n.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListNameError")]
        public string ShoppingListName { get; set; }
        [Required(ErrorMessageResourceType = typeof(ShoppingListApp.i18n.Resources.ModelValidationMessages.ModelValidationMessagesCommon), ErrorMessageResourceName = "ShoppingListDueDateError")]
        [DataType(DataType.Date)]
        public DateTime ShoppingListDueDate { get; set; }
        public List<ShoppingListLine> ShoppingListContent { get; set; } 
    }

    public class ShoppingListLine
    { 
        public Article ArticleToBuy { get; set; }
        public int QuantityToBuy { get; set; }
    }
}
