using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXmlRepositoryName : BaseRepositoryName
    {
        public ShoppingListXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
            : base(userInformation, dataPathProvider)
        {
        }

        public override string RepositoryName
        {
            get { return ComputeRepositoryName(@"\ShoppingListRepository."); }
        }
    }
}