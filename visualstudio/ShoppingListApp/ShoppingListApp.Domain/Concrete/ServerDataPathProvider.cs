using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ServerDataPathProvider : IDataPathProvider
    {
        public string DataPath
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/App_Data"); }
        }
    }
}
