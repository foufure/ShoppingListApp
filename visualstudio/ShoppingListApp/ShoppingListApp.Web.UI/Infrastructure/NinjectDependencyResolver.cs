﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Web.UI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IArticleRepository>().To<ArticleRepository>();
            kernel.Bind<IShoppingListRepository>().To<ShoppingListRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ArticleXMLRepositoryName>().WhenInjectedInto<IArticleRepository>();
            kernel.Bind<IRepositoryNameProvider>().To<ShoppingListXMLRepositoryName>().WhenInjectedInto<IShoppingListRepository>();
            kernel.Bind<IUserInformation>().To<GoogleUserInformation>();
            kernel.Bind<IBackupProcessor>().To<EmailBackupProcessor>().WithConstructorArgument("settings", new EmailSettings());
            
        }
    }
}