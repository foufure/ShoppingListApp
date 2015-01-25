using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ArticleXMLTestRepositoryName : IRepositoryNameProvider
    {
        public string repositoryName
        {
            get { return @"./ArticleRepository.example.xml"; }
        }
    }
}
