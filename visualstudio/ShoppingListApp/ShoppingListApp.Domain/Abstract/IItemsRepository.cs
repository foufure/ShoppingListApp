using System.Collections.Generic;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IItemsRepository
    {
        IEnumerable<Item> Repository { get; }

        string RepositoryName { get; }

        void ResetToDefault();

        void ResetToEmpty();

        void Add(string itemName);
        
        void Remove(uint itemId);

        void ModifyName(uint itemId, string itemName);

        void ChangeItemCategory(uint itemId, string itemCategory);

        void UpdateChangedCategoryName(string oldCategoryName, string newCategoryName);
        
        void Save();
    }
}
