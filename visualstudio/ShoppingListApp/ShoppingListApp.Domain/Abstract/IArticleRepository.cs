using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IArticleRepository
    {
        IEnumerable<Article> repository { get;}
        void Add(string articleName);
        void Remove(uint articleID);
        void Modify(Article article);
        void Save();
    }
}
