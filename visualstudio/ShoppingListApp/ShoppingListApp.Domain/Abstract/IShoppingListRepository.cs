using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IShoppingListRepository
    {
        IEnumerable<ShoppingList> Repository { get; }
        void Add(ShoppingList newShoppingList);
        void Remove(uint shoppingListId);
        void Modify(ShoppingList shoppingList);
        void Save();
    }
}
