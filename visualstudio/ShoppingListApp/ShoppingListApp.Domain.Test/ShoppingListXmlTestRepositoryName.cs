using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Test
{
    public class ShoppingListXmlTestRepositoryName : IRepositoryNameProvider
    {
        public string RepositoryName
        {
            get { return @"./ShoppingListRepository.example.xml"; }
        }
    }
}
