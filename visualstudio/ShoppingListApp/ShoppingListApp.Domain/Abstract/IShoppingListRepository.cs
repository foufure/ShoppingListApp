using System.Collections.Generic;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IShoppingListRepository
    {
        IEnumerable<ShoppingList> Repository { get; }

        string RepositoryName { get; }

        void ResetToEmpty();

        void Add(ShoppingList newShoppingList);
        
        void Remove(uint shoppingListId);
        
        void Modify(ShoppingList shoppingList);
        
        void Save();
    }
}
