using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class CategoryXmlRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;
        private IDataPathProvider dataPathProvider;

        public CategoryXmlRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
        {
            this.userInformation = userInformation;
            this.dataPathProvider = dataPathProvider;
        }

        public string RepositoryName
        {
            get { return (userInformation.UserName != null) ? (dataPathProvider.DataPath + @"\CategoryRepository." + userInformation.UserName + @".xml") : null; }
        }
    }
}