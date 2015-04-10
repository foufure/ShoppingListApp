using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public abstract class BaseRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;
        private IDataPathProvider dataPathProvider;

        protected BaseRepositoryName(IUserInformation userInformation, IDataPathProvider dataPathProvider)
        {
            this.userInformation = userInformation;
            this.dataPathProvider = dataPathProvider;
        }

        public abstract string RepositoryName
        {
            get;
        }

        protected string ComputeRepositoryName(string repositoryType)
        {
            return (userInformation.UserName != null) ? (dataPathProvider.DataPath + repositoryType + userInformation.UserName + @".xml") : null;
        }
    }
}
