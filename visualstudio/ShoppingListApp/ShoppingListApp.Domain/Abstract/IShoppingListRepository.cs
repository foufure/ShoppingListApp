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

        void DeleteShoppingListLine(uint shoppingListId, uint itemId);

        void ResetAllDoneElementsFromShoppingList(uint shoppingListId);

        void ToggleShoppingListLineDoneStatus(uint shoppingListId, uint itemId);

        void ReorderShoppingListLines(uint shoppingListId, uint itemId, int initialPositionOfElementToMove, int targetPositionOfElementToMove, string directionInWhichToMoveElement);
        
        void Save();
    }
}
