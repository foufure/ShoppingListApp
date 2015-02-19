using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemXmlTestRepositoryName : IRepositoryNameProvider
    {
        public string RepositoryName
        {
            get { return @"./ItemRepository.example.xml"; }
        }
    }
}
