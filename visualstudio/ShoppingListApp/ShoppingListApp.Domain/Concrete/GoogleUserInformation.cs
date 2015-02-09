using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class GoogleUserInformation : IUserInformation
    {
        public string UserName
        {
            get { return System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(type => type.Type.Contains("emailaddress")).First().Value.Split('@')[0]; }
        }

        public string UserEmail
        {
            get { return System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(type => type.Type.Contains("emailaddress")).First().Value; }
        }
    }
}
