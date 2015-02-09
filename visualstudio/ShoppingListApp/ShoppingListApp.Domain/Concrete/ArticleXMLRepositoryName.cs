using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ArticleXMLRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;

        public ArticleXMLRepositoryName(IUserInformation userInformationParam)
        {
            userInformation = userInformationParam;
        }

        public string repositoryName
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ArticleRepository." + userInformation.UserName + @".xml"; }
        }
    }
}
