using System.Collections.Generic;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface ICategoryRepository
    {
        IEnumerable<string> Repository { get; }

        string RepositoryName { get; }

        void ResetToDefault();

        void ResetToEmpty();

        void Add(string categoryName);

        void Remove(string categoryName);

        void Modify(string oldCategoryName, string newCategoryName);

        void Save();
    }
}
