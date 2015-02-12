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
            get 
            {
                try { return System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(type => type.Type.Contains("emailaddress")).First().Value.Split('@')[0]; }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        public string UserEmail
        {
            get 
            {
                try { return System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(type => type.Type.Contains("emailaddress")).First().Value; }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }
    }
}
