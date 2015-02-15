using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IItemsRepository
    {
        IEnumerable<Item> Repository { get;}
        void Add(string itemName);
        void Remove(uint itemId);
        void Modify(Item item);
        void Save();
    }
}
