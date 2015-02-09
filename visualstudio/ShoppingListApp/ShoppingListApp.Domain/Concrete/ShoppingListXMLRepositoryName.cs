using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXMLRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;

        public ShoppingListXMLRepositoryName(IUserInformation userInformationParam)
        {
            userInformation = userInformationParam;
        }

        public string repositoryName
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + @".xml"; }
        }
    }
}
