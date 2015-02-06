using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListApp.Domain.Entities
{
    public class ShoppingList
    {
        public uint ShoppingListID { get; set; }
        [Required(ErrorMessage = "Please enter a ShoppingList Name")]
        public string ShoppingListName { get; set; }
        [Required(ErrorMessage = "Please enter a ShoppingList Due Date")]
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
