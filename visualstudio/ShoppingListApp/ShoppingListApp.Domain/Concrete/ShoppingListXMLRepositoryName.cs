using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXmlRepositoryName : BaseRepositoryName
    {
        public ShoppingListXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
            : base(userInformation, dataPathProvider)
        {
        }

        override public string RepositoryName
        {
            get { return ComputeRepositoryName(@"\ShoppingListRepository."); }
        }
    }
}