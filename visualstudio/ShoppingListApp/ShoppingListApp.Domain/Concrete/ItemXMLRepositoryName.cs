using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemXmlRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;
        private IDataPathProvider dataPathProvider;

        public ItemXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
        {
            this.userInformation = userInformation;
            this.dataPathProvider = dataPathProvider;
        }

        public string RepositoryName
        {
            get { return (userInformation.UserName != null) ? (dataPathProvider.DataPath + @"\ItemRepository." + userInformation.UserName + @".xml") : null; }
        }
    }
}
