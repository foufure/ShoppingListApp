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
        IEnumerable<ShoppingList> repository { get; }
        void Add(ShoppingList shoppinglistNew);
        void Remove(uint shoppinglistID);
        void Modify(ShoppingList shoppingList);
        void Save();
    }
}
