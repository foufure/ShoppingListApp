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

        public CategoryXmlRepositoryName(IUserInformation userInformation)
        {
            this.userInformation = userInformation;
        }

        public string RepositoryName
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\CategoryRepository." + userInformation.UserName + @".xml"; }
        }
    }
}