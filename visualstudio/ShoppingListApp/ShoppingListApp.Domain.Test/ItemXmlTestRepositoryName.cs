using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Test
{
    public class ItemXmlTestRepositoryName : IRepositoryNameProvider
    {
        public string RepositoryName
        {
            get { return @"./ItemRepository.example.xml"; }
        }
    }
}
