using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class CategoryXmlRepositoryName : BaseRepositoryName
    {
        public CategoryXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
            : base(userInformation, dataPathProvider)
        {
        }

        override public string RepositoryName
        {
            get { return ComputeRepositoryName(@"\CategoryRepository."); }
        }
    }
}