﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class ShoppingListXmlRepositoryName : IRepositoryNameProvider
    {
        private IUserInformation userInformation;

        public ShoppingListXmlRepositoryName(IUserInformation userInformation)
        {
            this.userInformation = userInformation;
        }

        public string RepositoryName
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + @".xml"; }
        }
    }
}
