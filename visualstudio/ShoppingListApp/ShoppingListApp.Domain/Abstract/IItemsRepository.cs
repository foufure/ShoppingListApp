using System.Collections.Generic;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IItemsRepository
    {
        IEnumerable<Item> Repository { get; }
        
        void Add(string itemName);
        
        void Remove(uint itemId);

        void Modify(uint itemId, string itemName);
        
        void Save();
    }
}
