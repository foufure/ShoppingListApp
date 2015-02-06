using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ShoppingListApp.Web.UI.Infrastructure.Abstract;

namespace ShoppingListApp.Web.UI.Infrastructure.Concrete
{
    public class FormsAuthenticationProvider : IAuthenticationProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);

            if (result)
            {
                FormsAuthentication.SetAuthCookie(username, false);
            }

            return result;
        }
    }
}