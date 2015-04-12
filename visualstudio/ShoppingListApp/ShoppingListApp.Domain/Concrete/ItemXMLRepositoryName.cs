using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemXmlRepositoryName : BaseRepositoryName
    {
        public ItemXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider) 
            : base(userInformation, dataPathProvider)
        {
        }

        public override string RepositoryName
        {
            get { return ComputeRepositoryName(@"\ItemRepository."); }
        }
    }
}
