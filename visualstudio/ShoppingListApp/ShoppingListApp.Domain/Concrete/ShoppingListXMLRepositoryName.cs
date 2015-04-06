using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXmlRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;
        private IDataPathProvider dataPathProvider;

        public ShoppingListXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
        {
            this.userInformation = userInformation;
            this.dataPathProvider = dataPathProvider;
        }

        public string RepositoryName
        {
            get { return (userInformation.UserName != null) ? (dataPathProvider.DataPath + @"\ShoppingListRepository." + userInformation.UserName + @".xml") : null; }
        }
    }
}
