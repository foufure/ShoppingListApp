using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXMLTestRepositoryName : IRepositoryNameProvider
    {
        public string repositoryName
        {
            get { return @"./ShoppingListRepository.example.xml"; }
        }
    }
}
